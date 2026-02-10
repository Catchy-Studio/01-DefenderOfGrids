using System;
using _NueCore.Common.ReactiveUtils;
using UnityEngine;
using UnityEngine.UI;

namespace _NueExtras.TutorialSystem._Utility
{
    public class FinishTutorialButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        private void Awake()
        {
            button.onClick.AddListener(() =>
            {
                var nt = NTutorialManager.Instance;
                nt.FinishTutorial();
                //RBuss.Publish(new DeckController.LevelClearedREvent());
            });
        }
    }
}