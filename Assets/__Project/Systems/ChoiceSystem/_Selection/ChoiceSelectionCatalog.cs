using System.Collections.Generic;
using _NueCore.Common.Extensions;
using UnityEngine;

namespace __Project.Systems.ChoiceSystem._Selection
{
    [CreateAssetMenu(fileName = "ChoiceSelectionCatalog", menuName = "ChoiceSystem/Catalog")]
    public class ChoiceSelectionCatalog : ScriptableObject
    {
        [SerializeField] private List<ChoiceSelectionData> orderedSelectionDataList;
        [SerializeField] private List<ChoiceSelectionData> randomSelectionDataList;

        public List<ChoiceSelectionData> OrderedSelectionDataList => orderedSelectionDataList;
        public List<ChoiceSelectionData> RandomSelectionDataList => randomSelectionDataList;


        public ChoiceSelectionData GetChoiceSelectionData(int order)
        {
            if (order >= orderedSelectionDataList.Count)
            {
                var diff = order - orderedSelectionDataList.Count;
                var randomIndex = diff % randomSelectionDataList.Count;
                return randomSelectionDataList[randomIndex];
            }

            return orderedSelectionDataList[order];
        }

        public ChoiceSelectionData GetChoiceSelectionRandomData()
        {
            return randomSelectionDataList.RandomItem();
        }
    }
}