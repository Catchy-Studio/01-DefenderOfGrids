using System;
using _NueCore.Common.ReactiveUtils;
using UniRx;
using UnityEngine;

namespace _NueExtras.CursorSystem
{
    public class CursorController : MonoBehaviour
    {
        [SerializeField] private CursorData cursorData;

        #region Cache
        private CursorType _activeCursorType = CursorType.Default;
        private CursorData.CursorProfile _activeCursorProfile;
        private float _currentFrameTimer = 0f;
        private int _currentAnimationFrame = 0;
        private int _totalAnimationFrame;
        private int _totalAnimationClickedFrame;
        private bool _canAnimate;
        public event EventHandler<OnCursorChangedEventArgs> OnCursorChanged;
        private bool _isClicked;
        #endregion

        #region Classes

        public class OnCursorChangedEventArgs : EventArgs
        {
            public CursorType CursorType;

            public OnCursorChangedEventArgs(CursorType targetType)
            {
                CursorType = targetType;
            }
        }

        #endregion

        #region Setup
        private void Awake()
        {
            SetActiveCursor(CursorType.Default);
            RBuss.OnEvent<CursorREvents.CursorChangedREvent>().TakeUntilDisable(gameObject).Subscribe(
                ev =>
                {
                    if (ev.CursorType == _activeCursorType)
                        return;
                    SetActiveCursor(ev.CursorType);
                });
        }
        #endregion

        #region Process
        private void Update()
        {
            CheckInput();
            CheckAnimation();
        }

        private void CheckInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _currentFrameTimer = 0;
                _isClicked = true;
                _currentAnimationFrame = 0;
                ChangeFrame(_isClicked);
            }

            if (Input.GetMouseButtonUp(0))
            {
                _currentFrameTimer = 0;
                _isClicked = false;
                _currentAnimationFrame = 0;
                ChangeFrame(_isClicked);
            }
        }

        private void CheckAnimation()
        {
            if (!_canAnimate) return;
            _currentFrameTimer -= Time.unscaledDeltaTime;
            if (!(_currentFrameTimer <= 0f)) return;
            
            _currentFrameTimer += _activeCursorProfile.frameRate;
            _currentAnimationFrame = (_currentAnimationFrame + 1) % (_totalAnimationFrame <= 0 ? 1 : _totalAnimationFrame);

            if (_isClicked &&_activeCursorProfile.cursorAnimationClickedFrameList.Count > 0) 
                _currentAnimationFrame = (_currentAnimationFrame + 1) % (_totalAnimationClickedFrame <= 0 ? 1 : _totalAnimationClickedFrame);
   
            ChangeFrame(_isClicked);
        }
        #endregion

        #region Methods

        public void SetActiveCursor(CursorType targetType,int startFrameIndex =0)
        {
            _activeCursorType = targetType;
            _activeCursorProfile = cursorData.GetCursor(_activeCursorType);
            
            _canAnimate = false;
            if (_activeCursorProfile != null)
            {
                _currentAnimationFrame = startFrameIndex;
                _totalAnimationFrame = _activeCursorProfile.cursorAnimationFrameList.Count;
                _totalAnimationClickedFrame = _activeCursorProfile.cursorAnimationClickedFrameList.Count;
                
                if (_activeCursorProfile.useCursorAnimation)
                {
                    _currentFrameTimer = _activeCursorProfile.frameRate;
                    _canAnimate = true;
                }
                else
                {
                    ChangeFrame();
                }
            }
            else
            {
                Cursor.SetCursor(null, Vector3.zero, CursorMode.ForceSoftware);
            }
            OnCursorChanged?.Invoke(this,new OnCursorChangedEventArgs(targetType));
        }
        private void ChangeFrame(bool isClicked = false)
        {
            if (_activeCursorProfile == null) return;
            if (_activeCursorProfile.cursorAnimationFrameList.Count <= _currentAnimationFrame) return;
             
            var animationFrame = _activeCursorProfile.cursorAnimationFrameList[_currentAnimationFrame];

            if (isClicked)
                if (_activeCursorProfile.cursorAnimationClickedFrameList.Count > 0)
                    animationFrame = _activeCursorProfile.cursorAnimationClickedFrameList[_currentAnimationFrame];
            
            if (animationFrame != null)
                Cursor.SetCursor(animationFrame.cursorTexture, animationFrame.cursorOffset,CursorMode.ForceSoftware);
            else
                Cursor.SetCursor(null, Vector3.zero, CursorMode.ForceSoftware);
        }

        #endregion
        
    }
}
