using _NueCore.Common.ReactiveUtils;
using UnityEngine;

namespace _NueCore.SettingsSystem
{
    public abstract class SettingsREvents
    {
        public class AbandonButtonClickedREvent : REvent
        {
            public AbandonButtonClickedREvent()
            {
                
            }
        }
        
        public class ReturnToMainMenuButtonClickedREvent : REvent
        {
            
        }
        
        public class SaveAndQuitButtonClickedREvent : REvent
        {
            
        }

        
        public class SettingsClosedREvent : REvent
        {
            public SettingsClosedREvent()
            {
                Time.timeScale = 1;
            }
        }
        
        public class SettingsOpenedREvent : REvent
        {
            public SettingsOpenedREvent()
            {
                Time.timeScale = 0;
            }
        }
        
        public class SettingsChangedREvent : REvent
        {
            public SettingsChangedREvent()
            {
                
            }
        }
    }
}