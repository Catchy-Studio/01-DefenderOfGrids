using System;
using System.Collections.Generic;
using _NueCore.Common.ReactiveUtils;
using _NueCore.SaveSystem;
using _NueExtras.TutorialSystem._Dialog;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _NueExtras.TutorialSystem
{
    public abstract class NTutorialTask : MonoBehaviour
    {
        [SerializeField,TabGroup("Base")] private string id;
        [SerializeField,TabGroup("Base")] private bool isTrackable;
        [SerializeField,TabGroup("Base")] private bool saveAfterComplete;
        [SerializeField,TabGroup("Base")] private TutorialDialog dialog;
        [SerializeField,TabGroup("Base")] private AudioClip dialogClip;

        
        #region Cache

        public Action OnCompletedAction;
        protected bool IsSaved { get; private set; }
        protected bool IsCompleted { get; private set; }
        public List<IDisposable> ActiveDisposableList { get; private set; }= new List<IDisposable>();
        public List<IDisposable> TrackDisposableList { get; private set; }= new List<IDisposable>();

        public bool IsTrackable => isTrackable;
        public NTutorialTask ActiveTask { get; private set; }

        protected TutorialDialog Dialog => dialog;

        #endregion
        
        #region Setup

        private void Awake()
        {
            if (Dialog)
                Dialog.Hide();
        }

        public void Build()
        {
            if (CheckCompleted()) 
                return;
            OnBuilt();
        }

        public virtual void Track()
        {
            
        }

        public virtual void FinishTrack()
        {
            foreach (var disposable in TrackDisposableList)
            {
                if (disposable != null)
                    disposable?.Dispose();
            }
        }
        [Button]
        public void Activate()
        {
            FinishTrack();
            OnActivated();
            RBuss.Publish(new NTutorialREvents.TaskActivatedREvent(this));
        }

       
        public void Finish()
        {
            if (Dialog)
            {
                Dialog.Hide();
            }
            OnFinished();
            foreach (var disposable in ActiveDisposableList)
            {
                if (disposable != null)
                    disposable?.Dispose();
            }
        }

        protected abstract void OnBuilt();
        protected abstract void OnActivated();
        protected abstract void OnFinished();

        public void DisposeAll()
        {
            foreach (var disposable in TrackDisposableList)
            {
                disposable?.Dispose();
            }

            foreach (var disposable in ActiveDisposableList)
            {
                disposable?.Dispose();
            }
        }
        #endregion

        #region Dialog
        protected void PlayDialog(string text)
        {
            dialog.gameObject.SetActive(true);
            PlayDialogAsync(text).AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy()).Forget();
        }
        protected void PlayDialog(string text, Func<UniTask> onCompleteTask)
        {
            dialog.gameObject.SetActive(true);
            PlayDialogAsync(text, onCompleteTask).AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy()).Forget();
        }

        protected async UniTask PlayDialogAsync(string text)
        {
            if (Dialog)
            {
                if (dialogClip)
                    Dialog.PlayClip(dialogClip);
                await Dialog.PlayAsync(text);
               
            }
            else
            {
                Debug.LogWarning("Dialog is not assigned for task: " + id);
            }
        }
        protected async UniTask PlayDialogAsync(string text, Func<UniTask> onCompleteTask)
        {
            await PlayDialogAsync(text);
            if (onCompleteTask != null)
            {
                await onCompleteTask.Invoke();
            }
        }

        protected virtual void HideDialog()
        {
            if (Dialog)
            {
                Dialog.Hide();
            }
        }
        #endregion

        #region Methods
        [Button]
        public void Complete()
        {
            IsCompleted = true;
            if (saveAfterComplete)
            {
                var taskSaveData = NSaver.GetSaveData<NTutorialSave>();
                taskSaveData.CompleteTask(GetID());
                taskSaveData.Save();
            }
            
            OnCompletedAction?.Invoke();
            RBuss.Publish(new NTutorialREvents.TaskCompletedREvent(this));
            Finish();
        }
        public string GetID()
        {
            return id;
        }
        protected bool CheckCompleted()
        {
            if (IsCompleted)
                return true;
            var taskSaveData = NSaver.GetSaveData<NTutorialSave>();
            
            var hasTask = taskSaveData.TutorialSaveInfoDict.ContainsKey(GetID());

            if (hasTask)
                IsSaved = taskSaveData.TutorialSaveInfoDict[GetID()].IsCompleted;
            
            if(IsSaved)
                return true;
            return false;
        }
        #endregion

#if UNITY_EDITOR

        [Button]
        private void SetNameToID()
        {
            id = name;
        }
#endif
    }
}