using UnityEngine;

namespace _NueCore.NStatSystem
{
    [CreateAssetMenu(fileName = "NStatCustom", menuName = "N/StatSystem/CustomData", order = 0)]
    public class NStatCustom : ScriptableObject
    {
        [SerializeField] private string key;

        public string Key => key;
    }
}