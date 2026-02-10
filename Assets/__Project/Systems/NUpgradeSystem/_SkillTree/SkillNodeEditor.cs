using System.Collections.Generic;
using System.Linq;
using _NueCore.Common.NueLogger;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace __Project.Systems.NUpgradeSystem._SkillTree
{
    [RequireComponent(typeof(SkillNodeRefs))]
    public class SkillNodeEditor : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField,BoxGroup("References",Order = 10)] private SkillData data;
        [SerializeField,BoxGroup("References")] private SkillNodeBase node;
        [SerializeField,BoxGroup("References")] private SkillNodeRefs refs;
        [SerializeField,BoxGroup("References"),FolderPath] private string dataFolder;
        [SerializeField,BoxGroup("Settings"),OnValueChanged(nameof(OnReqChanged))] private List<SkillNodeBase> requiredNodes;
        
        [Button,BoxGroup("Settings")]
        public void FillValues()
        {
            if (!node.Data)
            {
                return;
            }
            
            requiredNodes.Clear();
            var reqList = node.Data.GetRequiredSkillList();
            foreach (var reqData in reqList)
            {
                var tree = GetComponentInParent<SkillTreeController>();
                if (tree != null)
                {
                    var tNode =tree.AllNodeList.FirstOrDefault(n =>
                    {
                        if (node.Data == null)
                        {
                            return false;
                        }
                        return node.Data == reqData;
                    });
                    requiredNodes.Add(tNode);
                }
            }

            refs.IconImage.sprite = node.Data.GetSprite();
        }

        [Button,BoxGroup("Settings")]
        public void RenameData()
        {
            if (node.Data == null)
            {
                Debug.LogError("SkillData is not assigned!");
                return;
            }

            string newName = $"Skill_{node.name}";
            string assetPath = AssetDatabase.GetAssetPath(node.Data);
            AssetDatabase.RenameAsset(assetPath, newName);
            AssetDatabase.SaveAssets();
            Debug.Log($"Renamed SkillData asset to {newName}");
        }
        
        
        [Button,BoxGroup("Settings")]
        public void ReplaceRequiredNodes(bool apply)
        {
            if (!apply)
            {
                return;
            }
            if (node.Data == null)
            {
                Debug.LogError("SkillData is not assigned!");
                return;
            }

            node.Data.GetRequiredSkillList().Clear();
            foreach (var reqNode in requiredNodes)
            {
                if (reqNode == null)
                {
                    Debug.LogWarning($"Required node {reqNode?.name} is invalid or has no SkillData assigned. Skipping.");
                    continue;
                }
                node.Data.GetRequiredSkillList().Add(reqNode.Data);
            }

            EditorUtility.SetDirty(node.Data);
            AssetDatabase.SaveAssets();
            Debug.Log($"Replaced required skills for node {node.name}");
        }
        [Button,BoxGroup("Settings")]
        public void SetToData()
        {
            if (refs == null)
            {
                refs = GetComponent<SkillNodeRefs>();
            }
            if (data == null)
            {
                data = node.Data;
            }
            node.SetData(data);
            refs.IconImage.sprite = data.GetSprite();
            EditorUtility.SetDirty(refs);
        }

        [Button,BoxGroup("Settings")]
        public void CreateNewDataFromNode(bool apply)
        {
            if (!apply)
            {
                return;
            }
            if (string.IsNullOrEmpty(dataFolder))
            {
                Debug.LogError("Data folder path is not set!");
                return;
            }

            requiredNodes.Clear();
            var newData = ScriptableObject.CreateInstance<SkillData>();
            newData.SetEditor();
            string assetPath = $"{dataFolder}/NewSkill_{node.name}.asset";
            AssetDatabase.CreateAsset(newData, assetPath);

            node.SetData(newData);
            data = newData;
            EditorUtility.SetDirty(refs);

            AssetDatabase.SaveAssets();
           
            //Selection.activeObject = newData;
            //EditorGUIUtility.PingObject(newData);
            var tree =GetComponentInParent<SkillTreeController>();
            if (tree != null)
            {
                "Tree".NLog();
                tree.SetEditor();
            }

            node.SetEditor();
            Debug.Log($"Created new SkillData asset for node {node.name} at {assetPath}");
        }


        
        private void OnValidate()
        {
            if (node == null)
                node = GetComponent<SkillNodeBase>();
            if (refs == null)
                refs = GetComponent<SkillNodeRefs>();
            if (node)
            {
                data = node.Data;
            }
        }
        
        private void OnReqChanged()
        {
            // var cData = new List<SkillData>();
            // // This method can be used to handle changes in the requiredNodes list if needed.
            // var tData = requiredNodes.Where(x =>
            // {
            //     var rf = x.GetComponent<SkillNodeRefs>();
            //     if (!rf)
            //     {
            //         return false;
            //     }
            //
            //     return rf.Data != null;
            // }).ToList();
            //
            // foreach (var @base in tData)
            // {
            //     cData.Add(@base.GetComponent<SkillNodeRefs>().Data);
            // }
            // var mData =refs.Data.GetRequiredSkillList();
            // foreach (var skillData in mData)
            // {
            //     if (cData.Contains(skillData))
            //     {
            //         //mData.Remove(skillData);
            //         continue;
            //     }
            //     return;
            // }
            //
        }
        

        private void OnDrawGizmosSelected()
        {
            foreach (var nodeBase in requiredNodes)
            {
                if (!nodeBase)
                {
                    continue;
                }

                Vector3 start = transform.position;
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(start, 0.1f);
                Vector3 end = nodeBase.transform.position;
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(end, 0.1f);
                
                Gizmos.color = Color.yellow;
                int segments = 20;
                float zigzagAmplitude = 0.1f;


                for (int i = 0; i < segments; i++)
                {
                    float t1 = i / (float)segments;
                    float t2 = (i + 1) / (float)segments;

                    Vector3 p1 = Vector3.Lerp(start, end, t1);
                    Vector3 p2 = Vector3.Lerp(start, end, t2);

                    Vector3 direction = (end - start).normalized;
                    Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0);

                    float offset1 = (i % 2 == 0 ? 1 : -1) * zigzagAmplitude;
                    float offset2 = ((i + 1) % 2 == 0 ? 1 : -1) * zigzagAmplitude;

                    p1 += perpendicular * offset1;
                    p2 += perpendicular * offset2;

                    Gizmos.DrawLine(p1, p2);
                }
            }
        }
#endif


    }
}