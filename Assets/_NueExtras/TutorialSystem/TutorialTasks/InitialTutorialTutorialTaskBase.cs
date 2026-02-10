namespace _NueExtras.TutorialSystem.TutorialTasks
{
    public class InitialTutorialTutorialTaskBase : TutorialTaskBase
    {
        private bool _isShown = true;
        private bool _isCompleted = true;
        protected override void ExecuteShowActions()
        {
            //HubStatic.SetSideButtonsStatus(false);
        }

        protected override void ExecuteCompleteActions()
        {
            
        }

        protected override bool GetShowConditions()
        {
            return _isShown;
        }

        protected override bool GetCompleteConditions()
        {
            return _isCompleted;
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