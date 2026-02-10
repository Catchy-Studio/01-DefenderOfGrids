﻿using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using _NueExtras.StockSystem;

namespace __Project.Systems.NUpgradeSystem._SkillTree.Editor
{
    public class SkillDataNode : Node
    {
        public SkillData SkillData { get; private set; }
        public Port TopInputPort;
        public Port BottomInputPort;
        public Port LeftInputPort;
        public Port RightInputPort;
        public Port TopOutputPort;
        public Port BottomOutputPort;
        public Port LeftOutputPort;
        public Port RightOutputPort;

        public List<Port> AllInputPorts => new List<Port> { TopInputPort, BottomInputPort, LeftInputPort, RightInputPort };
        public List<Port> AllOutputPorts => new List<Port> { TopOutputPort, BottomOutputPort, LeftOutputPort, RightOutputPort };

        private Foldout _tierFoldout;
        private VisualElement _topPortContainer;
        private VisualElement _bottomPortContainer;
        private VisualElement _leftPortContainer;
        private VisualElement _rightPortContainer;

        public SkillDataNode(SkillData skillData)
        {
            SkillData = skillData;
            title = skillData.GetTitle();

            // Create directional ports for requirements (inputs) - ORANGE
            TopInputPort = CreatePort(Orientation.Vertical, Direction.Input, "", true);
            BottomInputPort = CreatePort(Orientation.Vertical, Direction.Input, "", true);
            LeftInputPort = CreatePort(Orientation.Horizontal, Direction.Input, "", true);
            RightInputPort = CreatePort(Orientation.Horizontal, Direction.Input, "", true);

            // Create directional ports for unlocking other skills (outputs) - CYAN
            TopOutputPort = CreatePort(Orientation.Vertical, Direction.Output, "", false);
            BottomOutputPort = CreatePort(Orientation.Vertical, Direction.Output, "", false);
            LeftOutputPort = CreatePort(Orientation.Horizontal, Direction.Output, "", false);
            RightOutputPort = CreatePort(Orientation.Horizontal, Direction.Output, "", false);

            // Add top ports
            _topPortContainer = new VisualElement 
            { 
                name = "top-port-container",
                style = 
                { 
                    flexDirection = FlexDirection.Row, 
                    justifyContent = Justify.Center,
                    position = Position.Absolute,
                    top = -20,
                    left = 0,
                    right = 0
                } 
            };
            _topPortContainer.Add(TopInputPort);
            _topPortContainer.Add(TopOutputPort);
            Add(_topPortContainer);

            // Add left ports
            _leftPortContainer = new VisualElement 
            { 
                name = "left-port-container",
                style = 
                { 
                    flexDirection = FlexDirection.Column, 
                    justifyContent = Justify.Center,
                    position = Position.Absolute,
                    left = -20,
                    top = 0,
                    bottom = 0
                } 
            };
            _leftPortContainer.Add(LeftInputPort);
            _leftPortContainer.Add(LeftOutputPort);
            Add(_leftPortContainer);

            // Add right ports
            _rightPortContainer = new VisualElement 
            { 
                name = "right-port-container",
                style = 
                { 
                    flexDirection = FlexDirection.Column, 
                    justifyContent = Justify.Center,
                    position = Position.Absolute,
                    right = -20,
                    top = 0,
                    bottom = 0
                } 
            };
            _rightPortContainer.Add(RightInputPort);
            _rightPortContainer.Add(RightOutputPort);
            Add(_rightPortContainer);

            // Add bottom ports
            _bottomPortContainer = new VisualElement 
            { 
                name = "bottom-port-container",
                style = 
                { 
                    flexDirection = FlexDirection.Row, 
                    justifyContent = Justify.Center,
                    position = Position.Absolute,
                    bottom = -20,
                    left = 0,
                    right = 0
                } 
            };
            _bottomPortContainer.Add(BottomInputPort);
            _bottomPortContainer.Add(BottomOutputPort);
            Add(_bottomPortContainer);

            // Create content container
            var mainContentContainer = new VisualElement();
            mainContentContainer.style.paddingTop = 15;
            mainContentContainer.style.paddingBottom = 15;
            mainContentContainer.style.paddingLeft = 10;
            mainContentContainer.style.paddingRight = 10;

            // Add icon if available
            if (skillData.GetSprite() != null)
            {
                var icon = new Image
                {
                    sprite = skillData.GetSprite(),
                    name = "node-icon",
                    style = 
                    {
                        width = 64,
                        height = 64,
                        alignSelf = Align.Center,
                        marginBottom = 5
                    }
                };
                mainContentContainer.Add(icon);
            }

            // Add skill info (ID only)
            var infoLabel = new Label($"ID: {skillData.GetID()}")
            {
                style = 
                {
                    fontSize = 10,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    whiteSpace = WhiteSpace.Normal,
                    maxWidth = 250,
                    marginBottom = 5
                }
            };
            mainContentContainer.Add(infoLabel);

            // Add left side overlays (require max skills and fame requirement)
            float leftOverlayTopPosition = 40;
            
            // Add require max skills overlay at top left (absolute positioning)
            if (skillData.RequireMaxSkills)
            {
                var requireMaxOverlay = new VisualElement
                {
                    style =
                    {
                        position = Position.Absolute,
                        left = 4,
                        top = leftOverlayTopPosition,
                        backgroundColor = new Color(0.15f, 0.05f, 0.1f, 0.85f),
                        paddingLeft = 4,
                        paddingRight = 4,
                        paddingTop = 2,
                        paddingBottom = 2,
                        borderBottomLeftRadius = 2,
                        borderBottomRightRadius = 2,
                        borderTopLeftRadius = 2,
                        borderTopRightRadius = 2,
                        borderLeftWidth = 1,
                        borderLeftColor = new Color(1f, 0.5f, 0.7f, 0.6f)
                    }
                };

                var maxRequiredLabel = new Label("⭐ MAX")
                {
                    style =
                    {
                        fontSize = 7,
                        color = new Color(1f, 0.7f, 0.9f),
                        unityTextAlign = TextAnchor.MiddleCenter,
                        marginBottom = 0,
                        whiteSpace = WhiteSpace.NoWrap
                    }
                };
                requireMaxOverlay.Add(maxRequiredLabel);

                Add(requireMaxOverlay);
                leftOverlayTopPosition += 18; // Move down for next overlay
            }
            
            // Add fame requirement overlay at left (stacks below require max if both present)
            // if (skillData.RequiredFame > 0)
            // {
            //     var fameOverlay = new VisualElement
            //     {
            //         style =
            //         {
            //             position = Position.Absolute,
            //             left = 4,
            //             top = leftOverlayTopPosition,
            //             backgroundColor = new Color(0.1f, 0.15f, 0.05f, 0.85f),
            //             paddingLeft = 4,
            //             paddingRight = 4,
            //             paddingTop = 2,
            //             paddingBottom = 2,
            //             borderBottomLeftRadius = 2,
            //             borderBottomRightRadius = 2,
            //             borderTopLeftRadius = 2,
            //             borderTopRightRadius = 2,
            //             borderLeftWidth = 1,
            //             borderLeftColor = new Color(0.7f, 1f, 0.5f, 0.6f)
            //         }
            //     };
            //
            //     // var fameLabel = new Label($"👑 {skillData.RequiredFame}")
            //     // {
            //     //     style =
            //     //     {
            //     //         fontSize = 7,
            //     //         color = new Color(0.67f, 1f, 0.92f),
            //     //         unityTextAlign = TextAnchor.MiddleCenter,
            //     //         marginBottom = 0,
            //     //         whiteSpace = WhiteSpace.NoWrap
            //     //     }
            //     // };
            //     //fameOverlay.Add(fameLabel);
            //
            //     Add(fameOverlay);
            // }
            
            // Add reveal conditions overlay at top right (absolute positioning)
            var revealDepth = skillData.RevealDepth;
            var targetSkillsToReveal = skillData.GetTargetSkillsToRevealList();
            
            if (revealDepth > 0 || (targetSkillsToReveal != null && targetSkillsToReveal.Count > 0))
            {
                var revealOverlay = new VisualElement
                {
                    style =
                    {
                        position = Position.Absolute,
                        right = 4,
                        top = 40,
                        backgroundColor = new Color(0.05f, 0.1f, 0.15f, 0.85f),
                        paddingLeft = 4,
                        paddingRight = 4,
                        paddingTop = 2,
                        paddingBottom = 2,
                        borderBottomLeftRadius = 2,
                        borderBottomRightRadius = 2,
                        borderTopLeftRadius = 2,
                        borderTopRightRadius = 2,
                        borderLeftWidth = 1,
                        borderLeftColor = new Color(0.5f, 0.7f, 1f, 0.6f)
                    }
                };

                // Reveal distance text
                if (revealDepth > 0)
                {
                    var revealDistanceLabel = new Label($"📍 {revealDepth}")
                    {
                        style =
                        {
                            fontSize = 7,
                            color = new Color(0.7f, 0.9f, 1f),
                            unityTextAlign = TextAnchor.MiddleCenter,
                            marginBottom = 0,
                            whiteSpace = WhiteSpace.NoWrap
                        }
                    };
                    revealOverlay.Add(revealDistanceLabel);
                }

                // Target skills to reveal - each on separate line
                if (targetSkillsToReveal != null && targetSkillsToReveal.Count > 0)
                {
                    foreach (var skill in targetSkillsToReveal)
                    {
                        if (skill != null)
                        {
                            var revealTargetLabel = new Label($"→ {skill.GetID()}")
                            {
                                style =
                                {
                                    fontSize = 7,
                                    color = new Color(0.8f, 0.9f, 0.6f),
                                    unityTextAlign = TextAnchor.MiddleCenter,
                                    whiteSpace = WhiteSpace.NoWrap,
                                    marginBottom = 0
                                }
                            };
                            revealOverlay.Add(revealTargetLabel);
                        }
                    }
                }

                Add(revealOverlay);
            }

            // ...existing code...

            // Add button to select asset
            var selectButton = new Button(() => 
            {
                Selection.activeObject = skillData;
                EditorGUIUtility.PingObject(skillData);
            })
            {
                text = "Select Asset",
                style = 
                {
                    marginTop = 2,
                    height = 20
                }
            };
            mainContentContainer.Add(selectButton);

            // Add button to find in scene/prefab
            var findButton = new Button(() => 
            {
                FindAndSelectNode(skillData);
            })
            {
                text = "Find Node",
                style = 
                {
                    marginTop = 2,
                    height = 20
                }
            };
            mainContentContainer.Add(findButton);


            // Add tier information as collapsible foldout
            var tierList = skillData.GetSkillTierList();
            if (tierList != null && tierList.Count > 0)
            {
                var tierFoldout = new Foldout
                {
                    text = $"Tiers ({tierList.Count})",
                    value = false,
                    style =
                    {
                        marginTop = 8,
                        paddingLeft = 5,
                        paddingRight = 5,
                        paddingTop = 3,
                        paddingBottom = 3,
                        backgroundColor = new Color(0.15f, 0.15f, 0.2f, 0.8f),
                        borderBottomLeftRadius = 3,
                        borderBottomRightRadius = 3,
                        borderTopLeftRadius = 3,
                        borderTopRightRadius = 3,
                        borderLeftWidth = 2,
                        borderLeftColor = new Color(0.3f, 0.6f, 1f, 0.8f)
                    }
                };

                // Style the foldout header
                var toggle = tierFoldout.Q<Toggle>();
                if (toggle != null)
                {
                    toggle.style.marginLeft = 0;
                    toggle.style.marginRight = 5;
                }

                for (int i = 0; i < tierList.Count; i++)
                {
                    var tier = tierList[i];
                    
                    var tiersContainer = new VisualElement
                    {
                        style =
                        {
                            marginTop = 5,
                            marginBottom = 5,
                            paddingLeft = 8,
                            paddingRight = 8,
                            paddingTop = 5,
                            paddingBottom = 5,
                            backgroundColor = new Color(0.12f, 0.12f, 0.15f, 0.9f),
                            borderBottomLeftRadius = 3,
                            borderBottomRightRadius = 3,
                            borderTopLeftRadius = 3,
                            borderTopRightRadius = 3,
                            borderLeftWidth = 2,
                            borderLeftColor = new Color(0.2f, 0.5f, 1f, 0.8f)
                        }
                    };

                    // Tier header
                    var tierHeaderLabel = new Label($"Tier {i + 1}")
                    {
                        style =
                        {
                            fontSize = 10,
                            unityFontStyleAndWeight = FontStyle.Bold,
                            color = new Color(0.7f, 0.9f, 1f),
                            marginBottom = 3
                        }
                    };
                    tiersContainer.Add(tierHeaderLabel);

                    // Cost info
                    if (tier.requiredResourceList != null && tier.requiredResourceList.Count > 0)
                    {
                        var costLabel = new Label("Cost:")
                        {
                            style =
                            {
                                fontSize = 9,
                                color = new Color(0.9f, 0.7f, 0.3f),
                                marginLeft = 8,
                                marginBottom = 2
                            }
                        };
                        tiersContainer.Add(costLabel);

                        foreach (var resource in tier.requiredResourceList)
                        {
                            var resourceLabel = new Label($"  • {resource.StockType}: {resource.Amount}")
                            {
                                style =
                                {
                                    fontSize = 8,
                                    color = new Color(0.8f, 0.8f, 0.8f),
                                    marginLeft = 12,
                                    marginBottom = 1
                                }
                            };
                            tiersContainer.Add(resourceLabel);
                        }
                    }

                    // Stats info
                    if (tier.statList != null && tier.statList.GetStatList() != null && tier.statList.GetStatList().Count > 0)
                    {
                        var statsLabel = new Label($"Stats: {tier.statList.GetStatList().Count}")
                        {
                            style =
                            {
                                fontSize = 9,
                                color = new Color(0.4f, 1f, 0.4f),
                                marginLeft = 8,
                                marginBottom = 2,
                                marginTop = tier.requiredResourceList != null && tier.requiredResourceList.Count > 0 ? 3 : 0
                            }
                        };
                        tiersContainer.Add(statsLabel);

                        foreach (var stat in tier.statList.GetStatList())
                        {
                            var statLabel = new Label($"  • {stat.Key}: {stat.GetValue()}")
                            {
                                style =
                                {
                                    fontSize = 8,
                                    color = new Color(0.6f, 0.9f, 0.6f),
                                    marginLeft = 12,
                                    marginBottom = 1
                                }
                            };
                            tiersContainer.Add(statLabel);
                        }
                    }

                    tierFoldout.Add(tiersContainer);
                }

                mainContentContainer.Add(tierFoldout);
            }

            extensionContainer.Add(mainContentContainer);

            // Style the node
            RefreshExpandedState();
            RefreshPorts();
            
            // Set node color based on tier count
            var tierCount = skillData.GetSkillTierList()?.Count ?? 0;
            if (tierCount > 0)
            {
                var color = GetColorForTierCount(tierCount);
                style.backgroundColor = color;
            }
        }

