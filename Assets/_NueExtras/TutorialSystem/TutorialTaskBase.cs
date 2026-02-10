using System.Collections;
using System.Collections.Generic;
using _NueCore.Common.NueLogger;
using _NueCore.Common.ReactiveUtils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace _NueExtras.TutorialSystem
{
    public abstract class TutorialTaskBase : MonoBehaviour
    {
        [SerializeField,TabGroup("Task")] protected string taskID;
        [SerializeField,TabGroup("Task")] protected int checkID = 0;
        [SerializeField,TabGroup("Task")] protected bool isOrdered;
        [SerializeField,TabGroup("Task")] protected bool canForceShow;
        [SerializeField,TabGroup("Task"),ShowIf("@isOrdered")] protected int orderNumber;
        [SerializeField,TabGroup("Task"),TextArea(2, 20)] protected string taskText;
        [SerializeField,TabGroup("Task")] protected bool dontSave;

        [SerializeField,TabGroup("Start")] protected float startDelay;
        [SerializeField,TabGroup("Start")] protected UnityEvent OnStart;
        [SerializeField,TabGroup("Start")] protected List<GameObject> objsToOpenAtStart;
        [SerializeField,TabGroup("Start")] protected List<GameObject> objsToCloseAtStart;
        [SerializeField,TabGroup("Start")] protected List<RxEventSender> startEventSenders;
        [SerializeField,TabGroup("Start")] protected List<GameObject> objsToCloseAtStartCheck;
        [SerializeField,TabGroup("Start")] protected List<GameObject> objsToOpenAtStartCheck;

        
        [SerializeField,TabGroup("Complete")] protected float completeDelay;
        [SerializeField,TabGroup("Complete")] protected UnityEvent OnBeforeComplete;
        [SerializeField,TabGroup("Complete")] protected UnityEvent OnComplete;
        [SerializeField,TabGroup("Complete")] protected List<GameObject> objsToOpenAtBeforeComplete;
        [SerializeField,TabGroup("Complete")] protected List<GameObject> objsToCloseAtBeforeComplete;
        [SerializeField,TabGroup("Complete")] protected List<GameObject> objsToOpenAtComplete;
        [SerializeField,TabGroup("Complete")] protected List<GameObject> objsToCloseAtComplete;
        [SerializeField,TabGroup("Complete")] protected List<RxEventSender> completeEventSenders;
        [SerializeField,TabGroup("Complete")] protected List<GameObject> objsToCloseAtCompleteCheck;
        [SerializeField,TabGroup("Complete")] protected List<GameObject> objsToOpenAtCompleteCheck;
        [SerializeField,TabGroup("Complete")] protected bool destroyClosedCompleteObjs;

        #region Fields

        protected bool _taskStarted;
        protected bool _taskCompleted;
        protected bool _isTaskShown;
        protected bool _isConditionsSet;
        protected bool _isTaskShowRoutineStarted;
        protected bool _isTaskCompleteRoutineStarted;
        protected bool _showCheckStarted;
        protected bool _completeCheckStarted;

        protected Coroutine _taskShowCoroutine;
        protected Coroutine _taskCompleteCoroutine;

        public string TaskID => taskID;
        public int OrderNumber => orderNumber;
        public bool IsOrdered => isOrdered;
        public bool CanForceShow => canForceShow;
        public bool DontSave => dontSave;
        public bool HasForceShow { get; set; }
        public virtual int CheckID => checkID;
        public virtual int Amount { get; set; }

        #endregion

        #region Setup
        private void Start()
        {
            Init();
        }

        protected virtual void OnDisable()
        {
        }

        public virtual void Init()
        {
            RBuss.Publish(new TutorialREvents.OnSetTaskConditionsREvent(this));
        }

        #endregion

        #region ConditionCheck
        public virtual void CheckTaskConditions()
        {
            if (!CheckShowTaskCondition()) return;

            CheckCompleteCondition();
        }
        protected virtual bool CheckShowTaskCondition()
        {
            if (_isTaskShown) return true;

            if (!_showCheckStarted)
            {
                _showCheckStarted = true;
                foreach (var obj in objsToCloseAtStartCheck)
                {
                    obj.SetActive(false);
                }

                foreach (var obj in objsToOpenAtStartCheck)
                {
                    obj.SetActive(true);
                }
            }

            if (GetShowConditions())
            {
                ShowTask();
            }

            return false;
        }
        protected virtual void CheckCompleteCondition()
        {
            if (!_completeCheckStarted)
            {
                _completeCheckStarted = true;
                foreach (var obj in objsToCloseAtCompleteCheck)
                {
                    obj.SetActive(false);
                }

                foreach (var obj in objsToOpenAtCompleteCheck)
                {
                    obj.SetActive(true);
                }
            }

            if (GetCompleteConditions())
            {
                CompleteTask();
            }
        }

        #endregion

        #region MainTaskMethods

        #region Complete

        public void CompleteTask()
        {
            if (_isTaskCompleteRoutineStarted) return;

            _isTaskCompleteRoutineStarted = true;

            _taskCompleteCoroutine = StartCoroutine(CompleteTaskRoutine());

            IEnumerator CompleteTaskRoutine()
            {
                BeforeCompleteTaskActions();

                yield return new WaitForSeconds(completeDelay);

                yield return StartCoroutine(CompleteEffectCoroutine());

                DefaultCompleteTaskActions();
            }
        }

        protected virtual IEnumerator CompleteEffectCoroutine()
        {
            yield return null;
        }
        
        protected void BeforeCompleteTaskActions()
        {
            OnBeforeComplete?.Invoke();

            foreach (var obj in objsToOpenAtBeforeComplete)
            {
                obj.SetActive(true);
            }

            foreach (var obj in objsToCloseAtBeforeComplete)
            {
                obj.SetActive(false);
            }

            objsToOpenAtBeforeComplete.Clear();
            objsToCloseAtBeforeComplete.Clear();
        }
        
        protected virtual void DefaultCompleteTaskActions()
        {
            
            OnComplete?.Invoke();
            
            completeEventSenders.ForEach(sender => sender.SendEvent());

            foreach (var obj in objsToOpenAtComplete)
            {
                obj.SetActive(true);
            }

            foreach (var obj in objsToCloseAtComplete)
            {
                obj.SetActive(false);
            }

            if (destroyClosedCompleteObjs)
            {
                for (var i = 0; i < objsToCloseAtComplete.Count; i++)
                {
                    Destroy(objsToCloseAtComplete[i]);
                }
            }

            objsToCloseAtStart.Clear();
            objsToOpenAtStart.Clear();
            objsToCloseAtComplete.Clear();
            objsToOpenAtComplete.Clear();

            ExecuteCompleteActions();

            _taskCompleted = true;

            (taskID + " Complete").NLog(Color.green);

            RBuss.Publish(new TutorialREvents.OnTaskCompleteREvent(this));
            
        }

        #endregion

        #region Show

        protected void ShowTask()
        {
            if (HasForceShow) return;
            if (_isTaskShowRoutineStarted) return;

            _isTaskShowRoutineStarted = true;

            _taskShowCoroutine = StartCoroutine(ShowTaskRoutine());

            IEnumerator ShowTaskRoutine()
            {
                yield return new WaitForSeconds(startDelay);
                
                DefaultShowTaskActions(false);
            }
        }
        
        protected virtual void DefaultShowTaskActions(bool isForced, bool allowCameraLanding = false)
        {
            OnStart?.Invoke();
            
            startEventSenders.ForEach(sender => sender.SendEvent());

            foreach (var obj in objsToOpenAtStart)
            {
                obj.SetActive(true);
            }

            foreach (var obj in objsToCloseAtStart)
            {
                obj.SetActive(false);
            }

            RBuss.Publish(new TutorialREvents.OnTaskShowREvent(this));

            (taskID + " Show").NLog(Color.yellow);

            RBuss.Publish(new TutorialREvents.OnTaskTextOpenAndSetREvent(taskText));

            ExecuteShowActions();

            _isTaskShown = true;
        }

        #endregion

        protected abstract void ExecuteShowActions();
        protected abstract void ExecuteCompleteActions();
        protected abstract bool GetShowConditions();
        protected abstract bool GetCompleteConditions();
        protected abstract void ExecuteForceShowActions(bool allowCameraLanding);
        protected abstract void ExecuteForceCompleteActions();
        protected abstract void BeforeDestroyBehaviours();

        #endregion

        #region Methods 2
        
        public void DestroyItself()
        {
            BeforeDestroyBehaviours();

            Destroy(this);
        }
        public void RevertShowCondition()
        {
            if (_taskShowCoroutine != null)
            {
                StopCoroutine(_taskShowCoroutine);
                _taskShowCoroutine = null;
            }

            _isTaskShown = false;
            _isTaskShowRoutineStarted = false;
        }
        public void ForceShowTask(bool allowCameraLanding)
        {
            ExecuteForceShowActions(allowCameraLanding);
        }
        public void ForceCompleteTask()
        {
            ExecuteForceCompleteActions();
        }

        #endregion
    }
}