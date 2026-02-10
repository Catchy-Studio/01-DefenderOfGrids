using System.Collections.Generic;
using _NueCore.ManagerSystem.SingletonSystems;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _NueCore.NStatSystem
{
    [CreateAssetMenu(fileName = "NStatCustoms", menuName = "N/StatSystem/CustomCatalog", order = 0)]
    public class NStatCustomCatalog : SingletonScriptableObject<NStatCustomCatalog>
    {
        [SerializeField] private List<NStatCustom> customList = new List<NStatCustom>();
        
        
        [ShowInInspector,ReadOnly]private Dictionary<string, NStatCustom> _customDict = new Dictionary<string, NStatCustom>();
        
        public NStatCustom GetCustom(string key)
        {
            if (_customDict.TryGetValue(key, out var custom))
            {
                return custom;
            }

            return null;
        }
        
        public void OnEnable()
        {
            Refresh();
        }

        [Button("Refresh")]
        private void Refresh()
        {
            _customDict.Clear();
            foreach (var custom in customList)
            {
                if (!_customDict.ContainsKey(custom.Key))
                {
                    _customDict.Add(custom.Key, custom);
                }
            }
        }
    }
}