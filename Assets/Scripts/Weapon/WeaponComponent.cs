using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    protected float _KnockbackForce = 0f;
    protected float _ExtraDamageMultiplier = 1f;
    protected float _Size = 1f;
    
    public void SetKnockbackForce(float knockbackForce)
    {
        _KnockbackForce = knockbackForce;
    }

    public void SetExtraDamageMultiplier(float multiplier)
    {
        if (multiplier > 1f)
        {
            _ExtraDamageMultiplier = multiplier;
        }
    }

    public virtual void SetSize(float size)
    {
        if (size > 0f)
        {
            _Size = size;
            transform.localScale = new Vector3(size, size, size);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        transform.parent.GetComponent<WeaponBase>().TryDoDamage(other);
        ApplyKnockbackToEnemy(other);
    }

    protected void OnCollisionEnter(Collision collision)
    {
        // TODO: change implementation
        if (transform.parent.GetComponent<WeaponBase>().TryDoDamage(collision.collider))
        {
            Destroy(gameObject);
        }
        ApplyKnockbackToEnemy(collision.collider);
    }

    protected virtual void ApplyKnockbackToEnemy(Collider other)
    {
        if (_KnockbackForce != 0f)
        {
            if (other.TryGetComponent(out EnemyUnit enemy))
            {
                if (enemy.gameObject.TryGetComponent<Rigidbody>(out var enemyRb))
                {
                    Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                    Vector3 pointOfContact = other.ClosestPoint(transform.position);
                    enemyRb.AddForceAtPosition(knockbackDirection * _KnockbackForce, pointOfContact, ForceMode.Impulse);
                }
            }
        }
    }
}
