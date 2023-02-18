using System.Collections;
using UnityEngine;

public class WeaponCone : WeaponBase
{
    [SerializeField] private GameObject _ConeRef;
    [SerializeField] private float _Speed = 1000f;
    [SerializeField] private float _ChargeDuration = 5f;
    [SerializeField] private float _ActiveSkillDamageMulitplier = 10f;

    private readonly float _Duration = 3f;

    override public int GetID()
    {
        _WeaponID = 3;
        return base.GetID();
    }

    protected override void Awake()
    {
        base.Awake();

        _BaseAttackSpeed = 1f;
        _ActiveSkillCooldown = 30f;

        _ActiveSkill = ChargedAttack();
        _UpperBuffString = "ConeBodyBuff.asset";
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
        cone.SetActive(false);
        if (extraDamageMultiplier > 1f)
        {
            cone.TryGetComponent(out WeaponComponent component);
            if (component != null)
            {
                component.SetExtraDamageMultiplier(extraDamageMultiplier);
            }
        }
        cone.TryGetComponent(out Rigidbody rb);
        if (rb != null)
        {
            cone.SetActive(true);
            rb.AddRelativeForce(new Vector3(0, _Speed, 0));
        }
        Destroy(cone, _Duration);
    }

    protected IEnumerator ChargedAttack()
    {
        GameInstance.GetWeaponManager().ToggleArmAttack(false);
        _IsLocked = true;

        yield return new WaitForSeconds(_ChargeDuration);

        SpawnConeProjectile(_ActiveSkillDamageMulitplier);
        GameInstance.GetWeaponManager().ToggleArmAttack(true);
        _IsLocked = false;
    }
}
