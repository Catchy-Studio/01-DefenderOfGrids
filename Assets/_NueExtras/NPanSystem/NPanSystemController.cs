using UnityEngine;
using Unity.Cinemachine;

namespace _NueExtras.NPanSystem
{
    /// <summary>
    /// Generic pan and zoom system for 3D space using Cinemachine 3.0+
    /// </summary>
    public class NPanSystemController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera targetCamera;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private Transform cameraFollowTarget;
        
        [Header("Pan Settings")]
        [SerializeField] private float panSpeed = 0.5f;
        [SerializeField] private bool enablePan = true;
        [SerializeField] private float panSmoothTime = 0.15f;
        
        [Header("WASD Pan Settings")]
        [SerializeField] private bool enableWASDPan = true;
        [SerializeField] private float wasdPanSpeed = 10f;
        
        [Header("Zoom Settings")]
        [SerializeField] private float zoomSpeed = 10f;
        [SerializeField] private float minZoom = 5f;
        [SerializeField] private float maxZoom = 50f;
        [SerializeField] private bool enableZoom = true;
        [SerializeField] private float zoomSmoothTime = 0.15f;
        
        [Header("Boundary Settings")]
        [SerializeField] private bool useBoundary = true;
        [SerializeField] private Vector3 boundaryMin;
        [SerializeField] private Vector3 boundaryMax;
        [SerializeField] private float boundaryPadding = 2f;
        
        private Vector3 dragOrigin;
        private bool isDragging;
        private float currentZoom;
        private float zoomVelocity;
        private Vector3 targetPosition;
        private Vector3 panVelocity;
        private bool isInitialized;
        private bool isOrthographic;
        private Plane groundPlane;

        private void Awake()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;

            // Auto-create follow target if not assigned
            if (cameraFollowTarget == null)
            {
                GameObject targetObj = new GameObject("CameraFollowTarget");
                cameraFollowTarget = targetObj.transform;
                cameraFollowTarget.position = transform.position;
            }

            // Setup ground plane for raycast (XZ plane)
            groundPlane = new Plane(Vector3.up, cameraFollowTarget.position);

            // Setup virtual camera
            if (cinemachineCamera != null)
            {
                cinemachineCamera.Follow = cameraFollowTarget;
                isOrthographic = cinemachineCamera.Lens.Orthographic;
                currentZoom = isOrthographic ? cinemachineCamera.Lens.OrthographicSize : cinemachineCamera.Lens.FieldOfView;
            }
            