        private Port CreatePort(Orientation orientation, Direction direction, string label, bool isInput)
        {
            var port = InstantiatePort(orientation, direction, Port.Capacity.Multi, typeof(SkillData));
            
            // Add R (Requirements) or U (Unlocks) label
            port.portName = isInput ? "R" : "U";
            
            // Color code: Input (Requirements) = Orange, Output (Unlocks) = Cyan
            var portColor = isInput ? new Color(1f, 0.6f, 0f) : new Color(0f, 0.8f, 1f);
            port.portColor = portColor;
            
            port.style.minWidth = 12;
            port.style.minHeight = 12;
            port.style.paddingLeft = 0;
            port.style.paddingRight = 0;
            
            // Add tooltip
            port.tooltip = isInput ? "Requirements: Connect from skills needed to unlock this" : "Unlocks: Connect to skills this unlocks";
            
            return port;
        }

        private void FindAndSelectNode(SkillData skillData)
        {
            // First try to find in prefab stage if open
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                // Search in prefab stage
                var skillTreeController = prefabStage.prefabContentsRoot.GetComponentInChildren<SkillTreeController>();
                if (skillTreeController != null && TrySelectNodeInController(skillTreeController, skillData))
                {
                    return;
                }
            }
            
            // Try to find in active scene
            var sceneController = Object.FindAnyObjectByType<SkillTreeController>();
            if (sceneController != null)
            {
                TrySelectNodeInController(sceneController, skillData);
            }
        }

