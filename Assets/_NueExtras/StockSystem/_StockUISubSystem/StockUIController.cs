using System;
using __Project.Systems.RunSystem;
using _NueCore.Common.ReactiveUtils;
using _NueCore.SceneSystem;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _NueExtras.StockSystem._StockUISubSystem
{
    public class StockUIController : MonoBehaviour
    {
        [SerializeField] private StockCounterLayout menuLayout;
        [SerializeField] private StockCounterLayout gameLayout;
        [SerializeField] private Transform rootTransform;

        private void Awake()
        {
            RBuss.OnEvent<SceneREvents.SceneChangeFinishedREvent>().Subscribe(ev =>
            {
                if (ev.NewSceneName == SceneStatic.GameScene)
                {
                    gameLayout.Hide();
                }
                else if (ev.NewSceneName == SceneStatic.UpgradeScene)
                {
                    gameLayout.Show();
                }
                else if (ev.NewSceneName == SceneStatic.LobbyScene)
                {
                    gameLayout.Hide();
                }
            }).AddTo(gameLayout);
        }

        private void Start()
        {
            SetGame();
            var currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == SceneStatic.GameScene)
            {
                gameLayout.Hide();
            }
            else if (currentScene == SceneStatic.UpgradeScene)
            {
                gameLayout.Show();
            }
            else if (currentScene == SceneStatic.LobbyScene)
            {
                gameLayout.Hide();
            }

        }

        private void SetGame()
        {
            gameLayout.Show();
            menuLayout.Hide();
        }
        private void SetMenu()
        {
            menuLayout.Show();
            gameLayout.Hide();
        }
        
    }
}