using _NueCore.Common.ReactiveUtils;

namespace _NueExtras.TutorialSystem
{
    public abstract class NTutorialREvents
    {
        public class TutorialStartedREvent : REvent
        {
            
        }
        
        public class TutorialFinishedREvent : REvent
        {
            
        }
        
        public class TaskActivatedREvent : REvent
        {
            public NTutorialTask Task { get; private set; }
            public TaskActivatedREvent(NTutorialTask task)
            {
                Task = task;
            }
        }
        
        public class TaskCompletedREvent : REvent
        {
            public NTutorialTask Task { get; private set; }
            public TaskCompletedREvent(NTutorialTask task)
            {
                Task = task;
            }
        }
    }
}