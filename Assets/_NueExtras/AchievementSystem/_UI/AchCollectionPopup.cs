using UnityEngine;

namespace _NueExtras.AchievementSystem
{
    public class AchCollectionPopup : MonoBehaviour
    {
        [SerializeField] private AchievementDataCatalog achievementCatalog;
        [SerializeField] private AchCollectionCard achCollectionCardPrefab;
        [SerializeField] private Transform spawnRoot;
        
        
        public void Build()
        {
            foreach (var achievementData in achievementCatalog.AchievementDataList)
            {
                var card = Instantiate(achCollectionCardPrefab, spawnRoot);
                card.Build(achievementData);
            }
        }
    }
}