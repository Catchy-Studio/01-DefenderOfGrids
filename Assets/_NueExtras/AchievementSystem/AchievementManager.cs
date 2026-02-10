using _NueCore.ManagerSystem.Core;
using Sirenix.OdinInspector;

namespace _NueExtras.AchievementSystem
{
    public class AchievementManager : NManagerBase
    {
        [Button,HideInEditorMode]
        public void ClearAll(bool apply)
        {
            if (!apply)
            {
                return;
            }
            AchStatic.ClearAllAch();
        }
    }
}