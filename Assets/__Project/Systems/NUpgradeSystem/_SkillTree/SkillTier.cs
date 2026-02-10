using System;
using System.Collections.Generic;
using _NueCore.NStatSystem;
using _NueExtras.StockSystem;
using Random = UnityEngine.Random;

namespace __Project.Systems.NUpgradeSystem._SkillTree
{
    [Serializable]
    public class SkillTier
    {
        //public int tier;
        public NStatList statList;
        public List<RequiredResource> requiredResourceList = new List<RequiredResource>();

        public void RandomizeData()
        {
            if (statList == null)
            {
                statList = new NStatList();
            }
            statList.AddStatCustom("TEST",Random.Range(0,100));
            requiredResourceList.Clear();
            var res = new RequiredResource
            {
                StockType = StockTypes.Coin,
                Amount = Random.Range(10,500)
            };

            if (Random.value<=0.1f)
            {
                var res2 = new RequiredResource
                {
                    StockType = StockTypes.Gem,
                    Amount = Random.Range(1,10)
                };
                requiredResourceList.Add(res2);
            }
            requiredResourceList.Add(res);
        }
    }

    [Serializable]
    public class RequiredResource
    {
        public StockTypes StockType;
        public int Amount;
    }
}