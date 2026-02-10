using System;
using _NueCore.Common.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace __Project.Scripts
{
    public class Dragger : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private LayerMask groundLayer;
        
        private Vector3 _mousePos;
        private void OnMouseDown()
        {
        }

        private void OnMouseUp()
        {
          
        }

        private void OnMouseDrag()
        {
            var ray = CameraStatic.GetMouseRay();
           
            if (Physics.Raycast(ray.origin,ray.direction,out var hit, 1000, groundLayer))
            {
                var pos = hit.point;
                pos.y = 1;
                transform.position = pos;
            }
        }

        private Vector3 GetMousePos()
        {
            return CameraStatic.MainCamera.WorldToScreenPoint(transform.position);
        }
#if UNITY_EDITOR

        [Button]
        private void FindRb()
        {
            rb = GetComponent<Rigidbody>();
        }
#endif
    }
}