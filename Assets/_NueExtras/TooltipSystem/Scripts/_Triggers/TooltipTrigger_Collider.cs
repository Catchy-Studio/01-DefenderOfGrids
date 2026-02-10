using _NueCore.Common.Utility;

namespace NueGames.NTooltip
{
    public class TooltipTrigger_Collider : TooltipTriggerBase, ITooltipTarget_Collider
    {
       
        public override NTooltipInfo GetTooltipStruct()
        {
            var info = base.GetTooltipStruct();
            info.Is3D = true;
            return info;
        }

        public void OnMouseEnter()
        {
            if (UIHelper.IsMouseOverUI())
                return;
            ShowTooltipInfo();
        }

        public void OnMouseExit()
        {
            HideTooltipInfo();
        }


    }
}