            targetPosition = cameraFollowTarget.position;
        }

        private void Update()
        {
            if (!isInitialized)
                return;
            
            HandleZoom();
            HandlePan();
            ApplyMovement();
        }

        /// <summary>
        /// Initialize the pan system with boundary objects
        /// </summary>
        public void Initialize(IBoundaryProvider boundaryProvider)
        {
            if (boundaryProvider == null)
            {
                Debug.LogWarning("NPanSystem: No boundary provider given, boundaries disabled");
                useBoundary = false;
                isInitialized = true;
                return;
            }
            
            UpdateBoundary(boundaryProvider);
            isInitialized = true;
            CenterCamera();
        }

        public void UpdateBoundary(IBoundaryProvider boundaryProvider)
        {
            if (boundaryProvider == null)
            {
                Debug.LogWarning("NPanSystem: No boundary provider given, boundaries disabled");
                useBoundary = false;
                return;
            }

            var bounds = boundaryProvider.GetBounds();
            boundaryMin = bounds.min - Vector3.one * boundaryPadding;
            boundaryMax = bounds.max + Vector3.one * boundaryPadding;
        }
        /// <summary>
        /// Center camera on the boundary
        /// </summary>
        public void CenterCamera()
        {
            if (useBoundary)
            {
                Vector3 center = (boundaryMin + boundaryMax) / 2f;
                targetPosition = new Vector3(center.x, cameraFollowTarget.position.y, center.z);
                cameraFollowTarget.position = targetPosition;
            }
        }

        private void HandleZoom()
        {
            if (!enableZoom || cinemachineCamera == null)
                return;

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                currentZoom -= scroll * zoomSpeed;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
            }
            
            // Smooth zoom with Cinemachine 3.0+
            var lens = cinemachineCamera.Lens;
            if (isOrthographic)
            {
                lens.OrthographicSize = Mathf.SmoothDamp(lens.OrthographicSize, currentZoom, ref zoomVelocity, zoomSmoothTime);
            }
            else
            {
                lens.FieldOfView = Mathf.SmoothDamp(lens.FieldOfView, currentZoom, ref zoomVelocity, zoomSmoothTime);
            }
            cinemachineCamera.Lens = lens;
        }

        private void HandlePan()
        {
            if (!enablePan)
                return;

            // Start dragging with middle mouse or right mouse button
            if (Input.GetMouseButtonDown(2) || Input.GetMouseButtonDown(1))
            {
                dragOrigin = GetMouseWorldPosition();
                isDragging = true;
            }

            // Continue dragging
            if ((Input.GetMouseButton(2) || Input.GetMouseButton(1)) && isDragging)
            {
                Vector3 currentMousePos = GetMouseWorldPosition();
                Vector3 delta = dragOrigin - currentMousePos;
                
                // Convert delta to camera-relative movement
                Vector3 cameraRight = targetCamera.transform.right;
                Vector3 cameraUp = targetCamera.transform.up;
                
                Vector3 movement = (cameraRight * delta.x + cameraUp * delta.y) * panSpeed;
                
                targetPosition += movement;
                
                // Update drag origin for smooth dragging
                dragOrigin = currentMousePos;
            }

            // Stop dragging
            if (Input.GetMouseButtonUp(2) || Input.GetMouseButtonUp(1))
            {
                isDragging = false;
            }

            // WASD Keyboard Panning
            if (enableWASDPan)
            {
                Vector3 keyboardInput = Vector3.zero;
                
                if (Input.GetKey(KeyCode.W))
                    keyboardInput += Vector3.up;
                if (Input.GetKey(KeyCode.S))
                    keyboardInput += Vector3.down;
                if (Input.GetKey(KeyCode.A))
                    keyboardInput += Vector3.left;
                if (Input.GetKey(KeyCode.D))
                    keyboardInput += Vector3.right;

                if (keyboardInput.magnitude > 0)
                {
                    // Normalize to prevent faster diagonal movement
                    keyboardInput.Normalize();
                    
                    // Convert to camera-relative movement on XY plane
                    Vector3 cameraRight = targetCamera.transform.right;
                    Vector3 cameraUp = targetCamera.transform.up;
                    
                    // Project camera vectors onto XY plane (ignore Z)
                    cameraRight.z = 0;
                    cameraRight.Normalize();
                    cameraUp.z = 0;
                    cameraUp.Normalize();
                    
                    Vector3 movement = (cameraRight * keyboardInput.x + cameraUp * keyboardInput.y) * (wasdPanSpeed * Time.deltaTime);
                    targetPosition += movement;
                }
            }
        }

        private void ApplyMovement()
        {
            // Clamp target position to boundary BEFORE smoothing
            if (useBoundary)
            {
                ClampPositionToBoundary(ref targetPosition);
            }

            // Smooth movement with SmoothDamp for more natural feel
            cameraFollowTarget.position = Vector3.SmoothDamp(
                cameraFollowTarget.position, 
                targetPosition, 
                ref panVelocity, 
                panSmoothTime,
                Mathf.Infinity,
                Time.deltaTime
            );

            // Clamp actual position to ensure it never goes outside (in case of overshoot)
            if (useBoundary)
            {
                var pos = cameraFollowTarget.position;
                ClampPositionToBoundary(ref pos);
                cameraFollowTarget.position = pos;
            }
        }

        private void ClampPositionToBoundary(ref Vector3 position)
        {
            // Simple world-space clamping based on the actual boundary min/max
            position.x = Mathf.Clamp(position.x, boundaryMin.x, boundaryMax.x);
            position.y = Mathf.Clamp(position.y, boundaryMin.y, boundaryMax.y);
            position.z = Mathf.Clamp(position.z, boundaryMin.z, boundaryMax.z);
        }

        private Vector3 GetMouseWorldPosition()
        {
            Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);
            
            // Create a plane perpendicular to camera forward at the follow target position
            Plane plane = new Plane(targetCamera.transform.forward, cameraFollowTarget.position);
            
            if (plane.Raycast(ray, out float distance))
            {
                return ray.GetPoint(distance);
            }
            
            return cameraFollowTarget.position;
        }

        /// <summary>
        /// Set boundary manually
        /// </summary>
        public void SetBoundary(Vector3 min, Vector3 max, float padding = 2f)
        {
            boundaryMin = min - Vector3.one * padding;
            boundaryMax = max + Vector3.one * padding;
            useBoundary = true;
            isInitialized = true;
        }

        /// <summary>
        /// Disable boundary
        /// </summary>
        public void DisableBoundary()
        {
            useBoundary = false;
        }

        /// <summary>
        /// Set zoom limits
        /// </summary>
        public void SetZoomLimits(float min, float max)
        {
            minZoom = min;
            maxZoom = max;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        }

        /// <summary>
        /// Focus on a specific position
        /// </summary>
        public void FocusOn(Vector3 position, float zoom = -1)
        {
            targetPosition = new Vector3(position.x, position.y, cameraFollowTarget.position.y);
            
            if (zoom > 0 && cinemachineCamera != null)
            {
                currentZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (useBoundary && isInitialized)
            {
                Gizmos.color = Color.yellow;
                Vector3 center = (boundaryMin + boundaryMax) / 2f;
                Vector3 size = boundaryMax - boundaryMin;
                Gizmos.DrawWireCube(center, size);
            }
        }
#endif
    }
}

