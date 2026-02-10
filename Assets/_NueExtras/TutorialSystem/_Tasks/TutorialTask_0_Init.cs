namespace _NueExtras.TutorialSystem._Tasks
{
    public class TutorialTask_0_Init : NTutorialTask
    {
        protected override void OnBuilt()
        {
            
        }

        protected override void OnActivated()
        {
            // StockStatic.IncreaseStock(StockTypes.Gem,1);
            // StockStatic.IncreaseStock(StockTypes.Coin,5);
           
            Complete();
        }

        protected override void OnFinished()
        {
            
        }
    }
}