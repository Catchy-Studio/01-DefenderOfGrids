using __Project.Systems.RunSystem;
using _NueCore.Common.KeyValueDict;
using _NueCore.Common.Utility;
using _NueCore.ManagerSystem.Core;
using MoreMountains.Feedbacks;
using UniRx;
using Unity.Cinemachine;
using UnityEngine;

namespace __Project.Systems.CameraSystem
{
    public class CameraManager : NManagerBase
    {
        [SerializeField] private KeyValueDict<CameraEnum,CinemachineCamera> cameraDict = new KeyValueDict<CameraEnum, CinemachineCamera>();
        [SerializeField] private MMF_Player shakeFx;


        #region Cache

        public MMF_Player ShakeFx => shakeFx;
        #endregion

        #region Setup

        private CameraEnum _cachedCameraEnum;
        public override void NAwake()
        {
            //CameraStatic.ShakePlayer = ShakeFx;
            CameraStatic.CameraDict.Clear();
            foreach (var kvp in cameraDict)
                CameraStatic.CameraDict.Add(kvp.Key,kvp.Value);
            base.NAwake();
            
        }

        public override void NStart()
        {
            base.NStart();
        }

        #endregion
    }
}