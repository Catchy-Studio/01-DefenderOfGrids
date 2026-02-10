using _NueCore.Common.ReactiveUtils;

namespace _NueExtras.TutorialSystem
{
    public abstract class TutorialREvents
    {
        public class OnTaskTextOpenAndSetREvent : REvent
            {
                public string TaskText;
        
                public OnTaskTextOpenAndSetREvent(string taskText)
                {
                    TaskText = taskText;
                }
            }
        
            public class OnTaskTextCloseREvent : REvent
            {
              
            }
        
            public class OnTaskCompleteREvent : REvent
            {
                public TutorialTaskBase TutorialTaskBase;
        
                public OnTaskCompleteREvent(TutorialTaskBase tutorialTaskBase)
                {
                    TutorialTaskBase = tutorialTaskBase;
                }
            }
        
            public class OnTaskShowREvent : REvent
            {
                public TutorialTaskBase TutorialTaskBase;
        
                public OnTaskShowREvent(TutorialTaskBase tutorialTaskBase)
                {
                    TutorialTaskBase = tutorialTaskBase;
                }
            }
        
            public class OnSetTaskConditionsREvent : REvent
            {
                public TutorialTaskBase TutorialTaskBase;
                public OnSetTaskConditionsREvent(TutorialTaskBase tutorialTaskBase)
                {
                    TutorialTaskBase = tutorialTaskBase;
                }
            }
        
            public class OnTaskCloseUIREvent : REvent
            {
                public TutorialTaskBase TutorialTaskBase;
                public bool SendStatus;
        
                public OnTaskCloseUIREvent(TutorialTaskBase tutorialTaskBase, bool sendStatus)
                {
                    TutorialTaskBase = tutorialTaskBase;
                    SendStatus = sendStatus;
                }
            }
            
            public class OnShowQuestUI : REvent
            {
                public string CheckID { get; set; }
                
                public OnShowQuestUI(string checkID)
                {
                    CheckID = checkID;
                }
            }
        
            public class OnHideQuestUI : REvent
            {
                public int CheckID { get; set; }
                
                public OnHideQuestUI(int checkID)
                {
                    CheckID = checkID;
                }
            }

    }
}