using UnityEngine.EventSystems;

namespace NueGames.NTooltip
{
    public class TooltipTrigger_UI : TooltipTriggerBase,ITooltipTarget_UI
    {


        public void OnPointerEnter(PointerEventData eventData)
        {
            //gameObject.name.NLog(Color.yellow);
            ShowTooltipInfo();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HideTooltipInfo();
        }
    }
}