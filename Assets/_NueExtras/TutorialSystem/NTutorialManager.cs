using System;
using System.Collections.Generic;
using System.Linq;
using _NueCore.Common.NueLogger;
using _NueCore.Common.ReactiveUtils;
using _NueCore.ManagerSystem.Core;
using _NueCore.SaveSystem;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace _NueExtras.TutorialSystem
{
    public class NTutorialManager : NManagerBase
    {
        [SerializeField] private List<NTutorialTask> taskList = new List<NTutorialTask>();
        [SerializeField] private bool skipTutorial;

        private List<IDisposable> _disposables = new List<IDisposable>();

        public static NTutorialManager Instance { get; private set; }
        public NTutorialTask ActiveTask { get; private set; }

        public bool SkipTutorial => skipTutorial;

        public override void NAwake()
        {
            Instance = InitSingleton<NTutorialManager>();
            base.NAwake();
           
            RegisterREvents();
        }

        public override void NStart()
        {
            base.NStart();
            var tutorialSave = NSaver.GetSaveData<NTutorialSave>();
            if (SkipTutorial)
            {
                tutorialSave.TutorialFinished = true;
                tutorialSave.Save();
            }
            if (tutorialSave.TutorialFinished)
            {
                foreach (var task in taskList)
                {
                    if (task)
                        Destroy(task.gameObject);
                }
                return;
            }
            foreach (var task in taskList)
            {
                task.Build();
            }

            var lastItem =taskList.Last();
            lastItem.OnCompletedAction += () =>
            {
                FinishTutorial();
            };
            
            
        }

        private void RegisterREvents()
        {
            var tutorialSave = NSaver.GetSaveData<NTutorialSave>();
            if (tutorialSave.TutorialFinished)
            {
                return;
            }
            
            RBuss.OnEvent<NTutorialREvents.TaskActivatedREvent>().TakeUntilDisable(gameObject).Subscribe(ev =>
            {
                ActiveTask = ev.Task;
                $"ACTIVE TASK<{ActiveTask.gameObject.name}>".NLog(Color.yellow);
            }).AddTo(_disposables);
            
            
            RBuss.OnEvent<NTutorialREvents.TaskCompletedREvent>().TakeUntilDisable(gameObject).Subscribe(ev =>
            {
                if (tutorialSave.TutorialFinished)
                {
                    return;
                }

                var index = taskList.FindIndex(x => x.GetID() == ev.Task.GetID());
                index += 1;
                if (index>=taskList.Count)
                {
                    FinishTutorial();
                    return;
                }

                var nextTask =taskList[index];
                if (nextTask.IsTrackable)
                {
                    nextTask.Track();
                    return;
                }

                nextTask.Activate();
            }).AddTo(_disposables);
        }

        private bool _isTutorialStarted;
        public void StartTutorial()
        {
            if (_isTutorialStarted)
            {
                return;
            }

            _isTutorialStarted = true;
            taskList[0].Activate();
            RBuss.Publish(new NTutorialREvents.TutorialStartedREvent());
        }

        public void FinishTutorial()
        {
            var tutorialSave = NSaver.GetSaveData<NTutorialSave>();

            tutorialSave.TutorialFinished = true;
            tutorialSave.Save();
            RBuss.Publish(new NTutorialREvents.TutorialFinishedREvent());
            foreach (var disposable in _disposables)
                disposable?.Dispose();
            foreach (var task in taskList)
            {
                task.DisposeAll();
            }
        }
        
    }
}