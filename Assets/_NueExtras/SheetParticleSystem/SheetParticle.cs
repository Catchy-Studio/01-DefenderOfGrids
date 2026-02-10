using UnityEngine;
using System.Collections.Generic;
using _NueCore.Common.NueLogger;
using UnityEngine.Events;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace _NueExtras.SheetParticleSystem
{
    [ExecuteInEditMode]
    public class SheetParticle : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private List<Sprite> sprites = new List<Sprite>();
        [SerializeField] private float fps = 15f;
        [SerializeField] private bool playOnStart = false;
        [SerializeField] private bool destroyOnFinish;
        [SerializeField] private bool hideOnFinish;
        [SerializeField] private bool isLoop;
        [SerializeField] private UnityEvent onFinishedEvent;
        
        private int currentFrameIndex = 0;
        private float frameTimer = 0f;
        private bool isPlaying = false;
        private float frameDuration = 0f;

        public UnityEvent OnFinishedEvent => onFinishedEvent;

        private void OnValidate()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            CalculateFrameDuration();
        }

        private void Start()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            
            ResetParticle();
           
            if (playOnStart && sprites.Count > 0)
            {
                Play();
            }
        }

        private void CalculateFrameDuration()
        {
            if (fps > 0)
            {
                frameDuration = 1f / fps;
            }
            else
            {
                frameDuration = 1f / 15f;
            }
        }

        private void Update()
        {
            if (!isPlaying || sprites.Count == 0)
                return;
            if (!isLoop && _isFinishedOnce)
                return;
            frameTimer += Time.deltaTime;
            if (frameTimer >= frameDuration)
            {
                frameTimer -= frameDuration;
                currentFrameIndex++;
                if (currentFrameIndex >= sprites.Count)
                {
                    Stop();
                    return;
                }

                UpdateSprite();
            }
        }

        public void SetDirection(bool isLeft)
        {
            spriteRenderer.flipX = isLeft;
        }

        [Button]
        public void ResetParticle()
        {
            currentFrameIndex = 0;
            frameTimer = 0;
            isPlaying = false;
            frameDuration = 0;
            _isFinishedOnce = false;
            CalculateFrameDuration();

        }
        
        public void Play()
        {
            if (sprites.Count == 0)
            {
                Debug.LogWarning("SpritesheetAnimator: No sprites assigned!");
                return;
            }

            CalculateFrameDuration();
            isPlaying = true;
            currentFrameIndex = 0;
            frameTimer = 0f;
            UpdateSprite();
        }

        [Button]
        private void PlayOnEditor()
        {
            if (sprites.Count == 0)
            {
                return;
            }

            CalculateFrameDuration();
            isPlaying = true;
            currentFrameIndex = 0;
            frameTimer = 0f;
            UpdateSprite();
        }

        private void Dispose()
        {
            Destroy(gameObject);
        }

        private bool _isFinishedOnce;
        public void Stop()
        {
            isPlaying = false;
            currentFrameIndex = 0;
            frameTimer = 0f;
            UpdateSprite();
            if (!Application.isPlaying)
                return;
            _isFinishedOnce = true;
            onFinishedEvent?.Invoke();
            if (hideOnFinish)
                gameObject.SetActive(false);
            if (destroyOnFinish)
                Dispose();
        }
        [Button]
        public void StopOnEditor()
        {
            isPlaying = false;
            currentFrameIndex = 0;
            frameTimer = 0f;
            UpdateSprite();
        }

        public void SetSprites(List<Sprite> newSprites)
        {
            sprites = new List<Sprite>(newSprites);
            CalculateFrameDuration();
        }

        public void SetFPS(float newFps)
        {
            fps = Mathf.Max(0.1f, newFps);
            CalculateFrameDuration();
        }

        public bool IsPlaying()
        {
            return isPlaying;
        }

        private void UpdateSprite()
        {
            if (spriteRenderer != null && currentFrameIndex >= 0 && currentFrameIndex < sprites.Count)
            {
                spriteRenderer.sprite = sprites[currentFrameIndex];
            }
        }
    }
}





