using _NueCore.Common.KeyValueDict;
using _NueCore.ManagerSystem.Core;
using DamageNumbersPro;
using UnityEngine;

namespace __Project.Systems.DamageNumberSystem
{
    public class DamageNumberManager : NManagerBase
    {
        [SerializeField] private KeyValueDict<NumberTypes,DamageNumber> numberPrefabDict = new KeyValueDict<NumberTypes, DamageNumber>();
        
        public override void NAwake()
        {
            base.NAwake();
            foreach (var number in numberPrefabDict)
                NumberStatic.NumberDict.Add(number.Key,number.Value);
        }
        
    }
}