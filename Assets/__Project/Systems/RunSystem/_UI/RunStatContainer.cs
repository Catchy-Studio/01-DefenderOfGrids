using _NueCore.Common.Utility;
using UnityEngine;

namespace __Project.Systems.RunSystem._UI
{
    public class RunStatContainer : MonoBehaviour
    {
        public Transform root;
        public N_TMP_Text tmp;
        [SerializeField] private bool round;
            
        public void UpdateStat(float value)
        {
            tmp.SetValue((round ? Mathf.RoundToInt(value) : value).ToString("0"));
        }
    }
}