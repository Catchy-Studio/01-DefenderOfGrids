using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _NueCore.SettingsSystem
{
    public class SettingsTabButton : MonoBehaviour
    {
        [SerializeField] private Transform border;
        [SerializeField] private UnityEvent onSelect;
        [SerializeField] private UnityEvent onDeSelect;

        public UnityEvent OnSelect => onSelect;

        public UnityEvent OnDeSelect => onDeSelect;

        private void Awake()
        {
            var btn =GetComponent<Button>();
            btn.onClick.AddListener((() =>
            {
                Select();
            }));
        }

        public void Select()
        {
            OnSelect?.Invoke();
        }

        public void Deselect()
        {
            OnDeSelect?.Invoke();
        }
    }
}