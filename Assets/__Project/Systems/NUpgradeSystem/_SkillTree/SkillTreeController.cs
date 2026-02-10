using System.Collections.Generic;
using System.Linq;
using __Project.Systems.NUpgradeSystem._SkillTree.Comps;
using _NueCore.Common.NueLogger;
using _NueCore.Common.ReactiveUtils;
using _NueCore.Common.Utility;
using _NueCore.SaveSystem;
using _NueExtras.NPanSystem;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace __Project.Systems.NUpgradeSystem._SkillTree
{
    public class SkillTreeController : MonoBehaviour
    {
        [SerializeField,Unity.Collections.ReadOnly] private List<SkillNodeBase> allNodeList = new List<SkillNodeBase>();
        private NPanSystemController _panSystem;
        [SerializeField] private Transform nodeRoot;
        
        public List<SkillNodeBase> AllNodeList => allNodeList;
        #region Setup
        public void Build()
        {
            allNodeList.Clear();
            allNodeList = GetComponentsInChildren<SkillNodeBase>().ToList();
            var save = NSaver.GetSaveData<UpgradeSave>();
            save.ResetSkillTreeStats();
            foreach (var nodeBase in allNodeList)
            {
                nodeBase.Build(this);
                var tierComp =nodeBase.GetComp<SkillNodeComp_Tier>();
                if (tierComp != null)
                    tierComp.RecalculateStats();
                nodeBase.OnPurchasedAction += i =>
                {
                    UpdatePan();
                    FocusOnNode(nodeBase);
                };
            }

            foreach (var nodeBase in allNodeList)
                nodeBase.UpdateNode();
       
            RBuss.OnEvent<SkillTreeREvents.SkillNodePurchasedEvent>().DelayFrame(1).Subscribe(ev =>
            {
                //var node =ev.Node;
                // var connections = node.GetComp<SkillNodeComp_Connections>();
                // if (connections != null)
                // {
                //     foreach (var reqNode in connections.RequiredNodeList)
                //         reqNode.UpdateNode();
                // }
                foreach (var nodeBase in allNodeList)
                    nodeBase.UpdateNode();
            }).AddTo(gameObject);
            // Initialize pan system with skill tree boundaries
            InitializePanSystem();
        }
        #endregion

        #region Pan
        private void InitializePanSystem()
        {
            _panSystem = CameraStatic.MainCamera.GetComponent<NPanSystemController>();

            if (_panSystem == null)
            {
                Debug.LogWarning("SkillTreeController: No pan system assigned");
                return;
            }

            var targetNodeList = allNodeList.FindAll(x => x.IsRevealed()).ToList();
            var boundaryProvider = new SkillTreeBoundaryProvider(targetNodeList);
            _panSystem.Initialize(boundaryProvider);
            FocusOnNode(targetNodeList[0]);
        }
        
        public void UpdatePan()
        {
            var targetNodeList = allNodeList.FindAll(x => x.IsRevealed()).ToList();
            var boundaryProvider = new SkillTreeBoundaryProvider(targetNodeList);
            _panSystem.UpdateBoundary(boundaryProvider);
        }
        public void CenterCamera()
        {
            if (_panSystem != null)
            {
                _panSystem.CenterCamera();
            }
        }
        
        [Button]
        public void FocusOnNode(SkillNodeBase node, float zoom = -1)
        {
            if (_panSystem != null && node != null)
            {
                _panSystem.FocusOn(node.transform.position, zoom);
            }
        }
        #endregion

        #region Methods

        public bool TryGetNode(SkillData data,out SkillNodeBase node)
        { 
            node =allNodeList.FirstOrDefault(n =>
            {
                if (data == null)
                {
                    "No data passed to TryGetNode".NLog(Color.red);
                    return false;
                }

                if (n == null)
                {
                    "No node found in TryGetNode".NLog(Color.yellow);
                    return false;
                }
                return n.Data.GetID() == data.GetID();
            });
            return node != null;
        }


        #endregion


        #region Editor

#if UNITY_EDITOR
        [Button("Set Editor")]
        public void SetEditor()
        {
            allNodeList.Clear();
            allNodeList = nodeRoot.GetComponentsInChildren<SkillNodeBase>().ToList();
        }

        [Button("Fill Nodes From Data")]
        public void FillNodesFromData()
        {
            foreach (var nodeBase in allNodeList)
            {
                var ed =nodeBase.GetComponent<SkillNodeEditor>();
                if (ed)
                {
                    ed.FillValues();
                }
            }
        }

        [SerializeField] private bool draw;
        
        private void OnDrawGizmos()
        {
            if (!draw)
            {
                return;
            }
            foreach (var node in allNodeList)
            {
                if (node.TryGetComponent<SkillNodeRefs>(out var refs))
                {
                    if (refs == null)
                        continue;
                    var data = node.Data;
                    if (data == null)
                    {
                        continue;
                    }
                    
                    Gizmos.color = new Color(0.35f, 0.87f, 1f);
                    foreach (var connectedData in data.GetRequiredSkillList())
                    {
                        var targetNode =allNodeList.FirstOrDefault(n =>
                        {
                            return n.Data == connectedData;
                        });
                    
                        if (targetNode == null)
                            continue;
                        
                        var startPos = refs.transform.position;
                        var endPos = targetNode.transform.position;
                        Gizmos.DrawLine(startPos, endPos);
                        
                        // Draw spheres along the line
                        float sphereRadius = 0.05f;
                        float distance = Vector3.Distance(startPos, endPos);
                        int sphereCount = Mathf.Max(1, Mathf.FloorToInt(distance / (sphereRadius * 2)));
                        for (int i = 0; i <= sphereCount; i++)
                        {
                            float t = i / (float)sphereCount;
                            var mod = (i % 2 == 0) ? -25 : 0;
                            var r = sphereRadius;
                            r += mod * sphereRadius/100f;
                            
                            Vector3 position = Vector3.Lerp(startPos, endPos, t);
                            Gizmos.DrawSphere(position, r);
                        }
                    }
                }
            }
        }
#endif


        #endregion

    }
}