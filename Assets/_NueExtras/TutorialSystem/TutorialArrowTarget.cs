using DG.Tweening;
using UnityEngine;

namespace _NueExtras.TutorialSystem
{
    public class TutorialArrowTarget : MonoBehaviour
    {
        [SerializeField] private GameObject meshRoot;
        [SerializeField] private Transform distanceRoot;
        [SerializeField] private float initDistance = 4f;
        
        private bool _isInitialized;
        private UpdateType _updateType;
        private Transform _target;
        private Transform _parent;

        public Transform CurrentTarget => _target;

        private float _initDistance = 4f;

        private bool _hasLerpMovement;
        
        public void Init(Transform parent, Transform target, UpdateType updateType, bool hasLerpMovement = false, float initDistance = 4f)
        {
            this.initDistance = initDistance;
            _initDistance = this.initDistance;
            transform.SetParent(parent);
            var lossyScale = transform.lossyScale.x;
            if (lossyScale > 0)
            {
                transform.localScale = new Vector3(1f / lossyScale, 1f / lossyScale, 1f / lossyScale);
            }

            _hasLerpMovement = hasLerpMovement;
            transform.localPosition = Vector3.zero;
            _target = target;
            _parent = parent;
            _updateType = updateType;
            LookTarget(Time.deltaTime);
            _isInitialized = true;
        }
        
        public void ChangeTarget(Transform target, bool hasLerpMovement = false)
        {
            if(_target == target) return;
            
            _target = target;
            _hasLerpMovement = hasLerpMovement;
        }
    

        private void Update()
        {
            if (!_isInitialized) return;
            if(_updateType != UpdateType.Normal) return;
            LookTarget(Time.deltaTime);
        }

        private void LateUpdate()
        {
            if (!_isInitialized) return;
            if(_updateType != UpdateType.Late) return;
            LookTarget(Time.deltaTime);
        }
        
        private void FixedUpdate()
        {
            if (!_isInitialized) return;
            if(_updateType != UpdateType.Fixed) return;
            LookTarget(Time.fixedDeltaTime);
        }

        private float _lerpTimer;
        private float _lerpDuration = 1f;
        
        private void LookTarget(float deltaTime)
        {
            if (_target == null)
            {
                Destroy(gameObject);
                return;
            }

            var targetPos = _target.position;
            var parentPos = _parent.position;
            targetPos.y = 0;
            parentPos.y = 0;

            var direction = targetPos - parentPos;
            if (direction.magnitude < _initDistance)
            {
                distanceRoot.localPosition = new Vector3(0f, direction.magnitude, 0f);
            }
            else
            {
                distanceRoot.localPosition = new Vector3(0f, _initDistance, 0f);
            }
            //direction.y = transform.position.y;

            if (direction.magnitude <= 1.5f)
            {
                Hide();
            }
            else
            {
                if(!isHideOverriden)
                    Show();
            }

            if (_hasLerpMovement)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), deltaTime * 10f);
                _lerpTimer += deltaTime;
                if(_lerpTimer >= _lerpDuration)
                {
                    _lerpTimer = 0f;
                    _hasLerpMovement = false;
                }
                return;
            }
            
            transform.rotation = Quaternion.LookRotation(direction);
        }

        public bool isHideOverriden;
        public void HideOverride()
        {
            isHideOverriden = true;
            Hide();
        }
        
        public void ShowOverride()
        {
            isHideOverriden = false;
            Show();
        }

        private void Show()
        {
            if (!meshRoot.activeSelf)
                meshRoot.SetActive(true);
        }

        private void Hide()
        {
            if (meshRoot.activeSelf)
                meshRoot.SetActive(false);
        }
    }
}