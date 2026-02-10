using System;
using _NueCore.NStatSystem;
using _NueCore.SaveSystem;

namespace __Project.Systems.NUpgradeSystem._SkillTree.Comps
{
    public class SkillNodeComp_Tier : SkillNodeCompBase
    {
        
        protected override void OnBuilt()
        {
            base.OnBuilt();
            Node.IsMaxedFunc += IsMaxed;
            Node.OnPurchasedAction += OnPurchasedAction;
        }

        public void RecalculateStats()
        {
            var save = NSaver.GetSaveData<UpgradeSave>();
            var nodeSave = Node.GetSave();
            var pCount =nodeSave.purchaseCount;
            for (int i = 0; i < pCount; i++)
            {
                if (TryGetTier(i,out var tier))
                {
                    var stats = tier.statList.GetStatList();
                    foreach (var stat in stats)
                    {
                        var t = save.GetSkillTreeStatSave(stat.Key);
                        if (stat.GetStatCategory() is NStatCategory.Flat)
                            t.flatValue += stat.GetValue();
                        else if (stat.GetStatCategory() is NStatCategory.Percent)
                            t.percentValue += stat.GetValue();
                    }
                }
            }
        }

        private void OnPurchasedAction(int purchaseCount)
        {
            // Apply stats on purchase
            var save = NSaver.GetSaveData<UpgradeSave>();
            
            if (TryGetTier(purchaseCount,out var tier))
            {
                var stats = tier.statList.GetStatList();
                foreach (var stat in stats)
                {
                    var t =save.GetSkillTreeStatSave(stat.Key);
                    if (stat.GetStatCategory() is NStatCategory.Flat)
                        t.flatValue += stat.GetValue();
                    else if (stat.GetStatCategory() is NStatCategory.Percent)
                        t.percentValue += stat.GetValue();
                }
            }
            save.Save();
        }

       
        
        public int GetTotalTierCount()
        {
            var tierList =Node.Data.GetSkillTierList();
            return tierList.Count;
        }

        public SkillTier GetCurrentTier()
        {
            var save = Node.GetSave();
            if (TryGetTier(save.purchaseCount,out var tier))
            {
                return tier;
            }

            if (IsMaxed())
            {
                var tierList =Node.Data.GetSkillTierList();
                if (tierList.Count<=0)
                {
                    return null;
                }
                return tierList[^1];
            }
            return null;
        }
        
        public bool TryGetTier(int target,out SkillTier tier)
        {
            tier = null;
            var tierList =Node.Data.GetSkillTierList();

            if (target >= tierList.Count || target < 0)
            {
                return false;
            }
            tier = tierList[target];
            return tier != null;
        }

        public bool IsMaxed()
        {

            return Node.Data.IsSkillMaxed();
        }
    }
}