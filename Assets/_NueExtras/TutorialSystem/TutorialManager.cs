using System.Collections.Generic;
using System.Linq;
using _NueCore.Common.NueLogger;
using _NueCore.Common.ReactiveUtils;
using _NueCore.ManagerSystem.Core;
using _NueCore.SaveSystem;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UniRx;
using UnityEngine;

namespace _NueExtras.TutorialSystem
{
    public class TutorialManager : NManagerBase
    {
        
        [SerializeField,TabGroup("Main")] private List<TutorialTaskBase> tasks;
        
        #region Cache

        public static TutorialManager Instance { get; private set; }
        private Dictionary<string, TutorialSave.TutorialSaveInfo> _taskSaveInfoDict = new Dictionary<string, TutorialSave.TutorialSaveInfo>();
        private Dictionary<int, List<TutorialTaskBase>> _orderedTaskIdDict = new Dictionary<int, List<TutorialTaskBase>>();
        private Dictionary<int, List<TutorialTaskBase>> _removeOrderedTaskIdDict = new Dictionary<int, List<TutorialTaskBase>>();
        private Dictionary<int, List<TutorialTaskBase>> _combinedTaskIdDict = new Dictionary<int, List<TutorialTaskBase>>();
        private Dictionary<int, List<TutorialTaskBase>> _removeCombinedTaskIdDict = new Dictionary<int, List<TutorialTaskBase>>();
        private Dictionary<string, TutorialSave.TutorialSaveInfo> _questSaveInfoDict = new Dictionary<string, TutorialSave.TutorialSaveInfo>();
        private TutorialSave TutorialSave => NSaver.GetSaveData<TutorialSave>();
        private bool _tasksStarted;

        private bool _tasksConstructed;

        private float _taskStartTimer;
        private const float TaskStartDuration = .5f;
        private bool _tasksCanStart;
          

        private bool _firstQuestTaskInitiated;

        #endregion

        #region Setup
        public override void NAwake()
        {
            Instance = InitSingleton<TutorialManager>();
            GetSaveInfoFromSaveData();
            base.NAwake();
            
            RegisterREvents();
            SetAndSortTasksForCheckIDs();
            CheckTasksFromSave();

            _tasksConstructed = true;
        }

        public override void NStart()
        {
            base.NStart();
            _tasksStarted = true;
        }
        #endregion

        #region Reactive
        private void RegisterREvents()
        {
           RBuss.OnEvent<TutorialREvents.OnSetTaskConditionsREvent>().Subscribe(ev =>
            {
            
            }).AddTo(gameObject);

            RBuss.OnEvent<TutorialREvents.OnTaskCompleteREvent>().Subscribe(ev =>
            {
                if (!ev.TutorialTaskBase.DontSave)
                {
                    _taskSaveInfoDict[ev.TutorialTaskBase.TaskID].IsCompleted = true;
                    TutorialSave.TutorialSaveInfoDict[ev.TutorialTaskBase.TaskID].IsCompleted = true;
                    TutorialSave.Save();
                }
                if (ev.TutorialTaskBase.IsOrdered)
                {
                    if(_removeOrderedTaskIdDict.ContainsKey(ev.TutorialTaskBase.CheckID))
                        _removeOrderedTaskIdDict[ev.TutorialTaskBase.CheckID].Add(ev.TutorialTaskBase);
                    else
                        _removeOrderedTaskIdDict.Add(ev.TutorialTaskBase.CheckID, new List<TutorialTaskBase> {ev.TutorialTaskBase});
                }
                else
                {
                    if(_removeCombinedTaskIdDict.ContainsKey(ev.TutorialTaskBase.CheckID))
                        _removeCombinedTaskIdDict[ev.TutorialTaskBase.CheckID].Add(ev.TutorialTaskBase);
                    else
                        _removeCombinedTaskIdDict.Add(ev.TutorialTaskBase.CheckID, new List<TutorialTaskBase> {ev.TutorialTaskBase});
                }
            }).AddTo(gameObject);

            RBuss.OnEvent<TutorialREvents.OnTaskShowREvent>().Subscribe(ev =>
            {
                if (TutorialSave.TutorialSaveInfoDict.TryGetValue(ev.TutorialTaskBase.TaskID, out var taskSaveInfo))
                {
                    taskSaveInfo.IsShown = true;
                }
            }).AddTo(gameObject);
            
            RBuss.OnEvent<TutorialREvents.OnTaskTextCloseREvent>().Subscribe(ev =>
            {
                
            }).AddTo(gameObject);

            RBuss.OnEvent<TutorialREvents.OnTaskTextOpenAndSetREvent>().Subscribe(ev =>
            {
                if (ev.TaskText.IsNullOrWhitespace()) return;
               
            }).AddTo(gameObject);
        }
        #endregion

        #region Process
        private void Update()
        {
            if(_tasksStarted == false || _tasksConstructed == false) return;

            if (!_tasksCanStart)
            {
                _taskStartTimer += Time.deltaTime;
            }

            if (_taskStartTimer >= TaskStartDuration && !_tasksCanStart)
            {
                _tasksCanStart = true;
            }
            
            if(!_tasksCanStart) return;
            
            
            CheckTasks();
        }
        private void CheckTasks()
        {
            CheckOrderedTasks();
            CheckCombinedTasks();
        }

        #endregion

