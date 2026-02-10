using _NueCore.Common.ReactiveUtils;

namespace _NueExtras.AchievementSystem
{
    public abstract class AchievementREvents
    {
        public class AchievedREvent : REvent
        {
            public AchievementData Data { get; set; }
            public AchievedREvent(AchievementData data)
            {
                Data = data;
            }
        }
        
        public class PreAchievedREvent : REvent
        {
            public AchievementData Data { get; set; }
            public PreAchievedREvent(AchievementData data)
            {
                Data = data;
            }
        }
        public class AllAchievementsClearedREvent : REvent
        {
            
        }
        
        
        public class OpenREvent : REvent
        {
            
        }
        
        public class CloseREvent : REvent
        {
            
        }
        
        public class PopupOpenedREvent : REvent
        {
        }
        
        public class PopupClosedREvent : REvent
        {
        } 
    }
}