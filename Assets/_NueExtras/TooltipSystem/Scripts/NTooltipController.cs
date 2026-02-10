using _NueCore.Common.NueLogger;
using UnityEngine;

namespace NueGames.NTooltip
{
    public class NTooltipController : MonoBehaviour
    {
        [SerializeField] private RectTransform canvasRectTransform;
        [SerializeField] private RectTransform followRectTransform;
        [SerializeField] private KVD<NTooltipLayout,RectTransform> layoutDict = new KVD<NTooltipLayout, RectTransform>();
        
        #region Cache
        private RectTransform FollowRectTransform => followRectTransform;
        private Vector2 _followPos = Vector2.zero;
        private bool _isFollowEnabled;
        private Camera _cachedCamera;
        private Camera _followCam;
        private bool _is3D;
        private Transform _lastStaticTarget;
        #endregion

        #region Process
        private void Update()
        {
            if (NTooltipManager.Instance.ShownSourceList.Count<=0)
            {
                return;
            }
            SetPosition();
        }
        #endregion

        #region Methods

        public void SetLayout(NTooltipLayout layout,NTooltip tooltip)
        {
            if (layoutDict.TryGetValue(layout, out var layoutRectTransform))
            {
                tooltip.transform.SetParent(layoutRectTransform);
                tooltip.transform.localPosition = Vector3.zero;
            }
            else
            {
                Debug.LogWarning($"Layout {layout} not found in layout dictionary.");
            }
        }
        public void SetFollowPos(Transform followTarget = null,bool is3D = false,Camera cam =null)
        {
            if (followTarget)
            {
                var mainCam = cam;
                if (mainCam == null)
                {
                    if (!_cachedCamera)
                        _cachedCamera = Camera.main;
                    
                    mainCam = _cachedCamera;
                }
                if (mainCam == null)
                {
                    SetFollowPos();
                    return;
                }
                _followCam = mainCam;
                _lastStaticTarget = followTarget;
                _is3D = is3D;
                _isFollowEnabled = false;
                return;
            }
            _followCam = null;
            _lastStaticTarget = null;
            _is3D = false;
            _isFollowEnabled = true;
        }
        private void SetPosition()
        {
            if (_isFollowEnabled)
                _followPos = Input.mousePosition;
            else
            {
                if (_followCam && _lastStaticTarget)
                {
                    if (!_is3D)
                    {
                        var screenPos = _lastStaticTarget.position;
                        _followPos = new Vector2(screenPos.x, screenPos.y);
                    }
                    else
                    {
                       // Convert world position to screen position for 3D objects
                        var screenPos = _followCam.WorldToScreenPoint(_lastStaticTarget.position);
                        // _lastStaticTarget.position.NLog(Color.yellow);
                        // screenPos.NLog(Color.magenta);
                        // Don't update position if object is behind camera (z is negative)
                        if (screenPos.z >= 0)
                            _followPos = new Vector2(screenPos.x, screenPos.y);
                        else
                            _followPos = new Vector2(-1000, -1000); // Move tooltip off-screen when target is behind camera
                    }
                }
                else
                {
                    _followPos = Input.mousePosition;
                }
            }

            var anchoredPos = _followPos / canvasRectTransform.localScale.x;
            
            if (anchoredPos.x + FollowRectTransform.rect.width>canvasRectTransform.rect.width)
                anchoredPos.x = canvasRectTransform.rect.width - FollowRectTransform.rect.width;
            
            if (anchoredPos.y + FollowRectTransform.rect.height>canvasRectTransform.rect.height)
                anchoredPos.y = canvasRectTransform.rect.height - FollowRectTransform.rect.height;
            
            FollowRectTransform.anchoredPosition = anchoredPos;
        }
        #endregion
    }
}