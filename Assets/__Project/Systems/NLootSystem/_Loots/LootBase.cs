using System;
using _NueCore.Common.NueLogger;
using _NueCore.NStatSystem;
using UnityEngine;

namespace __Project.Systems.NLootSystem
{
    public abstract class LootBase : MonoBehaviour,ILootTarget
    {
        [SerializeField] private Rigidbody rb;
        
        #region Cache
        public float Amount { get; private set; }
        public LootData Data { get; private set; }
        public Action OnDisposeAction { get; set; }
        public Action OnCollectedAction { get; set; }
        protected bool IsCollected;
        public bool IgnoreCollectExternal { get; set; }

        public Rigidbody Rb => rb;

        #endregion

        #region Setup
        public void Build(LootData data)
        {
            Data = data;
            if (data.GetStatList().TryGetStat(NStatEnum.N1,out var stat))
            {
                Amount = stat.GetValue();
            }
            
            OnBuilt();
        }
        protected virtual void OnBuilt()
        {
            
        }
        public void Dispose()
        {
            OnDisposed();
            OnDisposeAction?.Invoke();
            Destroy(gameObject);
        }

        protected virtual void OnDisposed()
        {
            
        }
        #endregion

        #region Methods
        protected virtual void OnCollected()
        {
            
        }

       
        public void Collect()
        {
            if (IsCollected)
            {
                return;
            }

            IsCollected = true;
            OnCollected();
            OnCollectedAction?.Invoke();
            Dispose();
        }

        public virtual bool CanCollect()
        {
            if (IgnoreCollectExternal)
            {
                return false;
            }
            if (IsCollected)
                return false;
            return true;
        }
        #endregion
        
        public bool CanCollectLoot()
        {
            return CanCollect();
        }

        public void CollectLoot(Transform collector)
        {
            Collect();
        }
    }
}