using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace __Project.Systems.NUpgradeSystem._SkillTree.Editor
{
    public class SkillTreeGraphWindow : EditorWindow
    {
        private SkillTreeGraphView graphView;
        private SkillTreeController currentTreeController;
        private ObjectField treeControllerField;
        private bool autoFillNewNodes = false; // Toggle for auto-filling new nodes with random data
        private SkillData selectedSkillData;
        private ScrollView propertiesPanel;
        private VisualElement inspectorContainer;
        private VisualElement noControllerWarning;
        private VisualElement prefabModeWarning; // Warning when prefab is not open in prefab stage
        private Label modeLabel; // Label to show if in prefab or scene mode
        private bool isPrefabMode = false;
        private string namingPattern = "Skill_{index}_{title}"; // Default naming pattern for skills
        private float rightPanelWidth = 300f; // Current width of right panel
        private bool isResizingPanel = false;
        private VisualElement rightPanelContainer;

        public SkillTreeController GetCurrentTreeController() => currentTreeController;
        public bool IsAutoFillEnabled() => autoFillNewNodes;
        public bool IsPrefabMode() => isPrefabMode;
        
        public void OnNodeSelected(SkillData skillData)
        {
            selectedSkillData = skillData;
            RefreshInspector();
        }

        [MenuItem(SkillTreeEditorPaths.SkillTreeEditorMenuPath)]
        public static void OpenWindow()
        {
            var window = GetWindow<SkillTreeGraphWindow>();
            window.titleContent = new GUIContent("Skill Tree Editor");
            window.minSize = new Vector2(800, 600);
        }
        private void OnEnable()
        {
            // Register callbacks for automatic updates
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        private void OnDisable()
        {
            // Unregister callbacks
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.activeSceneChangedInEditMode -= OnActiveSceneChanged;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }

        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            // Scene was opened, check for controller
            CheckAndUpdateTreeController();
        }

        private void OnActiveSceneChanged(Scene previousScene, Scene newScene)
        {
            // Active scene changed, check for controller
            CheckAndUpdateTreeController();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // Play mode state changed, check for controller
            if (state == PlayModeStateChange.EnteredEditMode || state == PlayModeStateChange.ExitingPlayMode)
            {
                CheckAndUpdateTreeController();
            }
        }

        private void OnAfterAssemblyReload()
        {
            // After compilation/assembly reload, check for controller
            CheckAndUpdateTreeController();
        }

        private void CheckAndUpdateTreeController()
        {
            // Try to find controller if we don't have one or if it's been destroyed
            if (currentTreeController == null || currentTreeController.Equals(null))
            {
                var foundController = FindAnyObjectByType<SkillTreeController>();
                if (foundController != null)
                {
                    currentTreeController = foundController;
                    if (treeControllerField != null)
                    {
                        treeControllerField.value = foundController;
                    }
                    UpdateModeDetection();
                    Debug.Log($"[Skill Tree Editor] Auto-detected controller: {foundController.name}");
                    
                    // Delay the load to ensure UI is ready
                    EditorApplication.delayCall += () => LoadTree();
                }
                else
                {
                    // No controller found, show warning
                    if (noControllerWarning != null)
                    {
                        noControllerWarning.style.display = DisplayStyle.Flex;
                    }
                    if (graphView != null)
                    {
                        graphView.style.display = DisplayStyle.None;
                    }
                }
            }
            else
            {
                // We have a controller, make sure UI is showing correctly
                UpdateModeDetection();
                if (noControllerWarning != null)
                {
                    noControllerWarning.style.display = DisplayStyle.None;
                }
                if (graphView != null)
                {
                    graphView.style.display = DisplayStyle.Flex;
                }
            }
        }

        private void UpdateModeDetection()
        {
            if (currentTreeController == null)
            {
                isPrefabMode = false;
                UpdateModeLabel();
                HidePrefabModeWarning();
                return;
            }

            // Check if the controller is part of a prefab asset
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            
            // First check: Are we in a prefab stage?
            if (prefabStage != null)
            {
                // Check if the controller is the root or child of the prefab stage
                if (prefabStage.prefabContentsRoot == currentTreeController.gameObject)
                {
                    isPrefabMode = true;
                    HidePrefabModeWarning();
                    Debug.Log($"[Mode Detection] PREFAB MODE: Controller is prefab root");
                }
                else if (currentTreeController.transform.IsChildOf(prefabStage.prefabContentsRoot.transform))
                {
                    isPrefabMode = true;
                    HidePrefabModeWarning();
                    Debug.Log($"[Mode Detection] PREFAB MODE: Controller is child of prefab");
                }
                else
                {
                    isPrefabMode = false;
                    HidePrefabModeWarning();
                    Debug.Log($"[Mode Detection] SCENE MODE: In prefab stage but controller not part of it");
                }
            }
            else
            {
                // No prefab stage open - check if it's a prefab asset
                var assetPath = AssetDatabase.GetAssetPath(currentTreeController.gameObject);
                if (!string.IsNullOrEmpty(assetPath) && assetPath.EndsWith(".prefab"))
                {
                    isPrefabMode = true;
                    ShowPrefabModeWarning(); // Show warning - prefab detected but not open!
                    Debug.Log($"[Mode Detection] PREFAB MODE: Controller is prefab asset at: {assetPath} - WARNING: Not open in prefab stage!");
                }
                else
                {
                    isPrefabMode = false;
                    HidePrefabModeWarning();
                    Debug.Log($"[Mode Detection] SCENE MODE: Controller is in scene");
                }
            }

            UpdateModeLabel();
        }

        private void ShowPrefabModeWarning()
        {
            if (prefabModeWarning != null)
            {
                prefabModeWarning.style.display = DisplayStyle.Flex;
            }
            if (graphView != null)
            {
                graphView.style.opacity = 0.3f; // Dim the graph view
            }
        }

        private void HidePrefabModeWarning()
        {
            if (prefabModeWarning != null)
            {
                prefabModeWarning.style.display = DisplayStyle.None;
            }
            if (graphView != null)
            {
                graphView.style.opacity = 1.0f; // Restore full opacity
            }
        }

        private void UpdateModeLabel()
        {
            if (modeLabel != null)
            {
                if (currentTreeController == null)
                {
                    modeLabel.text = "Mode: None";
                    modeLabel.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
                }
                else if (isPrefabMode)
                {
                    modeLabel.text = "📦 PREFAB MODE";
                    modeLabel.style.backgroundColor = new Color(0.2f, 0.5f, 0.8f, 0.8f);
                }
                else
                {
                    modeLabel.text = "🎬 SCENE MODE";
                    modeLabel.style.backgroundColor = new Color(0.5f, 0.7f, 0.2f, 0.8f);
                }
            }
        }


        private void CreateGUI()
        {
            var root = rootVisualElement;

            // Main Toolbar
            var toolbar = new Toolbar();
            
            treeControllerField = new ObjectField("Tree Controller")
            {
                objectType = typeof(SkillTreeController),
                value = currentTreeController,
                style = { width = 200 }
            };
            treeControllerField.RegisterValueChangedCallback(evt =>
            {
                currentTreeController = evt.newValue as SkillTreeController;
                UpdateModeDetection();
                LoadTree();
            });
            toolbar.Add(treeControllerField);

            // Mode label
            modeLabel = new Label("Mode: None")
            {
                style = 
                {
                    paddingLeft = 10,
                    paddingRight = 10,
                    paddingTop = 4,
                    paddingBottom = 4,
                    marginLeft = 5,
                    backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.8f),
                    color = Color.white,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    fontSize = 11,
                    borderTopLeftRadius = 3,
                    borderTopRightRadius = 3,
                    borderBottomLeftRadius = 3,
                    borderBottomRightRadius = 3
                }
            };
            toolbar.Add(modeLabel);
            UpdateModeLabel();

            // Refresh button - loads from scene or prefab
            var refreshButton = new ToolbarButton(() => LoadTree()) 
            { 
                text = "🔄 Load Tree",
                style = 
                {
                    backgroundColor = new Color(0.2f, 0.5f, 0.8f, 0.3f),
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            };
            toolbar.Add(refreshButton);

            // Save connections only
            var saveButton = new ToolbarButton(() => SaveTree()) 
            { 
                text = "💾 Save Connections",
                style = 
                {
                    backgroundColor = new Color(0.2f, 0.7f, 0.2f, 0.3f),
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            };
            toolbar.Add(saveButton);

            toolbar.Add(new ToolbarSpacer());

            // Create new skill button
            var createNewDataButton = new ToolbarButton(() => CreateNewData()) 
            { 
                text = "➕ Create New Skill",
                style = 
                {
                    backgroundColor = new Color(0.5f, 0.8f, 0.2f, 0.3f),
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            };
            toolbar.Add(createNewDataButton);

            // Update positions button - works for both scene and prefab
            var setPositionsButton = new ToolbarButton(() => SetPositionsToScene()) 
            { 
                text = "📍 Update Positions",
                style = 
                {
                    backgroundColor = new Color(0.8f, 0.5f, 0.2f, 0.3f),
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            };
            toolbar.Add(setPositionsButton);

            // Sync button
            var syncButton = new ToolbarButton(() => SyncEditorWithScene()) 
            { 
                text = "🔗 Sync & Clean",
                style = 
                {
                    backgroundColor = new Color(0.7f, 0.3f, 0.7f, 0.3f),
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            };
            toolbar.Add(syncButton);

            root.Add(toolbar);

            // Secondary Toolbar with search
            var searchToolbar = new Toolbar();
            
            var searchField = new ToolbarSearchField();
            searchField.style.width = 300;
            searchField.RegisterValueChangedCallback(evt => SearchSkills(evt.newValue));
            searchToolbar.Add(searchField);

            var focusButton = new ToolbarButton(() => FocusAll()) { text = "🎯 Focus All" };
            searchToolbar.Add(focusButton);

            root.Add(searchToolbar);

            // Try to find tree controller in scene on startup
            CheckAndUpdateTreeController();

            // Create container for graph and fixed right panel
            var contentContainer = new VisualElement
            {
                style = 
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Row
                }
            };

            // Graph View (flexible - takes remaining space)
            graphView = new SkillTreeGraphView(this)
            {
                style = { flexGrow = 1 }
            };
            contentContainer.Add(graphView);
            // Create warning overlay for when no controller is attached
            CreateNoControllerWarning();
            contentContainer.Add(noControllerWarning);
            
            // Create warning overlay for when prefab is not open in prefab stage
            CreatePrefabModeWarning();
            contentContainer.Add(prefabModeWarning);


            // Right Panel Container (resizable by user)
            rightPanelContainer = new VisualElement
            {
                style = 
                {
                    width = rightPanelWidth,
                    minWidth = 200,
                    flexShrink = 0,
                    backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.95f),
                    borderLeftWidth = 2,
                    borderLeftColor = new Color(0.3f, 0.3f, 0.3f)
                }
            };

            // Add resize handle (draggable left edge)
            var resizeHandle = new VisualElement
            {
                name = "resize-handle",
                pickingMode = PickingMode.Position // Ensure it can capture mouse events
            };
            
            resizeHandle.style.position = Position.Absolute;
            resizeHandle.style.left = -3; // Position slightly to the left so it overlaps the border
            resizeHandle.style.top = 0;
            resizeHandle.style.bottom = 0;
            resizeHandle.style.width = 6; // Slightly wider for easier grabbing
            resizeHandle.style.backgroundColor = new Color(0.3f, 0.5f, 0.8f, 0.0f); // Transparent by default
            
            // Add hover effect
            resizeHandle.RegisterCallback<MouseEnterEvent>(_ =>
            {
                resizeHandle.style.backgroundColor = new Color(0.3f, 0.5f, 0.8f, 0.8f);
                resizeHandle.style.width = 6;
            });

            resizeHandle.RegisterCallback<MouseLeaveEvent>(_ =>
            {
                if (!isResizingPanel)
                {
                    resizeHandle.style.backgroundColor = new Color(0.3f, 0.5f, 0.8f, 0.0f);
                }
            });

            // Add resize functionality
            resizeHandle.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button == 0) // Left mouse button
                {
                    isResizingPanel = true;
                    resizeHandle.CaptureMouse();
                    resizeHandle.style.backgroundColor = new Color(0.5f, 0.7f, 1f, 1f); // Bright blue when active
                    evt.StopPropagation();
                }
            });

            resizeHandle.RegisterCallback<MouseMoveEvent>(evt =>
            {
                if (isResizingPanel)
                {
                    float newWidth = rightPanelContainer.resolvedStyle.width - evt.mouseDelta.x;
                    newWidth = Mathf.Clamp(newWidth, 200f, 800f); // Min 200, Max 800
                    rightPanelWidth = newWidth;
                    rightPanelContainer.style.width = rightPanelWidth;
                    evt.StopPropagation();
                }
            });

            resizeHandle.RegisterCallback<MouseUpEvent>(evt =>
            {
                if (isResizingPanel)
                {
                    isResizingPanel = false;
                    resizeHandle.ReleaseMouse();
                    resizeHandle.style.backgroundColor = new Color(0.3f, 0.5f, 0.8f, 0.0f);
                    evt.StopPropagation();
                }
            });

            rightPanelContainer.Add(resizeHandle);

            // Add Properties Panel at top of right panel
            CreatePropertiesPanel();
            rightPanelContainer.Add(propertiesPanel);

            // Add Separator
            var separator = new VisualElement
            {
                style = 
                {
                    height = 2,
                    backgroundColor = new Color(0.4f, 0.4f, 0.4f),
                    marginTop = 10,
                    marginBottom = 10,
                    marginLeft = 10,
                    marginRight = 10
                }
            };
            rightPanelContainer.Add(separator);

            // Add Inspector Panel below properties
            CreateInspectorPanel();
            rightPanelContainer.Add(inspectorContainer);

            contentContainer.Add(rightPanelContainer);
            root.Add(contentContainer);

            // Info Label at bottom
            var infoLabel = new Label("💡 Tip: CTRL+D to duplicate | Mouse wheel to zoom | Middle mouse to pan | Delete to remove nodes")
            {
                style = 
                {
                    paddingLeft = 10,
                    paddingTop = 5,
                    paddingBottom = 5,
                    fontSize = 10,
                    color = new Color(0.7f, 0.7f, 0.7f)
                }
            };
            root.Add(infoLabel);

            // Try to find tree controller in scene
            if (currentTreeController == null)
            {
                currentTreeController = FindAnyObjectByType<SkillTreeController>();
                if (currentTreeController != null)
                {
                    treeControllerField.value = currentTreeController;
                    LoadTree();
                }
            }
        }

        private void CreatePropertiesPanel()
        {
            propertiesPanel = new ScrollView(ScrollViewMode.Vertical);
            propertiesPanel.style.paddingLeft = 10;
            propertiesPanel.style.paddingRight = 10;
            propertiesPanel.style.paddingTop = 10;
            propertiesPanel.style.paddingBottom = 10;
            propertiesPanel.style.maxHeight = 250;

            // Header
            var headerLabel = new Label("⚙️ Editor Properties")
            {
                style = 
                {
                    fontSize = 14,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginBottom = 10,
                    color = new Color(0.9f, 0.9f, 0.9f)
                }
            };
            propertiesPanel.Add(headerLabel);

            // Section: Creation Settings
            var creationSectionLabel = new Label("Creation Settings")
            {
                style = 
                {
                    fontSize = 12,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginBottom = 8,
                    color = new Color(0.7f, 0.9f, 1f)
                }
            };
            propertiesPanel.Add(creationSectionLabel);

            // Auto-fill toggle
            var autoFillToggle = new Toggle("Auto-Fill Random Data")
            {
                value = autoFillNewNodes,
                tooltip = "When enabled, newly created skills will be automatically filled with random data"
            };
            autoFillToggle.RegisterValueChangedCallback(evt =>
            {
                autoFillNewNodes = evt.newValue;
                Debug.Log($"Auto-fill mode: {(autoFillNewNodes ? "ENABLED" : "DISABLED")}");
            });
            autoFillToggle.style.marginBottom = 5;
            propertiesPanel.Add(autoFillToggle);

            // Description for auto-fill
            var autoFillDescription = new Label("Automatically populate new skills with random title, description, tiers, and sprite.")
            {
                style = 
                {
                    fontSize = 10,
                    color = new Color(0.6f, 0.6f, 0.6f),
                    marginLeft = 18,
                    marginBottom = 15,
                    whiteSpace = WhiteSpace.Normal
                }
            };
            propertiesPanel.Add(autoFillDescription);

            // Naming pattern field
            var namingPatternLabel = new Label("Naming Pattern")
            {
                style = 
                {
                    fontSize = 11,
                    marginTop = 10,
                    marginBottom = 3,
                    color = new Color(0.7f, 0.9f, 1f)
                }
            };
            propertiesPanel.Add(namingPatternLabel);

            var namingPatternField = new TextField()
            {
                value = namingPattern,
                tooltip = "Pattern for naming nodes. Use {index} for node count, {title} for skill title.\nExample: Skill_{index}_{title}"
            };
            namingPatternField.RegisterValueChangedCallback(evt =>
            {
                namingPattern = evt.newValue;
                Debug.Log($"Naming pattern updated: {namingPattern}");
            });
            namingPatternField.style.marginBottom = 5;
            propertiesPanel.Add(namingPatternField);

            var patternHelpLabel = new Label("Use {index} for count, {title} for skill title")
            {
                style = 
                {
                    fontSize = 10,
                    color = new Color(0.6f, 0.6f, 0.6f),
                    marginLeft = 18,
                    marginBottom = 15,
                    whiteSpace = WhiteSpace.Normal
                }
            };
            propertiesPanel.Add(patternHelpLabel);
        }

        private void CreateInspectorPanel()
        {
            inspectorContainer = new ScrollView(ScrollViewMode.Vertical);
            inspectorContainer.style.paddingLeft = 10;
            inspectorContainer.style.paddingRight = 10;
            inspectorContainer.style.paddingTop = 10;
            inspectorContainer.style.paddingBottom = 10;
            inspectorContainer.style.flexGrow = 1;

            // Header
            var headerLabel = new Label("📝 Skill Inspector")
            {
                style = 
                {
                    fontSize = 14,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginBottom = 10,
                    color = new Color(0.9f, 0.9f, 0.9f)
                }
            };
            inspectorContainer.Add(headerLabel);

            // Initial empty state
            RefreshInspector();
        }

        private void CreateNoControllerWarning()
        {
            noControllerWarning = new VisualElement
            {
                style = 
                {
                    position = Position.Absolute,
                    left = 0,
                    right = 0,
                    top = 0,
                    bottom = 0,
                    backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.95f),
                    alignItems = Align.Center,
                    justifyContent = Justify.Center,
                    display = DisplayStyle.None // Hidden by default
                }
            };

            var warningContainer = new VisualElement
            {
                style = 
                {
                    backgroundColor = new Color(0.3f, 0.2f, 0.2f, 0.95f),
                    borderLeftWidth = 4,
                    borderRightWidth = 4,
                    borderTopWidth = 4,
                    borderBottomWidth = 4,
                    borderLeftColor = new Color(0.9f, 0.3f, 0.3f),
                    borderRightColor = new Color(0.9f, 0.3f, 0.3f),
                    borderTopColor = new Color(0.9f, 0.3f, 0.3f),
                    borderBottomColor = new Color(0.9f, 0.3f, 0.3f),
                    borderTopLeftRadius = 10,
                    borderTopRightRadius = 10,
                    borderBottomLeftRadius = 10,
                    borderBottomRightRadius = 10,
                    paddingLeft = 40,
                    paddingRight = 40,
                    paddingTop = 40,
                    paddingBottom = 40,
                    maxWidth = 500
                }
            };

            var warningIcon = new Label("⚠️")
            {
                style = 
                {
                    fontSize = 80,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    marginBottom = 20
                }
            };
            warningContainer.Add(warningIcon);

            var warningTitle = new Label("No Skill Tree Controller Found")
            {
                style = 
                {
                    fontSize = 24,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    color = new Color(1f, 0.8f, 0.8f),
                    marginBottom = 15
                }
            };
            warningContainer.Add(warningTitle);

            var warningMessage = new Label("Please assign a SkillTreeController to view and edit the skill tree.\n\nYou can:\n• Select one from the scene using the dropdown above\n• Open the scene containing your skill tree")
            {
                style = 
                {
                    fontSize = 14,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    color = new Color(0.9f, 0.9f, 0.9f),
                    whiteSpace = WhiteSpace.Normal,
                    marginBottom = 20
                }
            };
            warningContainer.Add(warningMessage);

            var findButton = new Button(() => 
            {
                var controller = FindAnyObjectByType<SkillTreeController>();
                if (controller != null)
                {
                    currentTreeController = controller;
                    treeControllerField.value = controller;
                    LoadTree();
                }
                else
                {
                    EditorUtility.DisplayDialog("Not Found", 
                        "No SkillTreeController found in the current scene.\n\nPlease open the scene containing your skill tree.", 
                        "OK");
                }
            })
            {
                text = "🔍 Find Controller in Scene",
                style = 
                {
                    height = 40,
                    fontSize = 14,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    backgroundColor = new Color(0.3f, 0.5f, 0.8f),
                    color = Color.white
                }
            };
            warningContainer.Add(findButton);

            noControllerWarning.Add(warningContainer);
        }

        private void CreatePrefabModeWarning()
        {
            prefabModeWarning = new VisualElement
            {
                style = 
                {
                    position = Position.Absolute,
                    left = 0,
                    right = 0,
                    top = 0,
                    bottom = 0,
                    backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.95f),
                    alignItems = Align.Center,
                    justifyContent = Justify.Center,
                    display = DisplayStyle.None // Hidden by default
                }
            };

            var warningContainer = new VisualElement
            {
                style = 
                {
                    backgroundColor = new Color(0.2f, 0.3f, 0.4f, 0.95f),
                    borderLeftWidth = 4,
                    borderRightWidth = 4,
                    borderTopWidth = 4,
                    borderBottomWidth = 4,
                    borderLeftColor = new Color(0.3f, 0.5f, 0.9f),
                    borderRightColor = new Color(0.3f, 0.5f, 0.9f),
                    borderTopColor = new Color(0.3f, 0.5f, 0.9f),
                    borderBottomColor = new Color(0.3f, 0.5f, 0.9f),
                    borderTopLeftRadius = 10,
                    borderTopRightRadius = 10,
                    borderBottomLeftRadius = 10,
                    borderBottomRightRadius = 10,
                    paddingLeft = 40,
                    paddingRight = 40,
                    paddingTop = 40,
                    paddingBottom = 40,
                    maxWidth = 600
                }
            };

            var warningIcon = new Label("📦")
            {
                style = 
                {
                    fontSize = 80,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    marginBottom = 20
                }
            };
            warningContainer.Add(warningIcon);

            var warningTitle = new Label("Prefab Mode - Please Open Prefab for Editing")
            {
                style = 
                {
                    fontSize = 24,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    color = new Color(0.7f, 0.9f, 1f),
                    marginBottom = 15
                }
            };
            warningContainer.Add(warningTitle);

            var warningMessage = new Label("You've selected a SkillTreeController prefab, but it's not currently open in Prefab Mode.\n\nTo create or edit nodes in this prefab, you need to open it for editing.\n\nClick the button below to automatically open the prefab in Prefab Mode.")
            {
                style = 
                {
                    fontSize = 14,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    color = new Color(0.9f, 0.9f, 0.9f),
                    whiteSpace = WhiteSpace.Normal,
                    marginBottom = 20
                }
            };
            warningContainer.Add(warningMessage);

            var openPrefabButton = new Button(() => 
            {
                if (currentTreeController != null)
                {
                    var assetPath = AssetDatabase.GetAssetPath(currentTreeController.gameObject);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        // Open the prefab in prefab stage
                        var openedStage = UnityEditor.SceneManagement.PrefabStageUtility.OpenPrefab(assetPath);
                        if (openedStage != null)
                        {
                            // Find the controller in the opened prefab
                            var controllerInStage = openedStage.prefabContentsRoot.GetComponent<SkillTreeController>();
                            if (controllerInStage == null)
                            {
                                controllerInStage = openedStage.prefabContentsRoot.GetComponentInChildren<SkillTreeController>();
                            }

                            if (controllerInStage != null)
                            {
                                // Update the reference to the controller in prefab stage
                                currentTreeController = controllerInStage;
                                treeControllerField.value = controllerInStage;

                                // Update mode detection
                                UpdateModeDetection();

                                // Reload the tree
                                LoadTree();

                                Debug.Log($"[Prefab Mode] Opened prefab and updated controller reference");
                            }
                        }
                    }
                }
            })
            {
                text = "📦 Open Prefab for Editing",
                style = 
                {
                    height = 50,
                    fontSize = 16,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    backgroundColor = new Color(0.2f, 0.5f, 0.9f),
                    color = Color.white,
                    marginBottom = 10
                }
            };
            warningContainer.Add(openPrefabButton);

            var infoLabel = new Label("💡 Tip: You can also double-click the prefab in the Project window to open it.")
            {
                style = 
                {
                    fontSize = 11,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    color = new Color(0.6f, 0.7f, 0.8f),
                    unityFontStyleAndWeight = FontStyle.Italic,
                    whiteSpace = WhiteSpace.Normal
                }
            };
            warningContainer.Add(infoLabel);

            prefabModeWarning.Add(warningContainer);
        }

        private void RefreshInspector()
        {
            if (inspectorContainer == null)
                return;

            // Clear existing content (keep header)
            var children = inspectorContainer.Children().ToList();
            for (int i = 1; i < children.Count; i++) // Skip header at index 0
            {
                inspectorContainer.Remove(children[i]);
            }

            if (selectedSkillData == null)
            {
                var noSelectionLabel = new Label("Select a skill node to edit its properties")
                {
                    style = 
                    {
                        fontSize = 10,
                        color = new Color(0.6f, 0.6f, 0.6f),
                        unityFontStyleAndWeight = FontStyle.Italic,
                        whiteSpace = WhiteSpace.Normal,
                        marginTop = 5,
                        marginBottom = 10
                    }
                };
                inspectorContainer.Add(noSelectionLabel);
                return;
            }

            // Create horizontal button container
            var buttonContainer = new VisualElement
            {
                style = 
                {
                    flexDirection = FlexDirection.Row,
                    marginBottom = 8,
                    justifyContent = Justify.FlexStart
                }
            };

            // Add Update Node button (icon only)
            var updateButton = new Button(() => UpdateSelectedNode())
            {
                text = "🔄",
                tooltip = "🔄 Update",
                style = 
                {
                    width = 32,
                    height = 32,
                    fontSize = 16,
                    backgroundColor = new Color(0.3f, 0.6f, 0.9f, 0.8f),
                    borderTopLeftRadius = 4,
                    borderBottomLeftRadius = 4,
                    marginLeft = 0,
                    marginRight = 0,
                    borderRightWidth = 1,
                    borderRightColor = new Color(0.1f, 0.1f, 0.1f, 0.3f)
                }
            };
            buttonContainer.Add(updateButton);

            // Add Rename Node & Data button (icon only)
            var renameButton = new Button(() => RenameNodeAndData())
            {
                text = "✏️",
                tooltip = "✏️ Rename",
                style = 
                {
                    width = 32,
                    height = 32,
                    fontSize = 16,
                    backgroundColor = new Color(0.9f, 0.6f, 0.3f, 0.8f),
                    marginLeft = 0,
                    marginRight = 0,
                    borderRightWidth = 1,
                    borderRightColor = new Color(0.1f, 0.1f, 0.1f, 0.3f)
                }
            };
            buttonContainer.Add(renameButton);

            // Add Rename ID button (icon only)
            var renameIdButton = new Button(() => RenameID())
            {
                text = "🔑",
                tooltip = "🔑 ID",
                style = 
                {
                    width = 32,
                    height = 32,
                    fontSize = 16,
                    backgroundColor = new Color(0.8f, 0.3f, 0.6f, 0.8f),
                    marginLeft = 0,
                    marginRight = 0,
                    borderRightWidth = 1,
                    borderRightColor = new Color(0.1f, 0.1f, 0.1f, 0.3f)
                }
            };
            buttonContainer.Add(renameIdButton);

            // Add Randomize Sprite button (icon only)
            var randomizeSpriteButton = new Button(() => RandomizeSelectedSprite())
            {
                text = "🎲",
                tooltip = "🎲 Randomize Sprite",
                style = 
                {
                    width = 32,
                    height = 32,
                    fontSize = 16,
                    backgroundColor = new Color(0.8f, 0.7f, 0.2f, 0.8f),
                    borderTopRightRadius = 4,
                    borderBottomRightRadius = 4,
                    marginLeft = 0,
                    marginRight = 0
                }
            };
            buttonContainer.Add(randomizeSpriteButton);

            inspectorContainer.Add(buttonContainer);

            // Create inspector using InspectorElement (Unity's built-in inspector)
            var inspector = new InspectorElement(selectedSkillData);
            inspector.style.marginTop = 5;
            inspectorContainer.Add(inspector);
        }

        private void UpdateSelectedNode()
        {
            if (selectedSkillData == null)
            {
                Debug.LogWarning("No skill selected to update");
                return;
            }

            if (graphView == null)
            {
                Debug.LogError("GraphView is null");
                return;
            }

            // Mark the asset as dirty to save changes
            EditorUtility.SetDirty(selectedSkillData);
            AssetDatabase.SaveAssets();

            // Update the node visuals in the graph
            graphView.UpdateNodeVisuals(selectedSkillData);

            Debug.Log($"Updated node visuals for '{selectedSkillData.GetTitle()}'");
        }

        private void RenameNodeAndData()
        {
            if (selectedSkillData == null)
            {
                Debug.LogWarning("No skill selected to rename");
                return;
            }

            if (currentTreeController == null)
            {
                Debug.LogWarning("No SkillTreeController assigned!");
                return;
            }

            // Generate new name based on pattern
            string newName = GenerateNameFromPattern(selectedSkillData);
            
            // Make sure name is unique
            newName = MakeNameUnique(newName);

            // Rename the SkillData asset
            string assetPath = AssetDatabase.GetAssetPath(selectedSkillData);
            if (!string.IsNullOrEmpty(assetPath))
            {

                // Rename asset file
                string error = AssetDatabase.RenameAsset(assetPath, newName);
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"Failed to rename asset: {error}");
                    return;
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"Renamed SkillData asset to: {newName}");
            }

            // Find and rename the node GameObject
            if (currentTreeController.AllNodeList != null)
            {
                foreach (var node in currentTreeController.AllNodeList)
                {
                    if (node != null && node.Data == selectedSkillData)
                    {
                        node.gameObject.name = newName;
                        EditorUtility.SetDirty(node.gameObject);
                        
                        // Mark prefab dirty if in prefab mode
                        if (isPrefabMode)
                        {
                            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                            if (prefabStage != null)
                            {
                                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(prefabStage.scene);
                            }
                        }
                        
                        Debug.Log($"Renamed node GameObject to: {newName}");
                        break;
                    }
                }
            }

            // Update the graph view
            if (graphView != null)
            {
                graphView.UpdateNodeVisuals(selectedSkillData);
            }

            EditorUtility.DisplayDialog("Success", $"Renamed to: {newName}", "OK");
        }

        private void RenameID()
        {
            if (selectedSkillData == null)
            {
                Debug.LogWarning("No skill selected to rename ID");
                return;
            }

            // Generate new ID from current name
            string newID = selectedSkillData.name.Replace(" ", "_").ToLower();

            // Show confirmation dialog
            bool confirmed = EditorUtility.DisplayDialog(
                "Rename ID Confirmation",
                $"Are you sure you want to change the ID?\n\n" +
                $"Current ID: {selectedSkillData.GetID()}\n" +
                $"New ID: {newID}\n\n" +
                $"⚠️ Warning: Changing the ID may break references in save data and other systems!",
                "Yes, Change ID",
                "Cancel"
            );

            if (!confirmed)
            {
                Debug.Log("ID rename cancelled by user");
                return;
            }

            // Use reflection to set the private id field
            var idField = typeof(SkillData).GetField("id", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (idField != null)
            {
                string oldID = selectedSkillData.GetID();
                idField.SetValue(selectedSkillData, newID);
                
                // Mark as dirty and save
                EditorUtility.SetDirty(selectedSkillData);
                AssetDatabase.SaveAssets();
                
                Debug.Log($"ID changed from '{oldID}' to '{newID}'");
                EditorUtility.DisplayDialog("Success", $"ID successfully changed!\n\nOld ID: {oldID}\nNew ID: {newID}", "OK");
            }
            else
            {
                Debug.LogError("Failed to find 'id' field in SkillData");
                EditorUtility.DisplayDialog("Error", "Failed to change ID - field not found", "OK");
            }
        }

        private void RandomizeSelectedSprite()
        {
            if (selectedSkillData == null)
            {
                Debug.LogWarning("No skill selected to randomize sprite");
                return;
            }

            if (EditorUtility.DisplayDialog(
                "Randomize Sprite",
                $"Are you sure you want to randomize the sprite for '{selectedSkillData.GetTitle()}'?",
                "Yes", "Cancel"))
            {
                selectedSkillData.RandomizeSprite();
                
                // Update the graph view to show the new sprite
                if (graphView != null)
                {
                    graphView.UpdateNodeVisuals(selectedSkillData);
                }
                
                // Refresh inspector to show new sprite
                RefreshInspector();
            }
        }

        private string GenerateNameFromPattern(SkillData skillData)
        {
            string pattern = namingPattern;
            
            // Get index (current node count)
            int index = 0;
            if (currentTreeController != null && currentTreeController.AllNodeList != null)
            {
                index = currentTreeController.AllNodeList.Count;
            }

            // Get title
            string skillTitle = skillData.GetTitle();
            if (string.IsNullOrEmpty(skillTitle))
            {
                skillTitle = "Untitled";
            }

            // Clean title for file naming (remove special characters)
            skillTitle = System.Text.RegularExpressions.Regex.Replace(skillTitle, @"[^a-zA-Z0-9_]", "");

            // Replace placeholders
            string result = pattern;
            result = result.Replace("{index}", index.ToString());
            result = result.Replace("{title}", skillTitle);

            // Clean up any remaining invalid characters
            result = System.Text.RegularExpressions.Regex.Replace(result, @"[^a-zA-Z0-9_\-\s]", "");
            result = result.Trim();

            // If result is empty, use a default name
            if (string.IsNullOrEmpty(result))
            {
                result = $"Skill_{index}";
            }

            return result;
        }

        private string MakeNameUnique(string baseName)
        {
            // Get all existing SkillData names
            var allSkillData = SkillTreeSearchProvider.FindAllSkillData();
            var existingNames = new HashSet<string>();
            
            foreach (var skill in allSkillData)
            {
                if (skill != null && skill != selectedSkillData)
                {
                    existingNames.Add(skill.name);
                }
            }

            // Check if base name is unique
            if (!existingNames.Contains(baseName))
            {
                return baseName;
            }

            // Use Unity's naming convention (Name, Name 0, Name 1, etc.)
            // Remove any existing number suffix
            string cleanName = System.Text.RegularExpressions.Regex.Replace(baseName, @" \d+$", "");
            
            // Try adding numbers starting from 0
            int counter = 0;
            string uniqueName = $"{cleanName} {counter}";
            
            while (existingNames.Contains(uniqueName))
            {
                counter++;
                uniqueName = $"{cleanName} {counter}";
            }

            return uniqueName;
        }

        private void LoadTree()
        {
            if (currentTreeController == null)
            {
                Debug.LogWarning("No SkillTreeController assigned!");
                
                // Show warning and hide graph
                if (noControllerWarning != null)
                    noControllerWarning.style.display = DisplayStyle.Flex;
                if (graphView != null)
                    graphView.style.display = DisplayStyle.None;
                    
                return;
            }

            // Hide warning and show graph
            if (noControllerWarning != null)
                noControllerWarning.style.display = DisplayStyle.None;
            if (graphView != null)
                graphView.style.display = DisplayStyle.Flex;

            if (graphView != null)
            {
                graphView.ClearGraph();
                // Automatically load from scene positions
                graphView.GetPositionsFromScene(currentTreeController);
            }
        }

        private void SaveTree()
        {
            if (currentTreeController == null)
            {
                Debug.LogWarning("No SkillTreeController assigned!");
                return;
            }

            // Only save connections, not positions
            graphView.SaveConnectionsOnly(currentTreeController);
            
            // Mark prefab as dirty if in prefab mode
            if (isPrefabMode)
            {
                var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(prefabStage.scene);
                }
                EditorUtility.SetDirty(currentTreeController);
            }
            
            AssetDatabase.SaveAssets();
            var modeText = isPrefabMode ? "prefab" : "scene";
            Debug.Log($"Skill tree connections saved successfully for {modeText}!");
        }

        private void SetPositionsToScene()
        {
            if (currentTreeController == null)
            {
                EditorUtility.DisplayDialog("Error", "No SkillTreeController assigned!", "OK");
                return;
            }

            if (graphView != null)
            {
                graphView.SetPositionsToScene(currentTreeController);
                
                // Mark prefab as dirty if in prefab mode
                if (isPrefabMode)
                {
                    var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                    if (prefabStage != null)
                    {
                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(prefabStage.scene);
                    }
                    EditorUtility.SetDirty(currentTreeController);
                }
                
                var modeText = isPrefabMode ? "prefab" : "scene";
                EditorUtility.DisplayDialog("Success", $"Node positions updated in {modeText}!", "OK");
            }
        }

        private void SyncEditorWithScene()
        {
            if (currentTreeController == null)
            {
                EditorUtility.DisplayDialog("Error", "No SkillTreeController assigned!", "OK");
                return;
            }
            
            SetPositionsToScene();
            SaveTree();

            // Reload the tree to reflect changes
            LoadTree();

            string message = $"Sync complete!\n\n";

            EditorUtility.DisplayDialog("Sync Complete", message, "OK");
        }

        private void CreateNewData()
        {
            if (currentTreeController == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a Tree Controller first!", "OK");
                return;
            }

            // Create new SkillData asset
            var newSkillData = ScriptableObject.CreateInstance<SkillData>();
            
            var path = EditorUtility.SaveFilePanelInProject(
                "Create New Skill Data",
                "NewSkillData",
                "asset",
                "Choose where to save the new skill data",
                SkillTreeEditorPaths.DefaultSkillDataFolder
            );

            if (string.IsNullOrEmpty(path))
                return;

            // Extract the name from the path
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            
            // Generate unique ID based on the file name
            string uniqueId = GenerateUniqueIdForNewData(fileName);
            
            // Set the unique ID using reflection
            var idField = typeof(SkillData).GetField("id", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (idField != null)
            {
                idField.SetValue(newSkillData, uniqueId);
            }

            // Initialize with SetEditor after ID is set
            newSkillData.SetEditor();

            // Auto-fill with random data if toggle is enabled
            if (autoFillNewNodes)
            {
                newSkillData.FillRandom(true);
                Debug.Log("Auto-filled new skill with random data");
            }

            AssetDatabase.CreateAsset(newSkillData, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            // Reload the asset to ensure we have a valid reference
            newSkillData = AssetDatabase.LoadAssetAtPath<SkillData>(path);
            
            if (newSkillData == null)
            {
                Debug.LogError($"Failed to load newly created SkillData at path: {path}");
                EditorUtility.DisplayDialog("Error", "Failed to create SkillData asset!", "OK");
                return;
            }
            
            Debug.Log($"Created new SkillData with unique ID: {uniqueId}");

            // Create SkillNode in scene or prefab
            var skillNodePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                SkillTreeEditorPaths.SkillNodeDefaultPrefab
            );

            if (skillNodePrefab != null)
            {
                GameObject newNode = null;
                
                Debug.Log($"[CreateNewData] isPrefabMode: {isPrefabMode}");
                
                // If in prefab mode, instantiate within the prefab
                if (isPrefabMode)
                {
                    // Check if we're in prefab stage (editing prefab)
                    var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                    Debug.Log($"[CreateNewData] Prefab Stage: {(prefabStage != null ? prefabStage.prefabContentsRoot.name : "NULL")}");
                    
                    if (prefabStage != null)
                    {
                        // We're in prefab editing mode - instantiate in prefab stage
                        Debug.Log($"[CreateNewData] Creating node in prefab stage");
                        newNode = PrefabUtility.InstantiatePrefab(skillNodePrefab, prefabStage.scene) as GameObject;
                        Debug.Log($"[CreateNewData] Node created: {(newNode != null ? newNode.name : "NULL")}");
                    }
                    else
                    {
                        // SkillTreeController is a prefab asset but not currently open
                        Debug.Log($"[CreateNewData] Prefab not open, attempting to open it");
                        var controllerPath = AssetDatabase.GetAssetPath(currentTreeController.gameObject);
                        Debug.Log($"[CreateNewData] Controller path: {controllerPath}");
                        
                        if (!string.IsNullOrEmpty(controllerPath))
                        {
                            // Open prefab stage for editing
                            var openedPrefabStage = UnityEditor.SceneManagement.PrefabStageUtility.OpenPrefab(controllerPath);
                            Debug.Log($"[CreateNewData] Opened prefab stage: {(openedPrefabStage != null ? "SUCCESS" : "FAILED")}");

                            if (openedPrefabStage != null)
                            {
                                // Find the controller in the opened prefab
                                var controllerInStage = openedPrefabStage.prefabContentsRoot.GetComponent<SkillTreeController>();
                                if (controllerInStage == null)
                                {
                                    controllerInStage = openedPrefabStage.prefabContentsRoot.GetComponentInChildren<SkillTreeController>();
                                }

                                Debug.Log($"[CreateNewData] Found controller in stage: {(controllerInStage != null ? controllerInStage.name : "NULL")}");

                                if (controllerInStage != null)
                                {
                                    // Update reference to the controller in the prefab stage
                                    currentTreeController = controllerInStage;
                                    treeControllerField.value = controllerInStage;
                                    isPrefabMode = true; // Ensure we're still in prefab mode

                                    Debug.Log($"[CreateNewData] Creating node in newly opened prefab stage");
                                    // Now instantiate in the prefab stage
                                    newNode = PrefabUtility.InstantiatePrefab(skillNodePrefab, openedPrefabStage.scene) as GameObject;
                                    Debug.Log($"[CreateNewData] Node created: {(newNode != null ? newNode.name : "NULL")}");
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Scene mode - instantiate in active scene
                    Debug.Log($"[CreateNewData] Creating node in scene mode");
                    newNode = PrefabUtility.InstantiatePrefab(skillNodePrefab) as GameObject;
                    Debug.Log($"[CreateNewData] Node created: {(newNode != null ? newNode.name : "NULL")}");
                }
                
                if (newNode != null)
                {
                    Debug.Log($"[CreateNewData] Setting up node {newNode.name}");
                    newNode.name = newSkillData.name;
                    newNode.transform.SetParent(currentTreeController.transform);
                    newNode.transform.localPosition = Vector3.zero;
                    Debug.Log($"[CreateNewData] Node parented to: {currentTreeController.name}");

                    // Get SkillNodeBase and SkillNodeEditor components
                    var nodeBase = newNode.GetComponent<SkillNodeBase>();
                    var nodeEditor = newNode.GetComponent<SkillNodeEditor>();
                    
                    if (nodeBase != null)
                    {
                        // First, set the data to the node
                        nodeBase.SetData(newSkillData);
                        EditorUtility.SetDirty(nodeBase);
                        
                        // Then update the node using SkillNodeEditor if available
                        if (nodeEditor != null)
                        {
                            // Call SetEditor on the node to initialize all components properly
                            nodeBase.SetEditor();
                            EditorUtility.SetDirty(nodeEditor);
                        }
                        
                        // Add to controller's list
                        if (!currentTreeController.AllNodeList.Contains(nodeBase))
                        {
                            currentTreeController.AllNodeList.Add(nodeBase);
                            EditorUtility.SetDirty(currentTreeController);
                            Debug.Log($"[CreateNewData] Node added to controller list");
                        }
                    }
                    
                    // Mark prefab as dirty if in prefab mode
                    if (isPrefabMode)
                    {
                        var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                        if (prefabStage != null)
                        {
                            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(prefabStage.scene);
                            Debug.Log($"[CreateNewData] Marked prefab stage as dirty");
                        }
                    }

                    Selection.activeGameObject = newNode;
                    EditorGUIUtility.PingObject(newNode);

                    var modeText = isPrefabMode ? "prefab" : "scene";
                    EditorUtility.DisplayDialog("Success", 
                        $"SkillData and SkillNode created successfully in {modeText}!\nUnique ID: {uniqueId}\n\nPosition the node and click 'Update Positions'.", 
                        "OK");

                    // Refresh the graph view
                    LoadTree();
                }
            }
            else
            {
                Debug.LogError("Could not find SkillNode_Default.prefab!");
                EditorUtility.DisplayDialog("Error", 
                    "SkillData created, but couldn't create scene node. Please create manually.", 
                    "OK");
            }
        }

        private string GenerateUniqueIdForNewData(string baseName)
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

        private void SearchSkills(string query)
        {
            if (graphView == null)
                return;

            var results = SkillTreeSearchProvider.SearchSkills(query);
            graphView.HighlightNodes(results);
        }

        private void ValidateTree()
        {
            if (currentTreeController == null)
            {
                EditorUtility.DisplayDialog("Validation", "No tree controller assigned!", "OK");
                return;
            }

            var circularDeps = SkillTreeSearchProvider.FindCircularDependencies();
            
            if (circularDeps.Count > 0)
            {
                var message = "Found circular dependencies in:\n";
                foreach (var skill in circularDeps)
                {
                    message += $"- {skill.GetTitle()} ({skill.name})\n";
                }
                EditorUtility.DisplayDialog("Validation Failed", message, "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Validation", "No circular dependencies found! Tree is valid.", "OK");
            }
        }

        private void ShowOrphanedSkills()
        {
            if (currentTreeController == null)
            {
                EditorUtility.DisplayDialog("Info", "No tree controller assigned!", "OK");
                return;
            }

            var orphans = SkillTreeSearchProvider.FindOrphanedSkills(currentTreeController);
            
            if (orphans.Count > 0)
            {
                var message = $"Found {orphans.Count} orphaned skills (not in tree):\n\n";
                foreach (var skill in orphans.Take(10))
                {
                    message += $"- {skill.GetTitle()} ({skill.name})\n";
                }
                if (orphans.Count > 10)
                {
                    message += $"\n... and {orphans.Count - 10} more";
                }
                EditorUtility.DisplayDialog("Orphaned Skills", message, "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Info", "No orphaned skills found!", "OK");
            }
        }

        private void FocusAll()
        {
            if (graphView != null)
            {
                graphView.FrameAll();
            }
        }

        private void GetPositionsFromScene()
        {
            Debug.Log("GetPositionsFromScene button clicked!");
            
            if (currentTreeController == null)
            {
                Debug.LogError("No SkillTreeController assigned!");
                EditorUtility.DisplayDialog("Error", "No SkillTreeController assigned!", "OK");
                return;
            }

            Debug.Log($"Current tree controller: {currentTreeController.name}");

            if (graphView != null)
            {
                Debug.Log("Calling graphView.GetPositionsFromScene...");
                graphView.GetPositionsFromScene(currentTreeController);
                EditorUtility.DisplayDialog("Success", "Node positions updated from scene!", "OK");
            }
            else
            {
                Debug.LogError("GraphView is null!");
            }
        }

        private void CreateNewSkillNode()
        {
            if (currentTreeController == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a Tree Controller first!", "OK");
                return;
            }

            // Create new SkillData asset
            var newSkillData = ScriptableObject.CreateInstance<SkillData>();
            newSkillData.SetEditor();
            
            var path = EditorUtility.SaveFilePanelInProject(
                "Create New Skill",
                "NewSkill",
                "asset",
                "Choose where to save the new skill data",
                SkillTreeEditorPaths.DefaultSkillDataFolder
            );

            if (string.IsNullOrEmpty(path))
                return;

            AssetDatabase.CreateAsset(newSkillData, path);
            AssetDatabase.SaveAssets();

            // Add to tree (optional - could spawn in scene)
            EditorUtility.DisplayDialog("Success", 
                "Skill created! You can now add it to your scene manually or drag it into the tree.", 
                "OK");
            
            Selection.activeObject = newSkillData;
            EditorGUIUtility.PingObject(newSkillData);
        }
    }
}

