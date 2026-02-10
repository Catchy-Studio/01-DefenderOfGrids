﻿using System.Collections.Generic;
using _NueCore.Common.NueLogger;
using _NueCore.Common.ReactiveUtils;
using UniRx;
using UnityEngine;

namespace __Project.Systems.NUpgradeSystem._SkillTree.Comps
{
    public class SkillNodeComp_Connections : SkillNodeCompBase
    {
        [SerializeField] private SkillLineRenderer linePrefab;
        [SerializeField] private Transform lineSpawnRoot;

        public List<SkillNodeBase> RequiredNodeList { get; private set; } = new List<SkillNodeBase>();
        private List<SkillLineRenderer> _spawnedRendList = new List<SkillLineRenderer>();

        protected override void OnBuilt()
        {
            base.OnBuilt();

            var reqs =Node.Data.GetRequiredSkillList();
            foreach (var data in reqs)
            {
                if (Node.Tree.TryGetNode(data,out var node))
                    RequiredNodeList.Add(node);
                else
                {
                    $"Could not find required node for data: {data.name}".NLog(Color.red);
                }
            }
            CreateConnections();
            Node.IsLockedFunc += () =>
            {
              
                foreach (var node in RequiredNodeList)
                {
                    if (!node.IsUnlocked())
                    {
                        return true;
                    }
                    
                    if (Node.Data.RequireMaxSkills)
                    {
                        if (!node.IsMaxed())
                        {
                            return true;
                        }
                    }
                }
                
                return false;
            };
            Node.OnNodeUpdatedAction += () =>
            {
                foreach (var lineRenderer in _spawnedRendList)
                {
                    if (lineRenderer)
                        lineRenderer.UpdateLineStatus(Node.GetStatus(),Node.Data.RequireMaxSkills);
                }
            };
            
            Node.IsRevealedFunc += () =>
            {
                // Check stat requirements first
                if (!Node.Data.AreRevealStatRequirementsSatisfied())
                    return false;
                
                var reqReveals = Node.Data.GetTargetSkillsToRevealList();
                foreach (var reqReveal in reqReveals)
                {
                    if (Node.Tree.TryGetNode(reqReveal,out var reqNode))
                    {
                        if (reqNode.GetPurchaseCount()<=0)
                            return false;
                    }
                }
                var depth =Node.Data.RevealDepth;
                if (depth <= 0)
                    return true;

                var pDepth = 0;
                var checkingList = new List<SkillNodeBase>(RequiredNodeList);
                while (pDepth<depth)
                {
                    var anyLocked = false;
                    foreach (var reqNode in checkingList)
                    {
                        if (reqNode.GetPurchaseCount()<=0)
                        {
                            anyLocked = true;
                            break;
                        }
                    }

                    if (!anyLocked)
                    {
                        //$"Hidden: {Node.Data.name} at depth {depth} failed at {pDepth}".NLog(Color.yellow);
                        return true;
                    }
                    pDepth++;
                    var nextCheckingList = new List<SkillNodeBase>();
                    foreach (var skillNodeBase in checkingList)
                    {
                        var ct =skillNodeBase.GetComp<SkillNodeComp_Connections>();
                        if (!ct) continue;
                        foreach (var nd in ct.RequiredNodeList)
                        {
                            if (!checkingList.Contains(nd))
                                nextCheckingList.Add(nd);
                        }
                    }
                    //checkingList.Clear();
                    checkingList.AddRange(nextCheckingList);
                }
                
                return false;
            };

            RBuss.OnEvent<SkillTreeREvents.SkillNodePurchasedEvent>().Subscribe(ev =>
            {
                if (!RequiredNodeList.Contains(ev.Node))
                {
                    return;
                }
                if (!Node.IsUnlocked())
                {
                    if (Node.Data.IsAllRequirementsSatisfied())
                        Node.Unlock();
                }
            }).AddTo(gameObject);
        }

        
        
        
        public void CreateConnections()
        {
            foreach (var lineRenderer in _spawnedRendList)
            {
                if (lineRenderer)
                    Destroy(lineRenderer.gameObject);
            }
            _spawnedRendList.Clear();
            foreach (var nodeBase in RequiredNodeList)
            {
                var lineRend = Instantiate(linePrefab, lineSpawnRoot, true);
                //lineRend.transform.localPosition = Vector3.zero;
                lineRend.SetPos(nodeBase.transform.position,Node.transform.position);
                lineRend.UpdateLineStatus(Node.GetStatus(),Node.Data.RequireMaxSkills);
                _spawnedRendList.Add(lineRend);
            }
        }
    }
}