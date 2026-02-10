using System.Collections.Generic;
using __Project.Systems.CameraSystem;
using _NueCore.Common.NueLogger;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace _NueCore.Common.Utility
{
    public static class CameraStatic
    {
        private static Camera _camera;
        public static Camera MainCamera
        {
            get
            {
                if (!_camera)
                {
                    _camera = Camera.main;
                }
                return _camera;
            }
        }
        
        public static Vector3 GetMouseWorldPosition()
        {
            var mousePos = Input.mousePosition;
            mousePos.z = MainCamera.nearClipPlane;
            return MainCamera.ScreenToWorldPoint(mousePos);
        }
        
        public static Vector3 GetWorldPosition(Vector3 uiPos)
        {
            var mousePos = uiPos;
            mousePos.z = MainCamera.nearClipPlane;
            return MainCamera.ScreenToWorldPoint(mousePos);
        }
        
        public static Vector3 GetUIPos(Vector3 worldPos)
        {
            return MainCamera.WorldToScreenPoint(worldPos);
        }
        
        public static Vector3 GetMouseWorldPositionWithZ(float z)
        {
            var mousePos = Input.mousePosition;
            mousePos.z = z;
            return MainCamera.ScreenToWorldPoint(mousePos);
        }
        
        public static Ray GetMouseRay()
        {
            return MainCamera.ScreenPointToRay(Input.mousePosition);
        }
        
        public static bool IsPositionOnScreen(Vector3 position)
        {
            Vector3 viewportPoint = MainCamera.WorldToViewportPoint(position);
            return viewportPoint.x is > 0 and < 1 && viewportPoint.y is > 0 and < 1;
        }

        public static Vector3 GetWorldPositionPlane(float zValue =0f)
        {
            var camera = MainCamera;
            var dragPlane = new Plane(camera.transform.forward, camera.transform.forward.normalized*zValue);
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            if (dragPlane.Raycast(ray, out float distance))
            {
                return ray.GetPoint(distance);
            }
            return Vector3.zero;
        }
        
        
        public static Dictionary<CameraEnum, CinemachineCamera> CameraDict { get; private set; } =
            new Dictionary<CameraEnum, CinemachineCamera>();
        public static CameraEnum CurrentState { get; private set; }

        public static CinemachineCamera GetCamera(CameraEnum targetCam)
        {
            if (CameraDict.TryGetValue(targetCam, out var camera))
            {
                return camera;
            }

            return null;
        }
        public static void ChangeCamera(CameraEnum targetCam)
        {
            targetCam.NLog(Color.yellow);
            if (!CameraDict.ContainsKey(targetCam))
                return;
            foreach (var cm in CameraDict)
                cm.Value.gameObject.SetActive(false);
            CameraDict[targetCam].gameObject.SetActive(true);
            CurrentState = targetCam;
        }

        public static async UniTask ChangeCameraAsync(CameraEnum targetCam)
        {
            if (!CameraDict.ContainsKey(targetCam))
                return;
            foreach (var cm in CameraDict)
                cm.Value.gameObject.SetActive(false);
            CameraDict[targetCam].gameObject.SetActive(true);
            CurrentState = targetCam;
            await UniTask.Delay(1000);
        }
    }
}