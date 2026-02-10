using _NueCore.Common.ReactiveUtils;

namespace __Project.Systems.ChoiceSystem
{
    public abstract class ChoiceREvents
    {
        public class ChoiceShownREvent : REvent
        {
            
            public ChoiceShownREvent()
            {
            }
        }
        public class ChoiceChosenREvent : REvent
        {
            public IChoiceItem ChoiceData { get; }
            public bool IgnoreLevelUp { get; private set; }

            public ChoiceChosenREvent(IChoiceItem choiceData,bool ignoreLevelUp = false)
            {
                ChoiceData = choiceData;
                IgnoreLevelUp = ignoreLevelUp;
            }
        }

    }
}