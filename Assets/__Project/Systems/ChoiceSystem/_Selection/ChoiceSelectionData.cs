using System;
using System.Collections.Generic;
using System.Linq;
using _NueCore.Common.Extensions;
using _NueCore.Common.NueLogger;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace __Project.Systems.ChoiceSystem._Selection
{
    [CreateAssetMenu(fileName = "ChoiceSelectionData", menuName = "ChoiceSystem/ChoiceSelectionData")]
    public class ChoiceSelectionData : ScriptableObject
    {
        [SerializeField] private string title;
        [SerializeField] private List<ChoiceSelectionOption> optionList;
        public List<ChoiceSelectionOption> OptionList => optionList;

        public string Title => title;
    }

    [Serializable]
    public class ChoiceSelectionOption
    {
        [SerializeField] private List<Object> choiceList;
        
        public List<Object> ChoiceList => choiceList;

        public static List<IChoiceItem> AvailableChoiceList { get; private set; } =
            new List<IChoiceItem>();

        public static Dictionary<Object, IChoiceItem> ChoiceDict { get; private set; } =
            new Dictionary<Object, IChoiceItem>();
        public IChoiceItem GetRandomChoiceData()
        {
            return GetRandomChoiceDataList().RandomItem();
        }

        public List<IChoiceItem> GetRandomChoiceDataList(int count = 3)
        {
            CalculateAvailableChoices();
            AvailableChoiceList.Shuffle();
            var tAvailableList = AvailableChoiceList.ToList();
            var choiceItems = tAvailableList.FindAll(x =>
            {
                if (x == null)
                    return false;

                if (!x.IsAvailableToChoose())
                    return false;
                return true;
            });
            var tList = new List<IChoiceItem>();
            for (int i = 0; i < count; i++)
            {
                if (choiceItems.Count<=0)
                {
                    break;
                }
                var rand = choiceItems.RandomItem();
                choiceItems.Remove(rand);
                AvailableChoiceList.Remove(rand);
                tList.Add(rand);
            }
            return tList;
        }

        private void CalculateAvailableChoices()
        {
            if (AvailableChoiceList.Count <= 0)
            {
                AvailableChoiceList.Clear();
                    
                foreach (var item in ChoiceList)
                {
                    if (item == null)
                    {
                        "Item is null".NLog(Color.red);
                        continue;
                    }
                    if (ChoiceDict.ContainsKey(item))
                    {
                        AvailableChoiceList.Add(ChoiceDict[item]);
                        continue;
                    }

                    if (item is not IChoiceItem cast)
                    {
                        "Item is not matched".NLog(Color.red);
                        ChoiceList.Remove(item);
                        continue;
                    }
                    AvailableChoiceList.Add(cast);
                    ChoiceDict.Add(item,cast);
                }
                   
            }
        }
    }
}