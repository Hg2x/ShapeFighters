using UnityEngine;

public class CubeWeaponComponent : WeaponComponent
{
    protected float _KnocbackForce;

    public void SetKnockbackForce(float knockbackForce)
    {
        _KnocbackForce = knockbackForce;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.TryGetComponent(out EnemyUnit enemy))
        {
            if (enemy.gameObject.TryGetComponent<Rigidbody>(out var enemyRb))
            {
                Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                Vector3 pointOfContact = other.ClosestPoint(transform.position);
                enemyRb.AddForceAtPosition(knockbackDirection * _KnocbackForce, pointOfContact, ForceMode.Impulse);
            }
        }
    }
}
