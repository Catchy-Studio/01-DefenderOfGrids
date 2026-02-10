using UnityEngine;

namespace _NueCore.Common.Utility
{
    [RequireComponent(typeof(Canvas))]
    public class WorldCanvasCameraSetter : MonoBehaviour
    {
        private void Start()
        {
            if (TryGetComponent<Canvas>(out var canvas))
            {
                canvas.worldCamera = CameraStatic.MainCamera;
                
            }
        }
    }
}