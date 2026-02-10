using UnityEngine;

namespace __Project.Systems.NUpgradeSystem._SkillTree.Comps
{
    public abstract class SkillNodeCompBase : MonoBehaviour
    {
        public SkillNodeBase Node { get; private set; }
        public void Build(SkillNodeBase skillNodeBase)
        {
            Node = skillNodeBase;
            Node.OnNodeUpdatedAction += OnNodeUpdatedAction;
            OnBuilt();
        }

        protected virtual void OnNodeUpdatedAction()
        {
            
        }

        protected virtual void OnBuilt()
        {
            
        }
    }
}