using UnityEngine;

namespace NueGames.NTooltip
{
  
    public abstract class NTooltipData : ScriptableObject
    {
        [SerializeField] protected NTooltip tooltipPrefab;
        [SerializeField] protected AnimationCurve fadeCurve;
        [SerializeField] protected float showDelayTime = 0.5f;

        public AnimationCurve FadeCurve => fadeCurve;
        public float ShowDelayTime => showDelayTime;
        public abstract string GetID();
        public virtual NTooltip GetTooltipPrefab()
        {
            return tooltipPrefab;
        }
        
    }
}