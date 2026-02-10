namespace NueGames.NTooltip
{
    public class NTooltipStatic
    {
        public static NTooltipTemp Temp
        {
            get
            {
                if (_temp == null)
                {
                    SetTemp(new NTooltipTemp());
                }
                return _temp;
            }
        }

        private static NTooltipTemp _temp;
        public static void SetTemp(NTooltipTemp temp)
        {
            _temp = temp;
        }
    }
}