using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _NueExtras.MainMenuSystem
{
    public class MenuButton : MonoBehaviour
    {
        [SerializeField,TabGroup("Main","References")] private Button button;
        [SerializeField] private Transform root;
        
        #region Cache
        public Button Button => button;
        #endregion

        #region Setup
        public void Build(Action onClicked)
        {
            button.onClick.AddListener(() =>
            {
                onClicked?.Invoke();
            });
            root.gameObject.SetActive(false);
        }

        #endregion
        
        #region Methods
        public void Show()
        {
            root.gameObject.SetActive(true);
        
        }

        public void Hide()
        {
            root.gameObject.SetActive(false);
        }
        #endregion
    }
}