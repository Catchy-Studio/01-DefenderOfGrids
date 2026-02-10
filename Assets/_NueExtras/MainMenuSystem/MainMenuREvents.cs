using _NueCore.Common.ReactiveUtils;

namespace _NueExtras.MainMenuSystem
{
    public abstract class MainMenuREvents
    {
        public class PreNewGameButtonClickedREvent : REvent
        {
            public PreNewGameButtonClickedREvent()
            {
                
            }
        }

        public class NewGameButtonClickedREvent : REvent
        {
            public NewGameButtonClickedREvent()
            {
                
            }
        }
        
        public class ContinueButtonClickedREvent : REvent
        {
            
        }
        
        public class CreditsButtonClickedREvent : REvent
        {
            
        }
        
        public class CollectionButtonClickedREvent : REvent
        {
            
        }
    }
}