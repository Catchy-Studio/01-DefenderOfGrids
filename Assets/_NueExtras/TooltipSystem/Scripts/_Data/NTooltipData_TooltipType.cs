using UnityEngine;

namespace NueGames.NTooltip
{
    [CreateAssetMenu(fileName = "NTooltip_Style_X", menuName = "NueGames/NTooltip/TooltipData/Style", order = 0)]
    public class NTooltipData_TooltipType : NTooltipData
    {
        [SerializeField] private NTooltipTypes nTooltipType;
        public override string GetID()
        {
            return nTooltipType.ToString();
        }
    }
}