using _NueCore.Common.ReactiveUtils;

namespace __Project.Systems.NUpgradeSystem._SkillTree
{
    public abstract class SkillTreeREvents
    {
        public class SkillNodePurchasedEvent : REvent
        {
            public SkillNodeBase Node { get; private set; }
            public SkillNodePurchasedEvent(SkillNodeBase node)
            {
                Node = node;
            }
        }
    }
}