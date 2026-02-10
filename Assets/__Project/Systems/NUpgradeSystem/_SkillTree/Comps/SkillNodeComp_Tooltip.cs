using System.Linq;
using System.Text;
using _NueCore.Common.Extensions;
using _NueCore.Common.NueLogger;
using _NueCore.Common.Utility;
using _NueCore.NStatSystem;
using _NueCore.SaveSystem;
using _NueExtras.StockSystem;
using NueGames.NTooltip;
using UnityEngine;

namespace __Project.Systems.NUpgradeSystem._SkillTree.Comps
{
    public class SkillNodeComp_Tooltip : SkillNodeCompBase
    {
        [SerializeField] private TooltipTriggerBase tooltip;
        [SerializeField] private Transform follow;
        
        
        protected override void OnBuilt()
        {
            base.OnBuilt();
            Node.OnPurchasedAction += _ =>
            {
                tooltip.Refresh();
            };
            tooltip.AddTooltipInfo(GetTooltip);
        }

        private NTooltipInfo GetTooltip()
        {
            
            var info = new NTooltipInfo
            {
                Source = Node.GetID(),
                NTooltipType = NTooltipTypes.Product,
                FollowTarget = follow,
                Is3D = true,
                Layout = NTooltipLayout.Default
            };

            if (!Node.IsUnlocked())
            {
                info.BlockTooltip = true;
            }
            var tStat = new NStatList();
            
            var tierList =Node.Data.GetSkillTierList();
            var tierComp = Node.GetComp<SkillNodeComp_Tier>();
            if (tierComp == null)
            {
                "No Tier Comp on Tooltip Node".NLog(Color.red,Node.gameObject);
                return info;
            }
            var currentTier = tierComp.GetCurrentTier();
            var stats = currentTier.statList.GetStatList();
            foreach (var stat in stats)
                tStat.AddStat(stat.Key, stat.GetValue(),stat.GetStatCategory());
            
            info.SetStringVariable(NTooltipKeys.Title, Node.Data.GetTitle());
            var dd = Node.Data.GetDesc(tStat);
            var strDesc = new StringBuilder();
            dd =Node.Data.ConvertCurrentNextStats(dd, tStat, tierComp && tierComp.IsMaxed());
            dd = ConvertTotalStats(dd, tStat, tierComp && tierComp.IsMaxed());
            strDesc.Append(dd);
            if (Node.Data.RequireMaxSkills && Node.GetPurchaseCount()<=0)
            {
                strDesc.AppendLine().AppendLine().AppendLine("[REQUIRED ALL PREVIOUS SKILLS MAXED]".Colorize(new Color(0.65f, 1f, 0f)));
            }
            info.SetStringVariable(NTooltipKeys.Description, strDesc.ToString());
            
            //CheckCurrentNext(tStat, info);

            var current = Node.GetSave().purchaseCount;
            var max = tierList.Count;
            info.SetStringVariable("#upgradeCount",$"{current}/{max}");
           
            
            
            SetPriceInfo(currentTier, info,tierComp.IsMaxed());
            return info;
        }

        
      
        private string ConvertTotalStats(string mainText, NStatList statList, bool isMaxed = false)
        {
            if (!mainText.Contains("<t_"))
            {
                return  mainText;
            }
            
            if (StringHelper.IsNull(mainText))
            {
                return mainText;
            }
            
            var str = mainText;
            // Handle <t_StatName> format - find stat name between <t_ and >, convert to NStatEnum and replace
            var index = 0;
            while ((index = str.IndexOf("<t_", index, System.StringComparison.Ordinal)) >= 0)
            {
                var startIndex = index + 3; // Skip "<t_"
                var endIndex = str.IndexOf('>', startIndex);
                
                if (endIndex == -1)
                {
                    // No closing >, skip this one
                    index = startIndex;
                    continue;
                }
                
                var statName = str.Substring(startIndex, endIndex - startIndex);
               
                
                // Try to parse as NStatEnum
                if (System.Enum.TryParse<NStatEnum>(statName, true, out var statEnum) && statEnum != NStatEnum.None)
                {
                    var totalValue = UpgradeStatic.GetTotalStat(statEnum);
                    str = str.Replace($"<t_{statName}>", $"Total: {totalValue:0.0}");
                    // Start from beginning again after replacement since string changed
                    index = 0;
                }
                else
                {
                    // Move past this tag if it's not valid
                    index = endIndex + 1;
                }
            }
            return str;
        }

        private void CheckCurrentNext(NStatList tStat, NTooltipInfo info)
        {
            var currentStr = new StringBuilder();
            var nextStr = new StringBuilder();
            var save = NSaver.GetSaveData<UpgradeSave>();
            foreach (var stat in tStat.GetStatList())
            {
                var saveData =save.GetSkillTreeStatSave(stat.Key);
                var currentValue = saveData.GetTotal();
                currentStr.AppendLine($"{currentValue:0.0}");
                var nextValue = saveData.GetTotal();
                if (stat.GetStatCategory() is NStatCategory.Flat)
                {
                    nextValue += stat.GetValue();
                }
                else if (stat.GetStatCategory() is NStatCategory.Percent)
                {
                    nextValue += saveData.flatValue * stat.GetValue()/100f;
                }
                nextStr.AppendLine($"{nextValue:0.0}".Colorize(!Mathf.Approximately(nextValue, currentValue) ? Color.green : Color.white));
            }
            info.SetStringVariable("#currentValues", currentStr.ToString());
            info.SetStringVariable("#nextValues", nextStr.ToString());
            var tierComp = Node.GetComp<SkillNodeComp_Tier>();
            if (tierComp)
            {
                if (tierComp.IsMaxed())
                {
                    var tr =info.GetTransformVariable("#nextValues");
                    if (tr)
                    {
                        tr.gameObject.SetActive(false);
                    }
                }
            }
           
        }

        private void SetPriceInfo(SkillTier currentTier, NTooltipInfo info, bool isMaxed)
        {

            if (isMaxed)
            {
                info.SetStringVariable("#price", "MAX".Colorize(new Color(1f, 0.18f, 0.11f)).ToString());

                return;
            }
            var reqs = currentTier.requiredResourceList;
            var resourceStr = new StringBuilder();
            
            foreach (var resource in reqs)
            {
                var tColor = StockStatic.GetStock(resource.StockType) >= resource.Amount
                    ? Color.green
                    : Color.red;
                resourceStr.Append($"{resource.Amount.ConvertToDigit()} {resource.StockType.GetSprite()} ".Colorize(tColor));
            }
          

            if (reqs.Count<=0)
            {
                info.SetStringVariable("#price", "Free".Colorize(Color.yellow));
            }
            else
            {
                info.SetStringVariable("#price", resourceStr.ToString());
            }
        }
    }
}