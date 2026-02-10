using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace __Project.Systems.NUpgradeSystem._SkillTree
{
   
    public class SkillNodeRefs : MonoBehaviour
    {
        [SerializeField] private Button purchaseButton;
        [SerializeField] private Transform root;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private Transform lineRoot;
        [SerializeField] private Image iconImage;
        
        public Button PurchaseButton => purchaseButton;
        public Transform Root => root;
        
        public Image IconImage => iconImage;

        public Transform VisualRoot => visualRoot;

        public Transform LineRoot => lineRoot;
    }
}