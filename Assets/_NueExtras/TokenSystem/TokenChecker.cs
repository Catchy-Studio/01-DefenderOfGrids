using System;
using System.Collections;
using _NueCore.SaveSystem;
using UnityEngine;

namespace _NueExtras.TokenSystem
{
    public class TokenChecker : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            var save = NSaver.GetSaveData<TokenSave>();
            if (save.CollectedExp>0)
                TokenManager.Instance.ShowTokenProgressPopup();
        }
    }
}