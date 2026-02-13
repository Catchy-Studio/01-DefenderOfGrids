using System;
using System.Collections.Generic;
using __Project.Systems.DamageNumberSystem;
using __Project.Systems.NUpgradeSystem._SkillTree.Comps;
using _NueCore.AudioSystem;
using _NueCore.Common.NueLogger;
using _NueCore.Common.ReactiveUtils;
using _NueCore.SaveSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace __Project.Systems.NUpgradeSystem._SkillTree
{
    [RequireComponent(typeof(SkillNodeRefs)),SelectionBase]
    public abstract class SkillNodeBase : MonoBehaviour
    {
        [SerializeField,ReadOnly] private SkillData data;
        
        #region Actions
        public Func<bool> IsLockedFunc { get; set; }
        public Func<bool> IsRevealedFunc { get; set; }
        public Func<bool> CanPurchaseFunc { get; set; }
        public Func<bool> IsMaxedFunc { get; set; }
        public Action OnNodeUpdatedAction { get; set; }
        public Action<int> OnPurchasedAction { get; set; }

        #endregion
        
        #region Comps
        public SkillTreeController Tree { get; private set; }
        public SkillData Data => data;
        public SkillNodeRefs Refs { get; private set; }
        public List<SkillNodeCompBase> CompList { get; private set; } = new List<SkillNodeCompBase>();
        private readonly Dictionary<Type, SkillNodeCompBase> _compCache = new Dictionary<System.Type, SkillNodeCompBase>();
        
        public T GetComp<T>() where T : SkillNodeCompBase
        {
            if (_compCache.TryGetValue(typeof(T), out var cachedComp))
                return (T)cachedComp;
            var comp = (T)CompList.Find(c => c is T);
            if (comp != null)
                _compCache[typeof(T)] = comp;
            return comp;
        }

        public void SetData(SkillData d)
        {
            data = d;
        }
        #endregion
        
        #region Setup
        public void Build(SkillTreeController tree)
        {
            Tree = tree;
            Refs = GetComponent<SkillNodeRefs>();
            if (Data == null)
            {
                $"No Data {gameObject.name}".NLog(Color.red,gameObject);
            }
            CompList = new List<SkillNodeCompBase>(GetComponents<SkillNodeCompBase>());

            Refs.PurchaseButton.onClick.AddListener(OnPurchaseClicked);
            var save = GetSave();
            if (!save.isUnlocked)
            {
                save.isUnlocked = true;
            }
            //Load();
            OnBuilt();
            foreach (var comp in CompList)
                comp.Build(this);
            
        }
        protected virtual void OnBuilt()
        {
            
        }
        #endregion

        #region Methods
        private void OnPurchaseClicked()
        {
            if (!CanPurchase())
            {
                var n = NumberStatic.GetDamageNumber(NumberTypes.Warning);
                n.enableLeftText = true;
                n.transform.position = transform.position;
                n.leftText = IsMaxed() ? "Maxed" :  "Cannot Purchase";
                AudioStatic.PlayFx(DefaultAudioDataTypes.Error);
                return;
            }
            AudioStatic.PlayFx(DefaultAudioDataTypes.Purchase);
            Purchase();
        }

        public virtual void UpdateNode()
        {
            OnNodeUpdatedAction?.Invoke();
        }
        #endregion
        
        #region Info

        private SkillSave _save = null;
        public SkillSave GetSave()
        {
            if (_save == null)
            {
                var save = NSaver.GetSaveData<UpgradeSave>();
                _save =save.GetNodeSave(GetID());
            }

            return _save;
        }
        public string GetID()
        {
            return Data.GetID();
        }
        #endregion
        
        #region Purchase
        public bool IsMaxed()
        {
            if (IsMaxedFunc != null)
            {
                var t = IsMaxedFunc.Invoke();
                if (!t)
                    return false;
            }
            return true;
        }
        public bool CanPurchase()
        {
            if (!IsUnlocked())
                return false;
            if (IsMaxed())
                return false;
            if (CanPurchaseFunc != null)
            {
                var t = CanPurchaseFunc.Invoke();
                if (!t)
                    return false;
            }
            return true;
        }
        public void Purchase()
        {
            var save =GetSave();
            var number = NumberStatic.GetDamageNumber(NumberTypes.Upgrade_Info);
            number.enableLeftText = true;
            number.transform.position = transform.position;
            number.leftText = "Purchased!";
            OnPurchased();
            OnPurchasedAction?.Invoke(save.purchaseCount);
            save.purchaseCount++;
            var upgradeSave = NSaver.GetSaveData<UpgradeSave>();
            upgradeSave.TotalPurchasedSkillPoints += 1;
            upgradeSave.Save();
            Data.PurchasedEvent?.Invoke();
            UpdateNode();
            RBuss.Publish(new SkillTreeREvents.SkillNodePurchasedEvent(this));
          
        }
        protected virtual void OnPurchased()
        {
            
        }
        
        public int GetPurchaseCount() 
        {
            var save = GetSave();
            return save.purchaseCount;
        }
        
        public bool IsUnlocked()
        {
            var save = GetSave();
            if (Data.IsPermaLocked)
            {
                return false;
            }
            if (save.isUnlocked)
            {
                return true;
            }

            if (Data.IsAutoUnlocked)
            {
                Unlock();
                return true;
            }
            if (IsLockedFunc != null)
            {
                var t = IsLockedFunc.Invoke();
                if (t)
                    return false;
            }

            return true;
        }

        public bool IsRevealed()
        {
            //return false;
            if (IsRevealedFunc != null)
            {
                var t = IsRevealedFunc.Invoke();
                if (t)
                    return true;
            }
            return false;
        }

        public void Unlock()
        {
            var save = GetSave();
            save.isUnlocked = true;
            UpdateNode();
        }
        
        public NodeStatus GetStatus()
        {
            if (!IsUnlocked())
                return NodeStatus.Passive;
            
            if (IsMaxed())
                return NodeStatus.Maxed;
            
            if(CanPurchase())
                return NodeStatus.Ready;
            
            return NodeStatus.Active;
        }
        #endregion


        #region Editor

#if UNITY_EDITOR
        public void SetEditor()
        {
            
        }
#endif
        #endregion
       
    }
}