        private bool TrySelectNodeInController(SkillTreeController controller, SkillData skillData)
        {
            if (controller == null || controller.AllNodeList == null) return false;
            
            foreach (var node in controller.AllNodeList)
            {
                if (node != null && node.Data == skillData)
                {
                    Selection.activeGameObject = node.gameObject;
                    UnityEditor.SceneView.FrameLastActiveSceneView();
                    EditorGUIUtility.PingObject(node.gameObject);
                    return true;
                }
            }
            return false;
        }

        private void RandomizeSprite(SkillData skillData)
        {
            skillData.RandomizeSprite();
        }

        private void RefreshTierEditor()
        {
            _tierFoldout.Clear();
            
            var tierList = SkillData.GetSkillTierList();
            if (tierList == null || tierList.Count == 0)
            {
                var noTiersLabel = new Label("No tiers - click below to add")
                {
                    style = 
                    {
                        fontSize = 9,
                        color = new Color(0.7f, 0.7f, 0.7f),
                        unityTextAlign = TextAnchor.MiddleCenter,
                        paddingTop = 5,
                        paddingBottom = 5
                    }
                };
                _tierFoldout.Add(noTiersLabel);
                
                // Add first tier button
                var addFirstTierButton = new Button(() => 
                {
                    Undo.RecordObject(SkillData, "Add First Tier");
                    tierList.Add(new SkillTier());
                    EditorUtility.SetDirty(SkillData);
                    RefreshTierEditor();
                    _tierFoldout.text = $"Tiers (1)";
                })
                {
                    text = "+ Add First Tier",
                    style = 
                    {
                        marginTop = 5,
                        height = 25,
                        backgroundColor = new Color(0.2f, 0.6f, 0.2f, 0.7f)
                    }
                };
                _tierFoldout.Add(addFirstTierButton);
                return;
            }

            for (int i = 0; i < tierList.Count; i++)
            {
                var tier = tierList[i];
                var tierIndex = i;
                
                var tierContainer = new VisualElement
                {
                    style = 
                    {
                        backgroundColor = new Color(0.15f, 0.15f, 0.2f, 0.8f),
                        marginTop = 3,
                        marginBottom = 3,
                        paddingLeft = 8,
                        paddingRight = 8,
                        paddingTop = 5,
                        paddingBottom = 5,
                        borderBottomLeftRadius = 4,
                        borderBottomRightRadius = 4,
                        borderTopLeftRadius = 4,
                        borderTopRightRadius = 4,
                        borderLeftWidth = 2,
                        borderLeftColor = new Color(0.3f, 0.5f, 1f, 0.8f)
                    }
                };

                // Header row with title and remove button
                var headerRow = new VisualElement { style = { flexDirection = FlexDirection.Row, justifyContent = Justify.SpaceBetween } };
                
                var tierHeader = new Label($"Tier {i + 1}")
                {
                    style = 
                    {
                        fontSize = 11,
                        unityFontStyleAndWeight = FontStyle.Bold,
                        color = new Color(0.8f, 0.9f, 1f)
                    }
                };
                headerRow.Add(tierHeader);

                // Remove button
                var removeButton = new Button(() => 
                {
                    if (EditorUtility.DisplayDialog("Remove Tier", 
                        $"Are you sure you want to remove Tier {tierIndex + 1}?", 
                        "Remove", "Cancel"))
                    {
                        Undo.RecordObject(SkillData, "Remove Tier");
                        tierList.RemoveAt(tierIndex);
                        EditorUtility.SetDirty(SkillData);
                        RefreshTierEditor();
                        _tierFoldout.text = $"Tiers ({tierList.Count})";
                    }
                })
                {
                    text = "✕",
                    style = 
                    {
                        width = 20,
                        height = 20,
                        backgroundColor = new Color(0.8f, 0.2f, 0.2f, 0.6f),
                        fontSize = 12,
                        unityFontStyleAndWeight = FontStyle.Bold
                    }
                };
                headerRow.Add(removeButton);
                
                tierContainer.Add(headerRow);

                // Cost editing section
                var costFoldout = new Foldout { text = "Cost", value = false };
                
                if (tier.requiredResourceList == null)
                    tier.requiredResourceList = new List<RequiredResource>();

                for (int j = 0; j < tier.requiredResourceList.Count; j++)
                {
                    var resource = tier.requiredResourceList[j];
                    var resIndex = j;
                    
                    var resRow = new VisualElement { style = { flexDirection = FlexDirection.Row, marginTop = 2 } };
                    
                    var typeField = new EnumField("Type", resource.StockType)
                    {
                        style = { flexGrow = 1, fontSize = 9 }
                    };
                    typeField.RegisterValueChangedCallback(evt =>
                    {
                        Undo.RecordObject(SkillData, "Change Cost Type");
                        resource.StockType = (StockTypes)evt.newValue;
                        EditorUtility.SetDirty(SkillData);
                    });
                    resRow.Add(typeField);

                    var amountField = new IntegerField("Amount")
                    {
                        value = resource.Amount,
                        style = { width = 80, fontSize = 9 }
                    };
                    amountField.RegisterValueChangedCallback(evt =>
                    {
                        Undo.RecordObject(SkillData, "Change Cost Amount");
                        resource.Amount = evt.newValue;
                        EditorUtility.SetDirty(SkillData);
                    });
                    resRow.Add(amountField);

                    var removeResButton = new Button(() =>
                    {
                        Undo.RecordObject(SkillData, "Remove Cost");
                        tier.requiredResourceList.RemoveAt(resIndex);
                        EditorUtility.SetDirty(SkillData);
                        RefreshTierEditor();
                    })
                    {
                        text = "✕",
                        style = { width = 20, backgroundColor = new Color(0.8f, 0.2f, 0.2f, 0.5f), fontSize = 10 }
                    };
                    resRow.Add(removeResButton);
                    
                    costFoldout.Add(resRow);
                }

                // Add cost button
                var addCostButton = new Button(() =>
                {
                    Undo.RecordObject(SkillData, "Add Cost");
                    tier.requiredResourceList.Add(new RequiredResource { StockType = StockTypes.Coin, Amount = 100 });
                    EditorUtility.SetDirty(SkillData);
                    RefreshTierEditor();
                })
                {
                    text = "+ Add Cost",
                    style = { height = 18, marginTop = 2, fontSize = 9, backgroundColor = new Color(0.2f, 0.4f, 0.2f, 0.5f) }
                };
                costFoldout.Add(addCostButton);

                tierContainer.Add(costFoldout);

                // Stats info (click to edit in inspector)
                var statsButton = new Button(() => 
                {
                    Selection.activeObject = SkillData;
                    EditorGUIUtility.PingObject(SkillData);
                })
                {
                    text = tier.statList != null && tier.statList.GetStatList() != null && tier.statList.GetStatList().Count > 0
                        ? $"Stats: {tier.statList.GetStatList().Count} (click to edit)"
                        : "Stats: None (click to add)",
                    style = 
                    {
                        height = 20,
                        marginTop = 3,
                        fontSize = 9,
                        backgroundColor = new Color(0.2f, 0.3f, 0.4f, 0.5f),
                        color = new Color(0.7f, 1f, 0.7f)
                    }
                };
                tierContainer.Add(statsButton);

                _tierFoldout.Add(tierContainer);
            }

            // Add tier button at bottom
            var addTierButton = new Button(() => 
            {
                Undo.RecordObject(SkillData, "Add Tier");
                tierList.Add(new SkillTier());
                EditorUtility.SetDirty(SkillData);
                RefreshTierEditor();
                _tierFoldout.text = $"Tiers ({tierList.Count})";
            })
            {
                text = "+ Add Tier",
                style = 
                {
                    marginTop = 5,
                    height = 25,
                    backgroundColor = new Color(0.2f, 0.6f, 0.2f, 0.7f),
                    fontSize = 10,
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            };
            _tierFoldout.Add(addTierButton);
        }

        private Color GetColorForTierCount(int tierCount)
        {
            if (tierCount == 0)
                return new Color(0.3f, 0.3f, 0.3f, 0.8f);
            else if (tierCount == 1)
                return new Color(0.2f, 0.4f, 0.2f, 0.8f);
            else if (tierCount <= 3)
                return new Color(0.2f, 0.3f, 0.5f, 0.8f);
            else if (tierCount <= 5)
                return new Color(0.4f, 0.2f, 0.5f, 0.8f);
            else
                return new Color(0.5f, 0.3f, 0.1f, 0.8f);
        }
    }
}

