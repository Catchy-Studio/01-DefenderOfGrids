using _NueCore.SettingsSystem;
using _NueExtras.StockSystem;

namespace _NueExtras.TutorialSystem._Tasks
{
    public class TutorialTask_Finish : NTutorialTask
    {
        protected override void OnBuilt()
        {
            
        }

        protected override void OnActivated()
        {
            
            // StockStatic.IncreaseStock(StockTypes.Gem,1);
            // StockStatic.IncreaseStock(StockTypes.Coin,5);
            //
            // PosessionsManager.instance.statInventoryButton.interactable = false;
            // PosessionsManager.instance.relicInventoryButton.interactable = false;
            Complete();
        }

        protected override void OnFinished()
        {
            //SettingsManager.Instance.UIController.EnableInteraction();
        }
    }
}