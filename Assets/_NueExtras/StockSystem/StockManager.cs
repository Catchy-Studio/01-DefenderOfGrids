using _NueCore.ManagerSystem.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _NueExtras.StockSystem
{
    public class StockManager : NManagerBase
    {
        #region Setup
        public override void NStart()
        {
            base.NStart();
            StockStatic.InitStocks();
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            StockStatic.SaveStocks();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                StockStatic.SaveStocks();
        }
        #endregion
        
        #region Editor
#if UNITY_EDITOR
        [Button,BoxGroup("Editor",CenterLabel = true,ShowLabel = true)]
        private void ChangeStock(StockTypes stockType, float amount)
        {
            if (!Application.isPlaying) return;
            StockStatic.IncreaseStock(stockType,amount);
        }
#endif
        #endregion
    }
}
