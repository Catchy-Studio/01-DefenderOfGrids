using System;
using System.Collections;
using UnityEngine;

namespace _NueExtras.StockSystem._StockSpawnerSubSystem
{
    /// <summary>
    /// Persistent singleton that batches save operations to run at end of frame.
    /// This prevents multiple saves when collecting many stocks in the same frame.
    /// </summary>
    public class StockSaveBatcher : MonoBehaviour
    {
        private Coroutine _saveCoroutine;
        private Action _onSaveComplete;

        /// <summary>
        /// Request a batched save operation
        /// </summary>
        public void RequestSave(Action onComplete = null)
        {
            _onSaveComplete = onComplete;
            
            // Cancel any pending save and start a new one
            if (_saveCoroutine != null)
            {
                StopCoroutine(_saveCoroutine);
            }
            
            _saveCoroutine = StartCoroutine(SaveAtEndOfFrame());
        }

        private IEnumerator SaveAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            
            // Perform the actual save
            StockStatic.SaveStocks();
            
            // Notify completion
            _onSaveComplete?.Invoke();
            _saveCoroutine = null;
        }
    }
}

