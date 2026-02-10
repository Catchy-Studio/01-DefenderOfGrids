using _NueCore.ManagerSystem.Core;

namespace _NueExtras.CursorSystem
{
    public class CursorManager : NManagerBase
    {
        public static CursorManager Instance { get; private set; }
        public override void NAwake()
        {
            Instance = this;
            base.NAwake();
        }
    }
}