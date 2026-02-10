using System.Collections;
using System.Collections.Generic;
using _NueCore.ManagerSystem.Core;
using UnityEngine;

namespace NueGames.NTooltip
{
    public class NTooltipManager : NManagerBase
    {
        #region Singleton
        public static NTooltipManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindObjectOfType<NTooltipManager>();
                if (_instance != null) return _instance;
                var items =Resources.FindObjectsOfTypeAll<NTooltipManager>();
                if (items.Length > 0)
                {
                    _instance = Instantiate(items[0]);
                }
                else
                {
                    Debug.LogError("NTooltipManager not found in the scene or resources. Please ensure it is present.");
                    return null;
                }
                return _instance;
            }
        }
        private static NTooltipManager _instance;
        #endregion
        
        [SerializeField] private NTooltipController nTooltipController;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private List<NTooltipData> tooltipDataList = new List<NTooltipData>();
        
        #region Cache
        private NTooltipController NTooltipController => nTooltipController;
        public Dictionary<string, List<NTooltip>> SpawnedTooltipDict { get; private set; } = new Dictionary<string, List<NTooltip>>();
        public Dictionary<string, int> ShownTooltipCountDict { get; private set; }= new Dictionary<string, int>();
        public List<string> ShownSourceList { get; private set; } = new List<string>();

        public List<NTooltip> ActiveTooltips { get; private set; } = new List<NTooltip>();
        #endregion

        #region Setup
        
        public override void NAwake()
        {
            base.NAwake();
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            InitDictionaries();
        }

        private void InitDictionaries()
        {
            var tooltipStyleList = tooltipDataList;
            SpawnedTooltipDict.Clear();
            ShownTooltipCountDict.Clear();

            foreach (var tooltipType in tooltipStyleList)
            {
                var tooltipID = tooltipType.GetID();
                SpawnedTooltipDict.Add(tooltipID, new List<NTooltip>());
                ShownTooltipCountDict.Add(tooltipID, 0);
            }
        }
        #endregion

        #region Local Methods
        public void ShowTooltip(NTooltipInfo nTooltipInfo)
        {
            StartCoroutine(ShowRoutine(nTooltipInfo));
            
        }
        public void HideTooltip()
        {
            ActiveTooltips.Clear();
            StopAllCoroutines();
            ShownSourceList.Clear();
            canvasGroup.alpha = 0;
            foreach (var kvp in SpawnedTooltipDict)
            {
                foreach (var tooltip in kvp.Value)
                    tooltip.gameObject.SetActive(false);
                SetShownCount(kvp.Key,0);
            }
        }
        
        
        private int GetShownCount(string tooltipID)
        {
            if (!ShownTooltipCountDict.ContainsKey(tooltipID))
            {
                ShownTooltipCountDict.Add(tooltipID, 0);
            }
            return ShownTooltipCountDict[tooltipID];
        }

        private void SetShownCount(string tooltipID, int value)
        {
            if (!ShownTooltipCountDict.ContainsKey(tooltipID))
            {
                ShownTooltipCountDict.Add(tooltipID, 0);
            }
            ShownTooltipCountDict[tooltipID] = value;
        }

        public NTooltipData GetStyleData(string tooltipID)
        {
            return tooltipDataList.Find(x => x.GetID() == tooltipID);
        } 
        private NTooltip GetTooltipPrefab(string tooltipID)
        {
            var data =tooltipDataList.Find(x => x.GetID() == tooltipID);
            return data ? data.GetTooltipPrefab() : null;
        }

        private List<NTooltip> GetSpawnedTooltipList(string tooltipID)
        {
            if (!SpawnedTooltipDict.ContainsKey(tooltipID))
            {
                SpawnedTooltipDict.Add(tooltipID, new List<NTooltip>());
            }
            return SpawnedTooltipDict[tooltipID];
        } 
        #endregion

        #region Routines
        private IEnumerator ShowRoutine(NTooltipInfo nTooltipInfo)
        {
            var tooltipID = nTooltipInfo.GetID();
            var tooltipData = GetStyleData(tooltipID);
            var waitFrame = new WaitForEndOfFrame();
            var timer = 0f;
            var delay = tooltipData ? tooltipData.ShowDelayTime : 0.5f;
            var curve = tooltipData ? tooltipData.FadeCurve : null;
            var currentShownCount = GetShownCount(tooltipID);
            canvasGroup.alpha = 0;
            
            SetShownCount(tooltipID,currentShownCount+1);
            
            var targetList = GetSpawnedTooltipList(tooltipID);
            
            currentShownCount = GetShownCount(tooltipID);
            
            if (targetList.Count<currentShownCount)
            {
                var tooltipPrefab = GetTooltipPrefab(tooltipID);
                var newTooltip = Instantiate(tooltipPrefab, NTooltipController.transform);
                newTooltip.Init(tooltipData);
                targetList.Add(newTooltip);
            }

            var selectedTooltip = targetList[currentShownCount - 1];
            selectedTooltip.gameObject.SetActive(true);
            selectedTooltip.Prepare(nTooltipInfo);
            
            ActiveTooltips.Add(selectedTooltip);
            var temp = NTooltipStatic.Temp;
            temp.TooltipCalled?.Invoke(selectedTooltip);
            NTooltipController.SetLayout(nTooltipInfo.Layout,selectedTooltip);
            yield return waitFrame;
            yield return waitFrame;
            NTooltipController.SetFollowPos(nTooltipInfo.FollowTarget,nTooltipInfo.Is3D,null);
            
            yield return waitFrame;
            yield return waitFrame;
            
            selectedTooltip.Show(nTooltipInfo);
            
            while (true)
            {
                timer += Time.deltaTime;

                var invValue = Mathf.InverseLerp(0, delay, timer);
                canvasGroup.alpha = curve?.Evaluate(invValue) ?? Mathf.Lerp(0,1,invValue);
                
                if (timer>=delay)
                { 
                    canvasGroup.alpha = 1;
                    break;
                }
                yield return waitFrame;
            }
        }
        #endregion
    }
}