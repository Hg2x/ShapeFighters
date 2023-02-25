using ICKT.ServiceLocator;
using System.Collections;
using UnityEngine;

public class WeaponCone : WeaponBase
{
    [SerializeField] private WeaponComponent _ConeRef;

    // TODO: scaling multipliers and faster cone spawns as level goes up

    protected override void ArmSkill() 
    {
        SpawnConeProjectile();
    }

    protected void SpawnConeProjectile(float extraDamageMultiplier = 1f)
    {
        var position = _PlayerTransform.localPosition + Vector3.Scale(_PlayerTransform.forward, new Vector3(3f, 0.5f, 3f));
        var rotation = Quaternion.Euler(new Vector3(90f, 0f, -_PlayerTransform.rotation.eulerAngles.y));

        var cone = Instantiate(_ConeRef, position, rotation, transform);
        cone.gameObject.SetActive(false);
        cone.TryGetComponent(out WeaponComponent component);
        if (component != null)
        {
            component.SetKnockbackForce(_BattleData.KnockbackForce);
            if (extraDamageMultiplier > 1f)
            {
                component.SetExtraDamageMultiplier(extraDamageMultiplier);
            }
        }

        cone.TryGetComponent(out Rigidbody rb);
        if (rb != null)
        {
            cone.gameObject.SetActive(true);
            rb.AddRelativeForce(new Vector3(0, _BattleData.Speed, 0));
        }
        Destroy(cone, _BattleData.Duration);
    }

    public override void LoadWeaponData(string weaponDataString)
    {
        base.LoadWeaponData(weaponDataString);
        _ActiveSkill = ChargedAttack();
    }

    protected IEnumerator ChargedAttack()
    {
        var weaponManager = ServiceLocator.Get<WeaponManager>();

        weaponManager.ToggleArmAttack(false);
        _IsLocked = true;

        yield return new WaitForSeconds(_BattleData.ActiveSkillDuration);

        SpawnConeProjectile(_BattleData.ActiveSkillDamageMulitplier);
        weaponManager.ToggleArmAttack(true);
        _IsLocked = false;
    }
}