        #region Methods
        private void GetSaveInfoFromSaveData()
        {
            _taskSaveInfoDict.Clear();
            _questSaveInfoDict.Clear();

            foreach (var kvp in TutorialSave.TutorialSaveInfoDict)
            {
                if (_taskSaveInfoDict.ContainsKey(kvp.Key))
                {
                    $"Has same Task Guid: {kvp.Key}".NLog();
                    _taskSaveInfoDict[kvp.Key] = kvp.Value;
                    continue;
                }
                
                _taskSaveInfoDict.Add(kvp.Key, kvp.Value);
            }
            
            foreach (var kvp in TutorialSave.QuestSaveInfoDict)
            {
                if (_questSaveInfoDict.ContainsKey(kvp.Key))
                {
                    $"Has same Quest Guid: {kvp.Key}".NLog();
                    _questSaveInfoDict[kvp.Key] = kvp.Value;
                    continue;
                }
                _questSaveInfoDict.Add(kvp.Key, kvp.Value);
            }
        }
        private void SkipTutorial()
        {
            foreach (var task in _orderedTaskIdDict)
            {
                foreach (var t in task.Value)
                {
                    t.CompleteTask();
                }
                //task.CompleteTask();
            }
                
            foreach (var task in _combinedTaskIdDict)
            {
                foreach (var t in task.Value)
                {
                    t.CompleteTask();
                }
            }
              
            _orderedTaskIdDict.Clear();
            _combinedTaskIdDict.Clear();
        }
        private void CheckTasksFromSave()
        {
            for (var i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                var taskGuid = task.TaskID;
              
                if (!_taskSaveInfoDict.ContainsKey(taskGuid))
                {
                    var saveInfo = new TutorialSave.TutorialSaveInfo(task.TaskID, task.Amount, task.IsOrdered,false , false);
                    _taskSaveInfoDict.Add(task.TaskID, saveInfo);
                    TutorialSave.TutorialSaveInfoDict.Add(task.TaskID, saveInfo);
                }
                else
                {
                    var saveInfo = _taskSaveInfoDict[taskGuid];
                    if (saveInfo.IsCompleted)
                    {
                        tasks.Remove(task);
                        task.DestroyItself();
                        i--;
                        continue;
                    }
                    task.Amount = saveInfo.TaskStep;
                }
            }
            
            TutorialSave.Save();
        }
         private void SetAndSortTasksForCheckIDs()
        {
            foreach (var task in tasks)
            {
                if (task.IsOrdered)
                {
                    if (_orderedTaskIdDict.ContainsKey(task.CheckID))
                    {
                        _orderedTaskIdDict[task.CheckID].Add(task);
                    }
                    else
                    {
                        _orderedTaskIdDict.Add(task.CheckID, new List<TutorialTaskBase> {task});
                    }
                }
                else
                {
                    if (_combinedTaskIdDict.ContainsKey(task.CheckID))
                    {
                        _combinedTaskIdDict[task.CheckID].Add(task);
                    }
                    else
                    {
                        _combinedTaskIdDict.Add(task.CheckID, new List<TutorialTaskBase> {task});
                    }
                }
           
            }

            var orderedTaskListList = new List<List<TutorialTaskBase>>();
            foreach (var kvp in _orderedTaskIdDict)
            {
                orderedTaskListList.Add(kvp.Value.OrderBy(t => t.OrderNumber).ToList());
            }

            for (var i = 0; i < _orderedTaskIdDict.Count; i++)
            {
                var kvp = _orderedTaskIdDict.ElementAt(i);
                _orderedTaskIdDict[kvp.Key] = orderedTaskListList[i];
            }
            
            
            var combinedTaskListList = new List<List<TutorialTaskBase>>();
            foreach (var kvp in _combinedTaskIdDict)
            {
                combinedTaskListList.Add(kvp.Value.OrderBy(t => t.OrderNumber).ToList());
            }

            for (var i = 0; i < _combinedTaskIdDict.Count; i++)
            {
                var kvp = _combinedTaskIdDict.ElementAt(i);
                _combinedTaskIdDict[kvp.Key] = combinedTaskListList[i];
            }
        }
        private void CheckOrderedTasks()
        {
            foreach (var kvp in _removeOrderedTaskIdDict)
            {
                var taskRemoveList = kvp.Value;
                if (taskRemoveList.Count > 0)
                {
                    foreach (var task in taskRemoveList)
                    {
                        _orderedTaskIdDict[task.CheckID].Remove(task);
                        task.DestroyItself();
                    }
                
                    taskRemoveList.Clear();
                }
            }
            
            foreach (var kvp in _orderedTaskIdDict)
            {
                var taskIdList = kvp.Value;
                if (taskIdList.Count <= 0) continue;
                for (var i = 0; i < taskIdList.Count; i++)
                {
                    if(taskIdList[i] == null)
                    {
                        taskIdList.RemoveAt(i);
                        i--;
                        continue;
                    }
                        
                    if(taskIdList[i].gameObject.activeInHierarchy)
                    {
                        taskIdList[i].CheckTaskConditions();
                        break;
                    }
                }
            }
        }
        private void CheckCombinedTasks()
        {
            foreach (var kvp in _removeCombinedTaskIdDict)
            {
                var taskRemoveList = kvp.Value;
                if (taskRemoveList.Count <= 0) continue;
                foreach (var task in taskRemoveList)
                {
                    _combinedTaskIdDict[task.CheckID].Remove(task);
                    task.DestroyItself();
                }
                
                taskRemoveList.Clear();
            }
            
            foreach (var kvp in _combinedTaskIdDict)
            {
                var taskIdList = kvp.Value;
                if (taskIdList.Count <= 0) continue;
                
                for (var i = 0; i < taskIdList.Count; i++)
                {
                    if (taskIdList[i] == null)
                    {
                        taskIdList.RemoveAt(i);
                        i--;
                        continue;
                    }

                    if(taskIdList[i].gameObject.activeInHierarchy)
                        taskIdList[i].CheckTaskConditions();
                }
            }
        }
        private void StartTutorial()
        {
            "Tasks Started".NLog();
            _tasksStarted = true;
        }
        #endregion
    }
}