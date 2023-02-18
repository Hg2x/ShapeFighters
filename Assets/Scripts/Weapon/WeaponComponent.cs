using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    protected float _ExtraDamageMultiplier = 1f;

    public void SetExtraDamageMultiplier(float multiplier)
    {
        if (multiplier > 1f)
        {
            _ExtraDamageMultiplier = multiplier;
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        transform.parent.GetComponent<WeaponBase>().TryDoDamage(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // TODO: change implementation
        if (transform.parent.GetComponent<WeaponBase>().TryDoDamage(collision.collider))
        {
            Destroy(gameObject);
        }
    }
}
