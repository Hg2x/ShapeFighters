using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        transform.parent.GetComponent<WeaponBase>().TryDoDamage(collision);
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
