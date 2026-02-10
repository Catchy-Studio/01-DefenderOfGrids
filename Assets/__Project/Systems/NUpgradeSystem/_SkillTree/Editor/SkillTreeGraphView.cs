using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace __Project.Systems.NUpgradeSystem._SkillTree.Editor
{
    public class SkillTreeGraphView : GraphView
    {
        private SkillTreeGraphWindow _window;
        private Dictionary<SkillData, SkillDataNode> _nodeMap = new Dictionary<SkillData, SkillDataNode>();
        private SkillData _lastSelectedSkillData;
        
        // Grid snapping settings
        private const float GridSize = 50f; // Snap every 50 pixels

        public SkillTreeGraphView(SkillTreeGraphWindow window)
        {
            _window = window;

            // Add grid background with custom spacing to match snap
            var gridBackground = new GridBackground();
            Insert(0, gridBackground);
            gridBackground.StretchToParentSize();

            // Enable zoom and pan
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            // Add minimap
            var minimap = new MiniMap { anchored = true };
            minimap.SetPosition(new Rect(10, 30, 200, 140));
            Add(minimap);

            // Enable copy/paste/delete
            serializeGraphElements = SerializeGraphElementsImplementation;
            unserializeAndPaste = UnserializeAndPasteImplementation;
            deleteSelection = DeleteSelectionImplementation;

            // Load style from embedded resource or use default
            var stylePath = SkillTreeEditorPaths.GraphStyleSheet;
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(stylePath);
            if (styleSheet != null)
            {
                styleSheets.Add(styleSheet);
            }

            // Enable node snapping to grid - register once
            graphViewChanged += OnGraphViewChanged;
            
            // Register keyboard shortcuts
            RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        private void UpdateSelection()
        {
            var selectedNode = selection.OfType<SkillDataNode>().FirstOrDefault();
            var newSelectedSkillData = selectedNode?.SkillData;
            
            // Only notify window if selection actually changed
            if (newSelectedSkillData != _lastSelectedSkillData)
            {
                _lastSelectedSkillData = newSelectedSkillData;
                _window.OnNodeSelected(newSelectedSkillData);
            }
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            // Update selection when context menu is opened
            UpdateSelection();
        }

        protected override void HandleEventBubbleUp(EventBase evt)
        {
            base.HandleEventBubbleUp(evt);
            
            // Update selection on mouse down events
            if (evt.eventTypeId == MouseDownEvent.TypeId())
            {
                schedule.Execute(() => UpdateSelection()).ExecuteLater(50);
            }
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            // Check for CTRL+D (or CMD+D on Mac)
            if ((evt.ctrlKey || evt.commandKey) && evt.keyCode == KeyCode.D)
            {
                DuplicateSelectedNodes();
                evt.StopPropagation();
            }
        }

        private void DuplicateSelectedNodes()
        {
            var selectedNodes = selection.OfType<SkillDataNode>().ToList();
            
            if (selectedNodes.Count == 0)
            {
                Debug.Log("No nodes selected to duplicate");
                return;
            }

            var controller = FindTreeController();
            if (controller == null)
            {
                EditorUtility.DisplayDialog("Error", "No SkillTreeController found!", "OK");
                return;
            }

            // Load the prefab for creating new scene nodes
            var skillNodePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                SkillTreeEditorPaths.SkillNodeDefaultPrefab
            );

            if (skillNodePrefab == null)
            {
                EditorUtility.DisplayDialog("Error", "Could not find SkillNode_Default.prefab!", "OK");
                return;
            }

            ClearSelection();
            var newNodes = new List<SkillDataNode>();

            foreach (var originalNode in selectedNodes)
            {
                var originalData = originalNode.SkillData;
                if (originalData == null)
                {
                    Debug.LogWarning("Skipping node with null SkillData");
                    continue;
                }

                // Generate unique name
                string originalPath = AssetDatabase.GetAssetPath(originalData);
                string originalName = originalData.name;
                string newName = GenerateUniqueName(originalName);

                // Create new SkillData by copying the original
                var newSkillData = Object.Instantiate(originalData);
                newSkillData.name = newName;

                // Generate unique ID for the new skill data
                string uniqueId = GenerateUniqueId(newName);
                
                // Use reflection to set the ID field
                var idField = typeof(SkillData).GetField("id", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (idField != null)
                {
                    idField.SetValue(newSkillData, uniqueId);
                }

                // Save the new asset
                string directory = System.IO.Path.GetDirectoryName(originalPath);
                string newPath = $"{directory}/{newName}.asset";
                newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);
                
                AssetDatabase.CreateAsset(newSkillData, newPath);
                
                // Clear requirements from duplicated data (to avoid circular dependencies)
                newSkillData.GetRequiredSkillList().Clear();
                newSkillData.GetTargetSkillsToRevealList().Clear();
                
                EditorUtility.SetDirty(newSkillData);

                Debug.Log($"Created duplicated SkillData: {newPath} with ID: {uniqueId}");

                // Create scene node FROM PREFAB (not from existing node)
                var originalSceneNode = controller.AllNodeList.FirstOrDefault(n => n != null && n.Data == originalData);
                if (originalSceneNode != null)
                {
                    // Instantiate from prefab instead of cloning existing node
                    var newSceneNode = PrefabUtility.InstantiatePrefab(skillNodePrefab) as GameObject;
                    if (newSceneNode != null)
                    {
                        newSceneNode.name = newName;
                        newSceneNode.transform.SetParent(controller.transform);
                        
                        // Offset position slightly from original
                        var originalPos = originalSceneNode.transform.position;
                        newSceneNode.transform.position = originalPos + new Vector3(1f, 1f, 0f);
                        newSceneNode.transform.localScale = originalSceneNode.transform.localScale;

                        var nodeBase = newSceneNode.GetComponent<SkillNodeBase>();
                        if (nodeBase != null)
                        {
                            // Set the data
                            nodeBase.SetData(newSkillData);
                            
                            // Add to controller
                            controller.AllNodeList.Add(nodeBase);
                            EditorUtility.SetDirty(controller);
                            EditorUtility.SetDirty(nodeBase);
                        }

                        Debug.Log($"Created duplicated scene node from prefab: {newName}");

                        // Create editor node
                        var originalGraphPos = originalNode.GetPosition();
                        var newGraphNode = new SkillDataNode(newSkillData);
                        
                        // Offset the graph position
                        var newGraphPos = new Vector2(originalGraphPos.x + 100, originalGraphPos.y + 100);
                        newGraphNode.SetPosition(new Rect(newGraphPos, originalGraphPos.size));
                        
                        _nodeMap[newSkillData] = newGraphNode;
                        AddElement(newGraphNode);
                        
                        newNodes.Add(newGraphNode);

                        Debug.Log($"Created duplicated editor node at position {newGraphPos}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Could not find original scene node for {originalName}");
                }
            }

            AssetDatabase.SaveAssets();

            // Select the new nodes
            foreach (var newNode in newNodes)
            {
                AddToSelection(newNode);
            }

            Debug.Log($"Duplicated {newNodes.Count} node(s) successfully! All created fresh from prefab.");
        }

        private string GenerateUniqueId(string baseName)
        {
            // Convert to lowercase and replace spaces/special chars with underscores
            string id = baseName.ToLower().Replace(" ", "_");
            id = System.Text.RegularExpressions.Regex.Replace(id, @"[^a-z0-9_]", "");
            
            // Find all existing IDs
            var allSkillData = SkillTreeSearchProvider.FindAllSkillData();
            var existingIds = new HashSet<string>();
            
            foreach (var skill in allSkillData)
            {
                string existingId = skill.GetID();
                if (!string.IsNullOrEmpty(existingId))
                {
                    existingIds.Add(existingId);
                }
            }

            // Make sure ID is unique
            string uniqueId = id;
            int counter = 1;
            
            while (existingIds.Contains(uniqueId))
            {
                uniqueId = $"{id}_{counter}";
                counter++;
            }

            return uniqueId;
        }

        private string GenerateUniqueName(string baseName)
        {
            // Remove any existing Unity-style number suffix (e.g., " 0", " 1", " 2")
            baseName = System.Text.RegularExpressions.Regex.Replace(baseName, @" \d+$", "");
            
            // Find all existing names
            var allSkillData = SkillTreeSearchProvider.FindAllSkillData();
            var existingNames = new HashSet<string>(allSkillData.Select(s => s.name));

            // Check if base name already exists
            if (!existingNames.Contains(baseName))
            {
                return baseName;
            }

            // Generate unique name using Unity's convention starting from 0
            int counter = 0;
            string newName = $"{baseName} {counter}";
            
            while (existingNames.Contains(newName))
            {
                counter++;
                newName = $"{baseName} {counter}";
            }

            return newName;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            // Handle moved elements with grid snapping
            if (change.movedElements != null)
            {
                foreach (var element in change.movedElements)
                {
                    if (element is SkillDataNode node)
                    {
                        var pos = node.GetPosition();
                        var snappedX = Mathf.Round(pos.x / GridSize) * GridSize;
                        var snappedY = Mathf.Round(pos.y / GridSize) * GridSize;
                        
                        node.SetPosition(new Rect(snappedX, snappedY, pos.width, pos.height));
                    }
                }
            }
            schedule.Execute(() => UpdateSelection()).ExecuteLater(10);

            return change;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
                endPort.direction != startPort.direction &&
                endPort.node != startPort.node
            ).ToList();
        }

        public void ClearGraph()
        {
            // Remove all edges
            edges.ToList().ForEach(edge => RemoveElement(edge));

            // Remove all nodes
            nodes.ToList().ForEach(node => RemoveElement(node));

            _nodeMap.Clear();
        }

        public void LoadFromTreeController(SkillTreeController controller)
        {
            if (controller == null || controller.AllNodeList == null)
                return;

            // Sync check: Remove null or invalid nodes from controller
            controller.AllNodeList.RemoveAll(n => n == null || n.Data == null);
            EditorUtility.SetDirty(controller);

            // Create nodes
            foreach (var nodeBase in controller.AllNodeList)
            {
                if (nodeBase == null || nodeBase.Data == null)
                    continue;

                var graphNode = CreateSkillNode(nodeBase.Data, nodeBase.transform.position);
                AddElement(graphNode);
            }

            // Create edges based on requirements and saved port types
            foreach (var kvp in _nodeMap)
            {
                var skillData = kvp.Key;
                var graphNode = kvp.Value;

                if (skillData.GetRequiredSkillList() == null || skillData.GetRequiredSkillList().Count == 0)
                    continue;

                // Clean up null references in requirements
                skillData.GetRequiredSkillList().RemoveAll(r => r == null);

                for (int i = 0; i < skillData.GetRequiredSkillList().Count; i++)
                {
                    var requiredSkill = skillData.GetRequiredSkillList()[i];
                    
                    if (requiredSkill == null || !_nodeMap.ContainsKey(requiredSkill))
                        continue;

                    var requiredNode = _nodeMap[requiredSkill];
                    
                    // Get both saved port types for this connection
                    string savedInputPortType = skillData.GetPortTypeForRequiredSkill(i);
                    string savedOutputPortType = skillData.GetOutputPortTypeForRequiredSkill(i);
                    
                    Port targetPort = GetPortByType(graphNode, savedInputPortType);
                    Port sourcePort = GetPortByType(requiredNode, savedOutputPortType);
                    
                    if (targetPort != null && sourcePort != null)
                    {
                        var edge = sourcePort.ConnectTo(targetPort);
                        AddElement(edge);
                        Debug.Log($"Connected {requiredSkill.GetTitle()} ({savedOutputPortType}) to {skillData.GetTitle()} ({savedInputPortType})");
                    }
                    else
                    {
                        // Fallback if saved port types are invalid - use default ports
                        targetPort = graphNode.LeftInputPort;
                        sourcePort = requiredNode.RightOutputPort;
                        var edge = sourcePort.ConnectTo(targetPort);
                        AddElement(edge);
                        Debug.LogWarning($"Fallback connection for {requiredSkill.GetTitle()} to {skillData.GetTitle()}");
                    }
                }
                
                if (skillData.GetRequiredSkillList().Count > 0)
                {
                    EditorUtility.SetDirty(skillData);
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"Loaded {_nodeMap.Count} nodes. Sync validated and cleaned up.");
        }

        private (Port outputPort, Port inputPort) FindBestPortPair(SkillDataNode sourceNode, SkillDataNode targetNode)
        {
            var sourcePos = sourceNode.GetPosition().center;
            var targetPos = targetNode.GetPosition().center;
            
            var delta = targetPos - sourcePos;
            var absX = Mathf.Abs(delta.x);
            var absY = Mathf.Abs(delta.y);

            // Determine direction based on position
            if (absX > absY)
            {
                // Horizontal connection
                if (delta.x > 0)
                {
                    // Target is to the right
                    return (sourceNode.RightOutputPort, targetNode.LeftInputPort);
                }
                else
                {
                    // Target is to the left
                    return (sourceNode.LeftOutputPort, targetNode.RightInputPort);
                }
            }
            else
            {
                // Vertical connection
                if (delta.y > 0)
                {
                    // Target is below
                    return (sourceNode.BottomOutputPort, targetNode.TopInputPort);
                }
                else
                {
                    // Target is above
                    return (sourceNode.TopOutputPort, targetNode.BottomOutputPort);
                }
            }
        }

        public void SaveToTreeController(SkillTreeController controller)
        {
            if (controller == null)
                return;

            // Update skill requirements based on connections
            foreach (var kvp in _nodeMap)
            {
                var skillData = kvp.Key;
                var graphNode = kvp.Value;

                // Clear current requirements and port types
                skillData.GetRequiredSkillList().Clear();
                skillData.GetRequiredSkillPortTypes().Clear();
                skillData.GetRequiredSkillOutputPortTypes().Clear();

                // Get all incoming connections from all input ports
                var incomingEdges = edges.ToList()
                    .Where(e => e.input.node == graphNode && graphNode.AllInputPorts.Contains(e.input))
                    .ToList();

                int portIndex = 0;
                foreach (var edge in incomingEdges)
                {
                    if (edge.output.node is SkillDataNode sourceNode)
                    {
                        if (!skillData.GetRequiredSkillList().Contains(sourceNode.SkillData))
                        {
                            skillData.GetRequiredSkillList().Add(sourceNode.SkillData);
                            
                            // Save both input and output port types used for this connection
                            string inputPortType = GetPortType(graphNode, edge.input);
                            string outputPortType = GetPortType(sourceNode, edge.output);
                            
                            skillData.SetPortTypeForRequiredSkill(portIndex, inputPortType);
                            skillData.SetOutputPortTypeForRequiredSkill(portIndex, outputPortType);
                            portIndex++;
                        }
                    }
                }

                EditorUtility.SetDirty(skillData);
            }

            // Update positions in scene
            foreach (var nodeBase in controller.AllNodeList)
            {
                if (nodeBase == null || nodeBase.Data == null || !_nodeMap.ContainsKey(nodeBase.Data))
                    continue;

                var graphNode = _nodeMap[nodeBase.Data];
                var worldPos = new Vector3(
                    graphNode.GetPosition().x * 0.01f,
                    nodeBase.transform.position.y,
                    graphNode.GetPosition().y * 0.01f
                );
                nodeBase.transform.position = worldPos;
                EditorUtility.SetDirty(nodeBase.gameObject);
            }

            AssetDatabase.SaveAssets();
        }

        public void SaveConnectionsOnly(SkillTreeController controller)
        {
            if (controller == null)
                return;

            Debug.Log("Saving connections only (preserving port types)...");

            // Update skill requirements based on current connections
            foreach (var kvp in _nodeMap)
            {
                var skillData = kvp.Key;
                var graphNode = kvp.Value;

                // Clear current requirements and port types
                skillData.GetRequiredSkillList().Clear();
                skillData.GetRequiredSkillPortTypes().Clear();
                skillData.GetRequiredSkillOutputPortTypes().Clear();

                // Get all incoming connections from all input ports
                var incomingEdges = edges.ToList()
                    .Where(e => e.input.node == graphNode && graphNode.AllInputPorts.Contains(e.input))
                    .ToList();

                int portIndex = 0;
                foreach (var edge in incomingEdges)
                {
                    if (edge.output.node is SkillDataNode sourceNode)
                    {
                        if (!skillData.GetRequiredSkillList().Contains(sourceNode.SkillData))
                        {
                            skillData.GetRequiredSkillList().Add(sourceNode.SkillData);
                            
                            // Save both input and output port types used for this connection
                            string inputPortType = GetPortType(graphNode, edge.input);
                            string outputPortType = GetPortType(sourceNode, edge.output);
                            
                            skillData.SetPortTypeForRequiredSkill(portIndex, inputPortType);
                            skillData.SetOutputPortTypeForRequiredSkill(portIndex, outputPortType);
                            portIndex++;
                        }
                    }
                }

                EditorUtility.SetDirty(skillData);
            }

            // Also mark the controller as dirty
            EditorUtility.SetDirty(controller);
            
            AssetDatabase.SaveAssets();
            Debug.Log("Connections saved successfully with port information preserved!");
        }

        public void SetPositionsToScene(SkillTreeController controller)
        {
            if (controller == null)
                return;

            Debug.Log("Setting editor positions to scene...");

            // Conversion scale: must match GetPositionsFromScene
            const float scaleMultiplier = 150f;

            // Update positions in scene based on editor positions
            foreach (var nodeBase in controller.AllNodeList)
            {
                if (nodeBase == null || nodeBase.Data == null || !_nodeMap.ContainsKey(nodeBase.Data))
                    continue;

                var graphNode = _nodeMap[nodeBase.Data];
                var graphPos = graphNode.GetPosition();
                
                // Convert editor position (pixels) to scene position (units)
                var worldPos = new Vector3(
                    graphPos.x / scaleMultiplier,
                    -graphPos.y / scaleMultiplier,
                    nodeBase.transform.position.z
                );
                
                nodeBase.transform.position = worldPos;
                EditorUtility.SetDirty(nodeBase.gameObject);
                
                Debug.Log($"Updated {nodeBase.name}: graph ({graphPos.x}, {graphPos.y}) -> scene ({worldPos.x}, {worldPos.y}, {worldPos.z})");
            }

            Debug.Log("Scene positions updated successfully!");
        }

        private SkillDataNode CreateSkillNode(SkillData skillData, Vector3 worldPosition)
        {
            var node = new SkillDataNode(skillData);
            
            // Convert world position to graph position
            var graphPos = new Vector2(worldPosition.x * 100f, worldPosition.z * 100f);
            node.SetPosition(new Rect(graphPos, Vector2.zero));

            _nodeMap[skillData] = node;
            return node;
        }

        public void AutoLayoutNodes()
        {
            if (_nodeMap.Count == 0)
                return;

            // Simple hierarchical layout
            var processedNodes = new HashSet<SkillDataNode>();
            var rootNodes = _nodeMap.Values.Where(n => 
            {
                var incomingEdges = edges.ToList().Where(e => e.input.node == n);
                return !incomingEdges.Any();
            }).ToList();

            float xOffset = 100f;
            float yOffset = 100f;
            float xSpacing = 300f;
            float ySpacing = 200f;

            int currentLevel = 0;
            var currentLevelNodes = new List<SkillDataNode>(rootNodes);

            while (currentLevelNodes.Count > 0)
            {
                for (int i = 0; i < currentLevelNodes.Count; i++)
                {
                    var node = currentLevelNodes[i];
                    if (processedNodes.Contains(node))
                        continue;

                    var pos = new Vector2(
                        xOffset + (i * xSpacing),
                        yOffset + (currentLevel * ySpacing)
                    );
                    node.SetPosition(new Rect(pos, node.GetPosition().size));
                    processedNodes.Add(node);
                }

                // Get next level nodes
                var nextLevel = new List<SkillDataNode>();
                foreach (var node in currentLevelNodes)
                {
                    var outgoingEdges = edges.ToList()
                        .Where(e => e.output.node == node)
                        .Select(e => e.input.node as SkillDataNode)
                        .Where(n => n != null && !processedNodes.Contains(n));

                    nextLevel.AddRange(outgoingEdges);
                }

                currentLevelNodes = nextLevel.Distinct().ToList();
                currentLevel++;
            }
        }

        public void CreateTreeLayout()
        {
            if (_nodeMap.Count == 0)
                return;

            const float nodeWidth = 280f;
            const float nodeHeight = 200f;
            const float horizontalSpacing = nodeWidth * 2f;
            const float verticalSpacing = nodeHeight * 2f;

            // Find root nodes (nodes with no requirements)
            var rootNodes = new List<SkillDataNode>();
            foreach (var kvp in _nodeMap)
            {
                var skillData = kvp.Key;
                var requirements = skillData.GetRequiredSkillList();
                if (requirements == null || requirements.Count == 0)
                {
                    rootNodes.Add(kvp.Value);
                }
            }

            if (rootNodes.Count == 0)
            {
                Debug.LogWarning("No root nodes found. Creating layout from first node.");
                rootNodes.Add(_nodeMap.Values.First());
            }

            // Place root nodes in the center
            var centerX = 0f;
            var centerY = 0f;
            
            if (rootNodes.Count == 1)
            {
                rootNodes[0].SetPosition(new Rect(centerX, centerY, nodeWidth, nodeHeight));
            }
            else
            {
                float radius = horizontalSpacing;
                for (int i = 0; i < rootNodes.Count; i++)
                {
                    float angle = (i * 360f / rootNodes.Count) * Mathf.Deg2Rad;
                    float x = centerX + radius * Mathf.Cos(angle);
                    float y = centerY + radius * Mathf.Sin(angle);
                    rootNodes[i].SetPosition(new Rect(x, y, nodeWidth, nodeHeight));
                }
            }

            // Process nodes level by level outward
            var processedNodes = new HashSet<SkillDataNode>(rootNodes);
            var currentLevelNodes = new Queue<SkillDataNode>(rootNodes);
            var levelIndex = 0;

            while (currentLevelNodes.Count > 0)
            {
                var nextLevelNodes = new Queue<SkillDataNode>();
                var nodesThisLevel = currentLevelNodes.ToList();

                foreach (var node in nodesThisLevel)
                {
                    // Find children (nodes that require this node)
                    var children = new List<SkillDataNode>();
                    foreach (var kvp in _nodeMap)
                    {
                        if (processedNodes.Contains(kvp.Value))
                            continue;

                        var requirements = kvp.Key.GetRequiredSkillList();
                        if (requirements != null && requirements.Contains(node.SkillData))
                        {
                            children.Add(kvp.Value);
                        }
                    }

                    if (children.Count > 0)
                    {
                        var parentPos = node.GetPosition().center;
                        
                        for (int i = 0; i < children.Count; i++)
                        {
                            float baseAngle = Mathf.Atan2(parentPos.y - centerY, parentPos.x - centerX);
                            float spreadAngle = 45f * Mathf.Deg2Rad;
                            float angleOffset = (i - (children.Count - 1) / 2f) * spreadAngle / Mathf.Max(1, children.Count - 1);
                            float angle = baseAngle + angleOffset;
                            
                            float distance = Mathf.Sqrt(horizontalSpacing * horizontalSpacing + verticalSpacing * verticalSpacing);
                            float x = parentPos.x + distance * Mathf.Cos(angle);
                            float y = parentPos.y + distance * Mathf.Sin(angle);
                            
                            children[i].SetPosition(new Rect(x - nodeWidth / 2, y - nodeHeight / 2, nodeWidth, nodeHeight));
                            
                            if (!processedNodes.Contains(children[i]))
                            {
                                processedNodes.Add(children[i]);
                                nextLevelNodes.Enqueue(children[i]);
                            }
                        }
                    }
                }

                currentLevelNodes = nextLevelNodes;
                levelIndex++;

                // Safety break to prevent infinite loops
                if (levelIndex > 100)
                {
                    Debug.LogWarning("Tree layout exceeded maximum depth. Some nodes may not be positioned.");
                    break;
                }
            }

            // Position any remaining unprocessed nodes (orphans)
            var orphanNodes = _nodeMap.Values.Where(n => !processedNodes.Contains(n)).ToList();
            if (orphanNodes.Count > 0)
            {
                float orphanX = centerX + horizontalSpacing * 4;
                float orphanY = centerY;
                
                for (int i = 0; i < orphanNodes.Count; i++)
                {
                    orphanNodes[i].SetPosition(new Rect(
                        orphanX,
                        orphanY + i * verticalSpacing,
                        nodeWidth,
                        nodeHeight
                    ));
                }
            }

            // Frame all nodes after layout
            FrameAll();
        }

        public void GetPositionsFromScene(SkillTreeController controller)
        {
            if (controller == null)
            {
                Debug.LogError("GetPositionsFromScene: Controller is null!");
                return;
            }
            
            if (controller.AllNodeList == null)
            {
                Debug.LogError("GetPositionsFromScene: Controller.AllNodeList is null!");
                return;
            }

            Debug.Log($"GetPositionsFromScene: Loading {controller.AllNodeList.Count} nodes from scene...");

            // Clear existing graph
            ClearGraph();

            const float scaleMultiplier = 150f;
            int nodesLoaded = 0;

            // Create nodes with positions from scene
            foreach (var nodeBase in controller.AllNodeList)
            {
                if (nodeBase == null)
                {
                    Debug.LogWarning("GetPositionsFromScene: Skipping null nodeBase");
                    continue;
                }
                
                if (nodeBase.Data == null)
                {
                    Debug.LogWarning($"GetPositionsFromScene: Skipping nodeBase {nodeBase.name} with null Data");
                    continue;
                }

                var scenePos = nodeBase.transform.position;
                var graphPos = new Vector2(scenePos.x * scaleMultiplier, -scenePos.y * scaleMultiplier);
                
                Debug.Log($"Creating node '{nodeBase.Data.GetTitle()}' at scene position ({scenePos.x}, {scenePos.y}) -> graph position {graphPos}");
                
                var graphNode = new SkillDataNode(nodeBase.Data);
                graphNode.SetPosition(new Rect(graphPos, Vector2.zero));
                
                _nodeMap[nodeBase.Data] = graphNode;
                AddElement(graphNode);
                nodesLoaded++;
            }

            Debug.Log($"GetPositionsFromScene: Loaded {nodesLoaded} nodes");

            // Create edges using saved port type information
            int edgesCreated = 0;
            foreach (var kvp in _nodeMap)
            {
                var skillData = kvp.Key;
                var targetNode = kvp.Value;

                if (skillData.GetRequiredSkillList() == null || skillData.GetRequiredSkillList().Count == 0)
                    continue;
                
                for (int i = 0; i < skillData.GetRequiredSkillList().Count; i++)
                {
                    var requiredSkill = skillData.GetRequiredSkillList()[i];
                    
                    if (requiredSkill == null || !_nodeMap.ContainsKey(requiredSkill))
                        continue;

                    var sourceNode = _nodeMap[requiredSkill];
                    
                    // Get both saved port types for this connection
                    string savedInputPortType = skillData.GetPortTypeForRequiredSkill(i);
                    string savedOutputPortType = skillData.GetOutputPortTypeForRequiredSkill(i);
                    
                    Port targetPort = GetPortByType(targetNode, savedInputPortType);
                    Port sourcePort = GetPortByType(sourceNode, savedOutputPortType);
                    
                    if (targetPort != null && sourcePort != null)
                    {
                        var edge = sourcePort.ConnectTo(targetPort);
                        AddElement(edge);
                        Debug.Log($"Connected {sourceNode.SkillData.GetTitle()} ({savedOutputPortType}) to {targetNode.SkillData.GetTitle()} ({savedInputPortType})");
                        edgesCreated++;
                    }
                    else
                    {
                        Debug.LogWarning($"Could not find ports - Input: '{savedInputPortType}', Output: '{savedOutputPortType}'");
                    }
                }
            }

            Debug.Log($"GetPositionsFromScene: Created {edgesCreated} edges");
            FrameAll();
            Debug.Log("GetPositionsFromScene: Complete!");
        }

        public void HighlightNodes(List<SkillData> skillsToHighlight)
        {
            // Clear current selection
            ClearSelection();

            if (skillsToHighlight == null || skillsToHighlight.Count == 0)
                return;

            // Select matching nodes
            foreach (var skill in skillsToHighlight)
            {
                if (_nodeMap.ContainsKey(skill))
                {
                    var node = _nodeMap[skill];
                    AddToSelection(node);
                }
            }

            // Frame the first selected node
            if (skillsToHighlight.Count > 0 && _nodeMap.ContainsKey(skillsToHighlight[0]))
            {
                FrameSelection();
            }
        }

        private string SerializeGraphElementsImplementation(IEnumerable<GraphElement> elements)
        {
            // Implement if you want copy/paste functionality
            return "";
        }

        private void UnserializeAndPasteImplementation(string operationName, string data)
        {
            // Implement if you want copy/paste functionality
        }

        private void DeleteSelectionImplementation(string operationName, AskUser askUser)
        {
            var elementsToDelete = selection.OfType<GraphElement>().ToList();

            foreach (var element in elementsToDelete)
            {
                if (element is Edge edge)
                {
                    edge.input.Disconnect(edge);
                    edge.output.Disconnect(edge);
                    RemoveElement(edge);
                }
                else if (element is SkillDataNode node)
                {
                    // Store references before any deletion happens
                    var skillData = node.SkillData;
                    if (skillData == null)
                    {
                        Debug.LogWarning("Cannot delete node with null SkillData");
                        _nodeMap.Remove(skillData);
                        RemoveElement(node);
                        continue;
                    }

                    var nodeName = skillData.GetTitle();
                    var controller = FindTreeController();
                    
                    // Find scene node before showing dialog
                    SkillNodeBase sceneNode = null;
                    string sceneNodeName = "Unknown";
                    if (controller != null)
                    {
                        sceneNode = controller.AllNodeList.FirstOrDefault(n => n != null && n.Data == skillData);
                        if (sceneNode != null)
                        {
                            sceneNodeName = sceneNode.name;
                        }
                    }
                    
                    var deleteChoice = EditorUtility.DisplayDialogComplex(
                        "Delete Node",
                        $"Delete node '{nodeName}'?\n\nChoose how to delete:",
                        "Delete All (Editor + Scene + Data)",
                        "Cancel",
                        "Delete Editor + Scene Only"
                    );

                    if (deleteChoice == 1) // Cancel
                    {
                        continue;
                    }

                    bool deleteData = (deleteChoice == 0);
                    
                    // Step 1: Remove all connected edges from editor
                    var connectedEdges = edges.ToList()
                        .Where(e => e.input.node == node || e.output.node == node)
                        .ToList();

                    foreach (var connectedEdge in connectedEdges)
                    {
                        connectedEdge.input.Disconnect(connectedEdge);
                        connectedEdge.output.Disconnect(connectedEdge);
                        RemoveElement(connectedEdge);
                    }

                    // Step 2: Remove from editor graph
                    _nodeMap.Remove(skillData);
                    RemoveElement(node);
                    Debug.Log($"Removed node '{nodeName}' from editor");

                    // Step 3: Delete scene node if found
                    if (sceneNode != null && controller != null)
                    {
                        controller.AllNodeList.Remove(sceneNode);
                        EditorUtility.SetDirty(controller);
                        
                        Object.DestroyImmediate(sceneNode.gameObject);
                        Debug.Log($"Deleted scene node: {sceneNodeName}");
                    }
                    else if (controller != null)
                    {
                        Debug.LogWarning($"Scene node for '{nodeName}' not found in controller");
                    }

                    // Step 4: Delete SkillData asset if requested
                    if (deleteData)
                    {
                        string assetPath = AssetDatabase.GetAssetPath(skillData);
                        if (!string.IsNullOrEmpty(assetPath))
                        {
                            if (AssetDatabase.DeleteAsset(assetPath))
                            {
                                Debug.Log($"Deleted SkillData asset: {assetPath}");
                            }
                            else
                            {
                                Debug.LogError($"Failed to delete SkillData asset: {assetPath}");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"SkillData '{nodeName}' has no valid asset path");
                        }
                    }
                    
                    Debug.Log($"Successfully deleted node '{nodeName}' (Data deleted: {deleteData})");
                }
            }
            
            AssetDatabase.SaveAssets();
        }

        public void UpdateNodeVisuals(SkillData skillData)
        {
            if (skillData == null || !_nodeMap.ContainsKey(skillData))
                return;

            var oldNode = _nodeMap[skillData];
            var position = oldNode.GetPosition();

            // Store all connected edges with their exact port types
            var connectedEdges = edges.ToList()
                .Where(e => e.input.node == oldNode || e.output.node == oldNode)
                .ToList();

            // Store edge connections with port direction information
            var inputConnections = new List<(SkillDataNode source, string sourcePortType, string targetPortType)>();
            var outputConnections = new List<(string sourcePortType, SkillDataNode target, string targetPortType)>();

            foreach (var edge in connectedEdges)
            {
                if (edge.output.node == oldNode && edge.input.node is SkillDataNode targetNode)
                {
                    // This node is the source
                    string sourcePortType = GetPortType(oldNode, edge.output);
                    string targetPortType = GetPortType(targetNode, edge.input);
                    outputConnections.Add((sourcePortType, targetNode, targetPortType));
                }
                else if (edge.input.node == oldNode && edge.output.node is SkillDataNode sourceNode)
                {
                    // This node is the target
                    string sourcePortType = GetPortType(sourceNode, edge.output);
                    string targetPortType = GetPortType(oldNode, edge.input);
                    inputConnections.Add((sourceNode, sourcePortType, targetPortType));
                }

                // Remove old edge
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
                RemoveElement(edge);
            }

            // Remove old node
            RemoveElement(oldNode);
            _nodeMap.Remove(skillData);

            // Create new node with updated data
            var newNode = new SkillDataNode(skillData);
            newNode.SetPosition(position);
            _nodeMap[skillData] = newNode;
            AddElement(newNode);

            // Recreate connections using exact same ports
            foreach (var (sourceNode, sourcePortType, targetPortType) in inputConnections)
            {
                Port sourcePort = GetPortByType(sourceNode, sourcePortType);
                Port targetPort = GetPortByType(newNode, targetPortType);
                
                if (sourcePort != null && targetPort != null)
                {
                    var newEdge = sourcePort.ConnectTo(targetPort);
                    AddElement(newEdge);
                }
            }

            foreach (var (sourcePortType, targetNode, targetPortType) in outputConnections)
            {
                Port sourcePort = GetPortByType(newNode, sourcePortType);
                Port targetPort = GetPortByType(targetNode, targetPortType);
                
                if (sourcePort != null && targetPort != null)
                {
                    var newEdge = sourcePort.ConnectTo(targetPort);
                    AddElement(newEdge);
                }
            }

            Debug.Log($"Updated node visuals for '{skillData.GetTitle()}' - preserved all port connections");
        }

        private string GetPortType(SkillDataNode node, Port port)
        {
            // Identify which specific port this is
            if (port == node.TopInputPort) return "TopInput";
            if (port == node.BottomInputPort) return "BottomInput";
            if (port == node.LeftInputPort) return "LeftInput";
            if (port == node.RightInputPort) return "RightInput";
            if (port == node.TopOutputPort) return "TopOutput";
            if (port == node.BottomOutputPort) return "BottomOutput";
            if (port == node.LeftOutputPort) return "LeftOutput";
            if (port == node.RightOutputPort) return "RightOutput";
            return "Unknown";
        }

        private Port GetPortByType(SkillDataNode node, string portType)
        {
            // Get the specific port by its type
            return portType switch
            {
                "TopInput" => node.TopInputPort,
                "BottomInput" => node.BottomInputPort,
                "LeftInput" => node.LeftInputPort,
                "RightInput" => node.RightInputPort,
                "TopOutput" => node.TopOutputPort,
                "BottomOutput" => node.BottomOutputPort,
                "LeftOutput" => node.LeftOutputPort,
                "RightOutput" => node.RightOutputPort,
                _ => null
            };
        }

        private Port FindMatchingPort(SkillDataNode node, Port oldPort)
        {
            // Match port by name/position
            if (oldPort.portName == "R")
            {
                // Input port - match by orientation
                if (oldPort.orientation == Orientation.Vertical)
                {
                    return oldPort.style.top.value.value < 0 ? node.TopInputPort : node.BottomInputPort;
                }
                else
                {
                    return oldPort.style.left.value.value < 50 ? node.LeftInputPort : node.RightInputPort;
                }
            }
            else
            {
                // Output port - match by orientation
                if (oldPort.orientation == Orientation.Vertical)
                {
                    return oldPort.style.top.value.value < 0 ? node.TopOutputPort : node.BottomOutputPort;
                }
                else
                {
                    return oldPort.style.left.value.value < 50 ? node.LeftOutputPort : node.RightOutputPort;
                }
            }
        }

        private SkillTreeController FindTreeController()
        {
            return _window?.GetCurrentTreeController();
        }
    }
}

