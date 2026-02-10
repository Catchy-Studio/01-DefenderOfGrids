using _NueCore.Common.Utility;
using UnityEngine;

namespace _NueExtras.TutorialSystem._Hands
{
    public class TutorialHandUITapper : MonoBehaviour
    {
        [SerializeField] private GameObject modelRoot;

        private Transform _targetPos;
        private bool _useUI;
        public void Build(Transform pos,bool useUI)
        {
            modelRoot.SetActive(true);
            _useUI = useUI;
            _targetPos = pos;
        }

        private void Update()
        {
            if (_targetPos)
            {
                var pos = _targetPos.position;
                if (_useUI)
                {
                    pos =CameraStatic.GetUIPos(pos);
                }
                transform.position = pos;
            }
        }
    }
}