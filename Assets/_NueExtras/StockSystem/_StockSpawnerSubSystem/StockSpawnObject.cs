﻿using System;
using System.Collections.Generic;
using _NueCore.AudioSystem;
using _NueCore.Common.ReactiveUtils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace _NueExtras.StockSystem._StockSpawnerSubSystem
{
    public class StockSpawnObject : MonoBehaviour
    {
        [SerializeField] private bool usePhysic;
        [SerializeField] private bool ignoreDisable;
        [SerializeField] private AudioData claimEndSfxData;
        [SerializeField,ShowIf(nameof(usePhysic))] private Rigidbody rb;
        [SerializeField,ShowIf(nameof(usePhysic))] private Collider col;
        [SerializeField] private Image rend;
        
        private bool _isDead;
        private Action<StockSpawnObject> FinishedAction { get; set; }

        public bool UsePhysic => usePhysic;

        public Rigidbody Rb => rb;

        public Collider Col => col;
        private List<IDisposable> _disposableList = new List<IDisposable>();

        // Static batching system for optimized saves
        private static bool _hasPendingSave;
        private static StockSaveBatcher _saveBatcher;

        public void ActivateDisable()
        {
            ignoreDisable = false;
        }

        private bool _hasBuilt;

        public void Build(Action<StockSpawnObject> onFinishedAction)
        {
            _hasBuilt = true;
            _isDead = false;
            FinishedAction = onFinishedAction;
        }
        
        public void ResetState()
        {
            DisposeAll();
            _hasBuilt = false;
            _isDead = false;
            ignoreDisable = false;
            FinishedAction = null;
            _disposableList.Clear();
        }

        private void DisposeAll()
        {
            foreach (var disposable in _disposableList)
            {
                disposable?.Dispose();
            }
        }

        /// <summary>
        /// Request a batched save at the end of the current frame
        /// </summary>
        private static void RequestBatchedSave()
        {
            if (_hasPendingSave) return;
            
            _hasPendingSave = true;
            
            if (_saveBatcher == null)
            {
                var go = new GameObject("[StockSaveBatcher]");
                _saveBatcher = go.AddComponent<StockSaveBatcher>();
                Object.DontDestroyOnLoad(go);
            }
            
            _saveBatcher.RequestSave(() => _hasPendingSave = false);
        }


        /// <summary>
        /// Complete the object's lifecycle and return it to the pool
        /// </summary>
        public void DestroyYourself()
        {
            if (ignoreDisable || _isDead)
                return;
            
            if (claimEndSfxData)
                claimEndSfxData.Play();
            
            _isDead = true;
            
            // Invoke the callback which returns the object to the pool
            FinishedAction?.Invoke(this);
            RequestBatchedSave();
        }
  
        private void OnApplicationFocus(bool hasFocus)
        {
            if (ignoreDisable)
                return;
            if (!hasFocus)
            {
                TryKill();
                DestroyYourself();
            }
        }
        
        private void OnApplicationQuit()
        {
            if (ignoreDisable)
                return;
            TryKill();
        }

        private void OnDisable()
        {
            if (ignoreDisable)
                return;
            TryKill();
        }

        private void TryKill()
        {
            if (ignoreDisable || _isDead)
                return;
            
            _isDead = true;
           
            // Invoke the callback which returns the object to the pool
            FinishedAction?.Invoke(this);
            RequestBatchedSave();
        }

        public void ChangeSprite(Sprite objCustomSprite)
        {
            if (!objCustomSprite)
            {
                return;
            }
            if (rend)
            {
                rend.sprite = objCustomSprite;
            }
        }
    }
}