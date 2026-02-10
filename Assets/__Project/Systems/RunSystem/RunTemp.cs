using System.Collections.Generic;
using _NueCore.NStatSystem;
using _NueExtras.StockSystem;
using UniRx;

namespace __Project.Systems.RunSystem
{
    public class RunTemp
    {
        public FloatReactiveProperty TimeScale { get; private set; }= new FloatReactiveProperty(1);
        public ReactiveProperty<RunState> RunState { get; private set; }= new ReactiveProperty<RunState>(RunSystem.RunState.Idle);
        public ReactiveProperty<InteractionState> InteractionState { get; private set; }= new ReactiveProperty<InteractionState>(RunSystem.InteractionState.Active);
        public IntReactiveProperty SecondsPassed { get; private set; } = new IntReactiveProperty(0);
        public Dictionary<StockTypes,int> RunStockDict { get; private set; } = new Dictionary<StockTypes, int>();

        public void IncreaseStock(StockTypes type, int amount)
        {
            if (!RunStockDict.ContainsKey(type))
            {
                RunStockDict[type] = 0;
            }
            RunStockDict[type] += amount;
        }
        public int GetStock(StockTypes type)
        {
            if (!RunStockDict.ContainsKey(type))
            {
                return 0;
            }
            return RunStockDict[type];
        }
        public void SetInteraction(InteractionState state)
        {
            InteractionState.Value = state;
        }
        public void SetState(RunState state)
        {
            RunState.Value = state;
        }
        
        #region Stats

        private NStatHandler StatHandler => _statHandler ??= new NStatHandler();
        private NStatHandler _statHandler;
        public NStatHandler GetStatHandler()
        {
            return StatHandler;
        }
        #endregion
       
    }
}