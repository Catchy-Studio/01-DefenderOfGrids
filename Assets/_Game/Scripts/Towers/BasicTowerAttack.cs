using UnityEngine;

public class BasicTowerAttack : MonoBehaviour, ITowerAttackBehaviour
{
    private TowerController _owner;
    private float _fireCountdown = 0f;

    public void Initialize(TowerController owner)
    {
        _owner = owner;
        _fireCountdown = 1f / _owner.FireRate;
    }

    public void Tick(float deltaTime)
    {
        if (_owner.Target == null || _owner.WeaponPart == null) return;

        Vector3 direction = _owner.Target.position - _owner.WeaponPart.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Rotate ONLY the weapon part.
        _owner.WeaponPart.rotation = Quaternion.Euler(0f, 0f, angle);
        
        if (_fireCountdown <= 0f)
        {
            Shoot();
            _fireCountdown = 1f / _owner.FireRate;
        }

        _fireCountdown -= deltaTime;
    }

    private void Shoot()
    {
        if (_owner.BulletPrefab == null || _owner.FirePoint == null) return;

        GameObject bulletGO = Instantiate(_owner.BulletPrefab, _owner.FirePoint.position, Quaternion.identity);
        if (bulletGO.TryGetComponent<Bullet>(out var bullet))
        {
            bullet.SetDamageBoost(_owner.GetDamageBoost());
        }
        
        bulletGO.SendMessage("Seek", _owner.Target, SendMessageOptions.DontRequireReceiver);
    }
}
