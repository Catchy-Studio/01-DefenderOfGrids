
using Sirenix.OdinInspector;
using UnityEngine;

namespace _NueCore.Common.Utility
{
    [RequireComponent(typeof(Canvas))]
    public class ScreenCameraCanvasSetter : MonoBehaviour
    {
        [SerializeField] private float planeDistance = 1;
        [SerializeField,ReadOnly]private Canvas canvas;
        private void Awake()
        {
            if (canvas == null)
                canvas = GetComponent<Canvas>();
            canvas.worldCamera = CameraStatic.MainCamera;
            canvas.planeDistance = planeDistance;
            canvas.vertexColorAlwaysGammaSpace = true;
        }

// #if UNITY_EDITOR
//         private void OnValidate()
//         {
//             if (Application.isPlaying)
//                 return;
//             if (canvas == null)
//                 canvas = GetComponent<Canvas>();
//             if (canvas.worldCamera == null)
//             {
//                 canvas.worldCamera = Camera.main;
//             }
//             canvas.planeDistance = planeDistance;
//         }
// #endif
    }
}