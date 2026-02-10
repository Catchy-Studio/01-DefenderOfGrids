using System.Collections.Generic;
using UnityEngine;

namespace _NueExtras.AchievementSystem
{
    [CreateAssetMenu(fileName = "Achievement_Catalog", menuName = "AchievementSystem/Catalog", order = 0)]
    public class AchievementDataCatalog : ScriptableObject
    {
        [SerializeField] private List<AchievementData> achievementDataList;

        public List<AchievementData> AchievementDataList => achievementDataList;
    }
}