using NueGames.NTooltip;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _NueExtras.AchievementSystem
{
    [CreateAssetMenu(fileName = "AchievementData", menuName = "AchievementSystem/AchievementData", order = 0)]
    public class AchievementData : ScriptableObject,ITooltipInfo
    {
        [SerializeField] private string id;
        [SerializeField] private AchEnum achType;
        [SerializeField,TextArea(3,5)] private string title;
        [SerializeField] private Sprite icon;
        [SerializeField,TextArea(8,15)] private string description;


        public string Title => title;

        public Sprite Icon => icon;

        public string Description => description;

        public string ID => id;

        public AchEnum AchType => achType;

        public string GetID()
        {
            return ID;
        }

#if UNITY_EDITOR
        [Button]
        private void SetNameToID()
        {
            id = name;
        }
#endif
        public NTooltipInfo GetTooltipInfo(Transform followRoot = null)
        {
            var info = new NTooltipInfo
            {
                FollowTarget = followRoot,
                NTooltipType = NTooltipTypes.Default,
                Layout = NTooltipLayout.Default
            };
            info.SetStringVariable(NTooltipKeys.Title, title);
            info.SetStringVariable(NTooltipKeys.Description, description);
            return info;
        }
    }
}