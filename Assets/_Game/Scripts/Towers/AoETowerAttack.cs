using System.Collections;
using UnityEngine;

public class AoETowerAttack : MonoBehaviour, ITowerAttackBehaviour
{
    [SerializeField] private Transform _auraVisual;
    private TowerController _owner;
    private Collider2D[] _results = new Collider2D[50]; // Pre-allocated buffer to store hits
    private float _tickInterval = 0.25f; // Check for enemies 4 times per second

    private void Awake()
    {
        if (_auraVisual == null)
        {
            _auraVisual = transform.Find("AuraVisual");
        }

        if (_auraVisual != null)
        {
            if (!_auraVisual.TryGetComponent<SpriteRenderer>(out var sr))
            {
                sr = _auraVisual.gameObject.AddComponent<SpriteRenderer>();
            }

            sr.color = new Color(1f, 0f, 0f, 0.3f);
            sr.sortingOrder = 50;
        }
    }

    public void Initialize(TowerController owner)
    {
        _owner = owner;
        StartCoroutine(AuraDamageRoutine());
    }

    public void Tick(float deltaTime)
    {
        // Damage is applied via Coroutine at a controlled tick rate
    }

    private IEnumerator AuraDamageRoutine()
    {
        SpriteRenderer visualSR = _auraVisual != null ? _auraVisual.GetComponent<SpriteRenderer>() : null;

        while (true)
        {
            if (_owner == null) yield break;

            float currentRadius = _owner.GetTotalRange();
            
            // Correct Visual Scale (Visual bounds matched to Physics diameter)
            if (visualSR != null && visualSR.sprite != null)
            {
                float spriteBaseSizeX = visualSR.sprite.bounds.size.x;
                // Avoid divide by zero
                if (spriteBaseSizeX > 0.01f)
                {
                    float targetScale = (currentRadius * 2f) / spriteBaseSizeX;
                    _auraVisual.localScale = new Vector3(targetScale, targetScale, 1f);
                }
            }
            else if (_auraVisual != null)
            {
                // Fallback if no sprite assigned
                _auraVisual.localScale = new Vector3(currentRadius * 2f, currentRadius * 2f, 1f);
            }

            float baseDamage = _owner.Data.damagePerSecond;
            float damageBoostPercent = _owner.GetDamageBoost();
            
            float totalDPS = baseDamage + (baseDamage * damageBoostPercent / 100f);
            float damageToApply = totalDPS * _tickInterval;

            // Strict Performance: OverlapCircleNonAlloc with cached buffer
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, currentRadius, _results, _owner.EnemyLayer);

            for (int i = 0; i < count; i++)
            {
                if (_results[i].TryGetComponent<EnemyHealth>(out var health))
                {
                    health.TakeDamage(damageToApply);
                }
            }

            yield return new WaitForSeconds(_tickInterval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_owner == null && TryGetComponent<TowerController>(out var c)) _owner = c;
        if (_owner == null) return;

        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, _owner.GetTotalRange());
    }
}
