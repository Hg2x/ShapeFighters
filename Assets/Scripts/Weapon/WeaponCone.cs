using System.Collections;
using UnityEngine;

public class WeaponCone : WeaponBase
{
    [SerializeField] private WeaponComponent _ConeRef;

    // TODO: scaling multipliers and faster cone spawns as level goes up

    protected override void Awake()
    {
        base.Awake();

        _ActiveSkill = ChargedAttack();
    }

    protected override void ArmSkill() 
    {
        SpawnConeProjectile();
    }

    protected void SpawnConeProjectile(float extraDamageMultiplier = 1f)
    {
        var curDirection = _Player.GetComponent<UnitBase>().CurrentDirection;
        var position = _Player.transform.localPosition + Vector3.Scale(curDirection, new Vector3(3f, 0.5f, 3f));
        var rotation = Quaternion.Euler(new Vector3(curDirection.z * 90f, 0.5f, curDirection.x * -90f));

        var cone = Instantiate(_ConeRef, position, rotation, transform);
        cone.gameObject.SetActive(false);
        cone.TryGetComponent(out WeaponComponent component);
        if (component != null)
        {
            component.SetKnockbackForce(_KnockbackForce);
            if (extraDamageMultiplier > 1f)
            {
                component.SetExtraDamageMultiplier(extraDamageMultiplier);
            }
        }
        
        cone.TryGetComponent(out Rigidbody rb);
        if (rb != null)
        {
            cone.gameObject.SetActive(true);
            rb.AddRelativeForce(new Vector3(0, _Speed, 0));
        }
        Destroy(cone, _Duration);
    }

    protected IEnumerator ChargedAttack()
    {
        GameInstance.GetWeaponManager().ToggleArmAttack(false);
        _IsLocked = true;

        yield return new WaitForSeconds(_ActiveSkillDuration);

        SpawnConeProjectile(_ActiveSkillDamageMulitplier);
        GameInstance.GetWeaponManager().ToggleArmAttack(true);
        _IsLocked = false;
    }
}
