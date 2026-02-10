using _NueCore.Common.Utility;
using UnityEngine;

namespace _NueExtras.TutorialSystem._Hands
{
    public class TutorialHandUIMover : MonoBehaviour
    {
        [SerializeField] private Transform handObj;
        private float _animDuration = 1f;
        private float _timer = 0f;

        private Transform _pos;
        private Transform _target;
        private bool _useUI;
        public void Build(Transform pos, Transform target,bool useUI)
        {
            _useUI = true;
            _pos = pos;
            _target = target;
            handObj.position = pos.position;
            _timer = 0f;
            _moveIndex = 0;
        }

        private int _moveIndex = 0;

        private void Update()
        {
            if (!_pos)
            {
                return;
            }

            if (!_target)
            {
                return;
            }
            _timer += Time.unscaledDeltaTime;
            
            if(_timer > _animDuration)
            {
                _timer = _animDuration;
            }

            var pos = _pos.position;
            var target = _target.position;
            if (_useUI)
            {
                pos = CameraStatic.GetUIPos(pos);
                target = CameraStatic.GetUIPos(target);
            }
            if (_moveIndex == 0)
            {
                handObj.position = Vector3.Lerp(pos, target, EaseHelper.EaseInOutSine(_timer, _animDuration));
            }
            else
            {
                handObj.position = Vector3.Lerp(target, pos, EaseHelper.EaseInOutSine(_timer, _animDuration));
            }
            
            if(_timer >= _animDuration)
            {
                _timer = 0f;
                _moveIndex++;
                
                if (_moveIndex > 1)
                {
                    _moveIndex = 0;
                }
                
            }
        }
    }
}