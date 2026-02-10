﻿using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace __Project.Systems.NUpgradeSystem._SkillTree.Editor
{
    [CustomEditor(typeof(SkillData))]
    public class SkillDataEditor : OdinEditor
    {
        private bool _showVisualEditor = true;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            return;
            EditorGUILayout.Space(10);
            
            _showVisualEditor = EditorGUILayout.BeginFoldoutHeaderGroup(_showVisualEditor, "Visual Editor");
            
            if (_showVisualEditor)
            {
                EditorGUILayout.Space(5);
                var skillData = target as SkillData;

            if (GUILayout.Button("Open in Skill Tree Editor", GUILayout.Height(30)))
            {
                var window = EditorWindow.GetWindow<SkillTreeGraphWindow>();
                window.titleContent = new GUIContent("Skill Tree Editor");
                window.Show();
                
                // Try to focus on this skill
                if (skillData != null)
                {
                    skillData.ShowNode();
                }
            }

            EditorGUILayout.Space(5);

            if (skillData != null)
            {
                // Show reveal conditions section
                EditorGUILayout.LabelField("Reveal Conditions:", EditorStyles.boldLabel);
                
                // Show stat requirements
                var statReqs = skillData.GetRevealStatRequirements();
                if (statReqs != null && statReqs.Count > 0)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Stat Requirements:", EditorStyles.boldLabel);
                    foreach (var statReq in statReqs)
                    {
                        var isSatisfied = statReq.IsSatisfied();
                        var color = isSatisfied ? Color.green : Color.red;
                        var prevColor = GUI.color;
                        GUI.color = color;
                        EditorGUILayout.LabelField($"  • {statReq.GetDescription()}", EditorStyles.miniLabel);
                        GUI.color = prevColor;
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space(3);
                }
                
                // Show requirement info
                EditorGUILayout.LabelField("Skill Requirements:", EditorStyles.boldLabel);
                var reqList = skillData.GetRequiredSkillList();
                if (reqList != null && reqList.Count > 0)
                {
                    EditorGUI.indentLevel++;
                    foreach (var req in reqList)
                    {
                        if (req != null)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField($"• {req.GetTitle()}", GUILayout.Width(150));
                            if (GUILayout.Button("Select", GUILayout.Width(60)))
                            {
                                Selection.activeObject = req;
                                EditorGUIUtility.PingObject(req);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUI.indentLevel--;
                }
                else
                {
                    EditorGUILayout.LabelField("  No skill requirements", EditorStyles.miniLabel);
                }

                EditorGUILayout.Space(5);

                // Show skills that require this
                EditorGUILayout.LabelField("Required By:", EditorStyles.boldLabel);
                var allSkills = FindAllSkillData();
                var dependents = allSkills.FindAll(s => s.GetRequiredSkillList() != null && s.GetRequiredSkillList().Contains(skillData));
                
                if (dependents.Count > 0)
                {
                    EditorGUI.indentLevel++;
                    foreach (var dependent in dependents)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"• {dependent.GetTitle()}", GUILayout.Width(150));
                        if (GUILayout.Button("Select", GUILayout.Width(60)))
                        {
                            Selection.activeObject = dependent;
                            EditorGUIUtility.PingObject(dependent);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                }
                else
                {
                    EditorGUILayout.LabelField("  Not required by any skill", EditorStyles.miniLabel);
                }
            }
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private System.Collections.Generic.List<SkillData> FindAllSkillData()
        {
            var guids = AssetDatabase.FindAssets("t:SkillData");
            var result = new System.Collections.Generic.List<SkillData>();
            
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var skill = AssetDatabase.LoadAssetAtPath<SkillData>(path);
                if (skill != null)
                {
                    result.Add(skill);
                }
            }
            
            return result;
        }
    }
}

