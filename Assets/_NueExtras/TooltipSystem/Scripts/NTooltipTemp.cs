using System;

namespace NueGames.NTooltip
{
    public class NTooltipTemp
    {
        #region Actions
        public Action<bool> OnTooltipStatusChanged { get; set; }

        public Action<NTooltip> TooltipCalled { get; set; }
        #endregion
        public bool IsTooltipsDisabled { get; private set; }
        public void EnableTooltips()
        {
            IsTooltipsDisabled = false;
            OnTooltipStatusChanged?.Invoke(IsTooltipsDisabled);
        }
        public void DisableTooltips()
        {
            IsTooltipsDisabled = true;
            OnTooltipStatusChanged?.Invoke(IsTooltipsDisabled);
        }
    }
}