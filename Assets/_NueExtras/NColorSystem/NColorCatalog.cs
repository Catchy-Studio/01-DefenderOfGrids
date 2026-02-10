using _NueCore.Common.KeyValueDict;
using UnityEngine;

namespace _NueExtras.NColorSystem
{
    [CreateAssetMenu(fileName = "NColorCatalog", menuName = "N/ColorSystem/Catalog", order = 0)]
    public class NColorCatalog : ScriptableObject
    {
        [SerializeField] private KeyValueDict<NColor,Color> commonColorDict = new KeyValueDict<NColor, Color>();
        [SerializeField] private KeyValueDict<string,Color> customColorDict = new KeyValueDict<string, Color>();
        
        
        public string GetColorHex(NColor nColor)
        {
            if (commonColorDict.TryGetValue(nColor,out var color))
                return ColorUtility.ToHtmlStringRGBA(color);
            return "FFFFFFFF";
        }
        
        public string GetColorHex(string customColorKey)
        {
            if (customColorDict.TryGetValue(customColorKey,out var color))
                return ColorUtility.ToHtmlStringRGBA(color);
            return "FFFFFFFF";
        }
    }
}