using System.Text;
using _NueCore.Common.NueLogger;
using _NueCore.SaveSystem;
using TMPro;
using UnityEngine;

namespace _NueExtras.NTrackerSystem
{
    public class TrackTester : MonoBehaviour
    {
        [SerializeField] private TMP_Text displayText;
        [SerializeField] private bool everyUpdate;
        

        private StringBuilder _str = new StringBuilder();
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                ShowStats();
            }

            if (everyUpdate)
            {
                ShowStats();
            }
        }

        private void ShowStats()
        {
            _str.Clear();
            _str.AppendLine("GLOBAL");
            var save = NSaver.GetSaveData<TrackSave_Global>();
            if (save == null) return;
            if (save.globalTrack == null)
            {
                _str.AppendLine("No Global Stats");
            }
            else
            {
                foreach (var kvp in save.globalTrack.StringDict)
                {
                    _str.AppendLine($"{kvp.Key}: {kvp.Value}");
                }

                _str.AppendLine("<>");
                foreach (var kvp in save.globalTrack.IntDict)
                {
                    _str.AppendLine($"{kvp.Key}: {kvp.Value}");
                }
                _str.AppendLine("<>");
                foreach (var kvp in save.globalTrack.FloatDict)
                {
                    _str.AppendLine($"{kvp.Key}: {kvp.Value}");
                }
                _str.AppendLine("<>");
                foreach (var kvp in save.globalTrack.DoubleDict)
                {
                    _str.AppendLine($"{kvp.Key}: {kvp.Value}");
                }
                _str.AppendLine("<>");
                foreach (var kvp in save.globalTrack.DecimalDict)
                {
                    _str.AppendLine($"{kvp.Key}: {kvp.Value}");
                }
                _str.AppendLine("<>");
            }
            _str.AppendLine("********************");
            _str.AppendLine("SESSION");
            
            if (save.sessionTrack == null)
            {
                _str.AppendLine("No Session Stats");
            }
            else
            {
                foreach (var kvp in save.sessionTrack.StringDict)
                {
                    _str.AppendLine($"{kvp.Key}: {kvp.Value}");
                }

                _str.AppendLine("<>");
                foreach (var kvp in save.sessionTrack.IntDict)
                {
                    _str.AppendLine($"{kvp.Key}: {kvp.Value}");
                }
                _str.AppendLine("<>");
                foreach (var kvp in save.sessionTrack.FloatDict)
                {
                    _str.AppendLine($"{kvp.Key}: {kvp.Value}");
                }
                _str.AppendLine("<>");
                foreach (var kvp in save.sessionTrack.DoubleDict)
                {
                    _str.AppendLine($"{kvp.Key}: {kvp.Value}");
                }
                _str.AppendLine("<>");
                foreach (var kvp in save.sessionTrack.DecimalDict)
                {
                    _str.AppendLine($"{kvp.Key}: {kvp.Value}");
                }
            }
            _str.AppendLine("*******************");
            _str.AppendLine("ACTIVE");
            var activeStats = NTrackStatic.GetActiveStats();
            if (activeStats == null)
            {
                _str.AppendLine("No Active Stats");
            }
            else
            {
                foreach (var kvp in activeStats.StringDict)
                {
                    _str.AppendLine($"{kvp.Key}: {kvp.Value}");
                }

                _str.AppendLine("<>");
                foreach (var kvp in activeStats.IntDict)
                {
                    _str.AppendLine($"{kvp.Key}: {kvp.Value}");
                }
                _str.AppendLine("<>");
                foreach (var kvp in activeStats.FloatDict)
                {
                    _str.AppendLine($"{kvp.Key}: {kvp.Value}");
                }
                _str.AppendLine("<>");
                foreach (var kvp in activeStats.DoubleDict)
                {
                    _str.AppendLine($"{kvp.Key}: {kvp.Value}");
                }
                _str.AppendLine("<>");
                foreach (var kvp in activeStats.DecimalDict)
                {
                    _str.AppendLine($"{kvp.Key}: {kvp.Value}");
                }
            }
            
            _str.AppendLine("*******************");

            if (displayText)
            {
                displayText.text = _str.ToString();
            }
            _str.ToString().NLog(Color.white);
        }
    }
}