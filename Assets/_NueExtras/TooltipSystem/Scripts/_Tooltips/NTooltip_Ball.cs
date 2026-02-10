using UnityEngine;

namespace NueGames.NTooltip
{
    public class NTooltip_Ball : NTooltip
    {
        public override void Show(NTooltipInfo nTooltipInfo)
        {
            base.Show(nTooltipInfo);
            var stickerImages =imageFieldList.FindAll(x=> x.Key.Contains("sticker"));
            foreach (var image in stickerImages)
            {
                //TODO match info
                //nTooltipInfo
            }
        }
    }
}