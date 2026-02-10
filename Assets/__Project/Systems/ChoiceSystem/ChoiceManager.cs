using _NueCore.ManagerSystem.Core;

namespace __Project.Systems.ChoiceSystem
{
    public class ChoiceManager : NManagerBase
    {
        public static ChoiceManager Instance { get; private set; }
        public override void NAwake()
        {
            Instance = InitSingleton<ChoiceManager>();
            base.NAwake();
        }
    }
}