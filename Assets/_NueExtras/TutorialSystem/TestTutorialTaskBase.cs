using UnityEngine;

namespace _NueExtras.TutorialSystem
{
    public class TestTutorialTaskBase : TutorialTaskBase
    {
        [SerializeField] private bool showCondition;
        [SerializeField] private bool completeCondition;
        
        protected override void ExecuteShowActions()
        {
            
        }

        protected override void ExecuteCompleteActions()
        {
            
        }

        protected override bool GetShowConditions()
        {
            return showCondition;
        }

        protected override bool GetCompleteConditions()
        {
            return completeCondition;
        }

        protected override void ExecuteForceShowActions(bool allowCameraLanding)
        {
            
        }

        protected override void ExecuteForceCompleteActions()
        {
           
        }

        protected override void BeforeDestroyBehaviours()
        {
            
        }
    }
}