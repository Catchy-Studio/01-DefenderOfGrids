using _NueCore.ManagerSystem.Core;

namespace _NueExtras.MainMenuSystem
{
    public class MainMenuManager : NManagerBase
    {
        #region Setup
        public override void NAwake()
        {
            Instance = InitSingleton<MainMenuManager>();
            base.NAwake();
        }

        public override void NStart()
        {
            base.NStart();
        }

        #endregion

        #region Cache
        public static MainMenuManager Instance { get; private set; }
        #endregion
    }
}