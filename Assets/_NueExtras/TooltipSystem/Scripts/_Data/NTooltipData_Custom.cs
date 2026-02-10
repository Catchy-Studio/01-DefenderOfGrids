using UnityEngine;

namespace NueGames.NTooltip
{
    [CreateAssetMenu(fileName = "NTooltipData_Custom_X", menuName = "NueGames/NTooltip/TooltipData/Custom", order = 0)]
    public class NTooltipData_Custom : NTooltipData
    {
        [SerializeField] private string id;
        
        public override string GetID()
        {
            return id;
        }
    }
}