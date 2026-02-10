using System;
using _NueCore.Common.KeyValueDict;
using UniRx;
using UnityEngine;

namespace __Project.Systems.NUpgradeSystem._SkillTree
{
    public class SkillLineRenderer : MonoBehaviour
    {
        [SerializeField] private KeyValueDict<NodeStatus,LineRenderer> lineRendDict = new KeyValueDict<NodeStatus, LineRenderer>();
        
        public NodeStatus CurrentStatus { get; private set; }
  
        
        private Vector3 _startPos;
        private Vector3 _endPos;
        
        public void SetPos(Vector3 pos1, Vector3 pos2)
        {
            _startPos = pos1;
            _endPos = pos2;
            
            // Calculate intermediate points based on distance for smooth waves
            float distance = Vector3.Distance(pos1, pos2);
            float pointsPerUnit = 5f; // Adjust this value for more/less density
            int intermediatePoints = Mathf.Max(3, Mathf.RoundToInt(distance * pointsPerUnit));
            int totalPoints = intermediatePoints + 2; // +2 for start and end points
            
            foreach (var keyValue in lineRendDict)
            {
                var rend = keyValue.Value;
                rend.positionCount = totalPoints;
                
                // Set positions along the line
                for (int i = 0; i < totalPoints; i++)
                {
                    float t = i / (float)(totalPoints - 1);
                    Vector3 pos = Vector3.Lerp(pos1, pos2, t);
                    rend.SetPosition(i, pos);
                }
            }
           
        }

        public void UpdateLineStatus(NodeStatus status,bool isRequireMaxed = false)
        {
            // Update visual based on status
            CurrentStatus = status;
            UpdateRend(isRequireMaxed);
        }

        public LineRenderer ActiveRenderer { get; private set; }
        private bool _isRequireMaxed = false;
        private IDisposable _disposable;
        private void UpdateRend(bool isRequireMaxed =false)
        {
            _isRequireMaxed = isRequireMaxed;
            foreach (var keyValue in lineRendDict)
            {
                var rend = keyValue.Value;
                if (keyValue.Key == CurrentStatus)
                {
                    rend.enabled = true;
                    ActiveRenderer = rend;
                }
                else
                {
                    rend.enabled = false;
                }
            }
            _disposable?.Dispose();
            if (_isRequireMaxed && CurrentStatus is not NodeStatus.Maxed)
            {
                _disposable = Observable.EveryUpdate().Subscribe(_ =>
                {
                    AnimateWaveMovement();
                });
            }
        }
        
        private void AnimateWaveMovement()
        {
            if (ActiveRenderer == null || ActiveRenderer.positionCount < 2) return;
            
            var positions = new Vector3[ActiveRenderer.positionCount];
            
            float waveAmplitude = 0.03f;
            float waveFrequency = 1f;
            float waveSpeed = 2f;
            
            // Calculate perpendicular direction in XY plane
            Vector3 direction = (_endPos - _startPos).normalized;
            Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0); // Perpendicular in XY plane
            
            // Keep start point steady
            positions[0] = _startPos;
            
            // Animate intermediate points with wave effect in XY plane
            for (int i = 1; i < positions.Length - 1; i++)
            {
                float t = i / (float)(positions.Length - 1);
                Vector3 basePos = Vector3.Lerp(_startPos, _endPos, t);
                
                float wave = Mathf.Sin(Time.time * waveSpeed + i * waveFrequency) * waveAmplitude;
                positions[i] = basePos + perpendicular * wave;
            }
            
            // Keep end point steady
            positions[^1] = _endPos;
            
            ActiveRenderer.SetPositions(positions);
        }
    }
}