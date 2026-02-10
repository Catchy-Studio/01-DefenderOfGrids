using _NueCore.Common.ReactiveUtils;
using _NueCore.SaveSystem;
using _NueExtras.StockSystem;
using UniRx;

namespace __Project.Systems.NUpgradeSystem._SkillTree.Comps
{
    public class SkillNodeComp_Stock : SkillNodeCompBase
    {
        protected override void OnBuilt()
        {
            base.OnBuilt();
            Node.CanPurchaseFunc += CanPurchaseFunc;
            Node.OnPurchasedAction += OnPurchasedAction;
            RBuss.OnEvent<StockREvents.StockValueChangedREvent>().Subscribe(ev =>
            {
                if (ev.StockType == StockTypes.Coin)
                {
                    Node.UpdateNode();
                }
            }).AddTo(gameObject);
        }

        private void OnPurchasedAction(int purchaseCount)
        {
            var tierComp = Node.GetComp<SkillNodeComp_Tier>();
            var currentTier = tierComp.GetCurrentTier();
            foreach (var resource in currentTier.requiredResourceList)
            {
                StockStatic.DecreaseStock(resource.StockType,resource.Amount);
            }
        }

        private bool CanPurchaseFunc()
        {
            var reqs = Node.Data.GetRequiredSkillList();
            foreach (var data in reqs)
            {
                if (Node.Tree.TryGetNode(data, out var node))
                {
                    if (!node.IsUnlocked())
                        return false;
                    if (node.GetPurchaseCount()<=0)
                        return false;
                    if (Node.Data.RequireMaxSkills)
                    {
                        if (!node.IsMaxed())
                            return false;
                    }
                }
            }
            var tierComp = Node.GetComp<SkillNodeComp_Tier>();
            if (tierComp)
            {
                var currentTier = tierComp.GetCurrentTier();
                foreach (var resource in currentTier.requiredResourceList)
                {
                    var current = StockStatic.GetStock(resource.StockType);

                    if (current < resource.Amount)
                        return false;
                }
                
            }

            return true;
        }
    }
}