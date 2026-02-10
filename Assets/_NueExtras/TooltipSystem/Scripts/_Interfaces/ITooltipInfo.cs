using UnityEngine;

namespace NueGames.NTooltip
{
    public interface ITooltipInfo
    {
        public NTooltipInfo GetTooltipInfo(Transform followRoot = null);
    }
}