using _NueCore.Common.KeyValueDict;
using _NueCore.Common.NueLogger;
using UnityEngine;

namespace __Project.Systems.NUpgradeSystem._SkillTree.Comps
{
    public class SkillNodeComp_Outline : SkillNodeCompBase
    {
        [SerializeField] private KeyValueDict<NodeStatus,Transform> outlineDict = new KeyValueDict<NodeStatus, Transform>();
        
        protected override void OnNodeUpdatedAction()
        {
            base.OnNodeUpdatedAction();
            CheckOutline();
        }

        private void CheckOutline()
        {
            foreach (var kvp in outlineDict)
            {
                kvp.Value.gameObject.SetActive(kvp.Key == Node.GetStatus());
            }

            var revealed = Node.IsRevealed();
            //$"{revealed} <> {Node.Data.GetID()}".NLog(revealed? Color.yellow:Color.magenta);
            Node.Refs.VisualRoot.gameObject.SetActive(revealed);
            Node.Refs.LineRoot.gameObject.SetActive(revealed);
            
        }

    }
}