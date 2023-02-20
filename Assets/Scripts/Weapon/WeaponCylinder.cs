using System.Collections;
using UnityEngine;

public class WeaponCylinder : WeaponBase
{
    [SerializeField] private CylinderWeaponComponent _CylinderRef;

    // TODO: scaling multipliers and more cylinder spawns as level goes up

    protected override void Awake()
    {
        base.Awake();

        _ActiveSkill = MulticastAttack();
    }

    protected override void ArmSkill()
    {
        var tempAmount = 3; // TODO: change to _WeaponAmount later
        var enemyPositions = GameInstance.GetLevelManager().GetRandomDifferentEnemyPositions(tempAmount);
        if (tempAmount > enemyPositions.Length)
        {
            tempAmount = enemyPositions.Length;
        }

        for(int i = 0; i < tempAmount; i++)
        {
            var location = enemyPositions[i];
            var cylinder = Instantiate(_CylinderRef, location, Quaternion.identity, transform);
            cylinder.TryGetComponent(out CylinderWeaponComponent cylinderComponent);
            if (cylinderComponent != null)
            {
                cylinderComponent.SetKnockbackForce(_KnockbackForce);
                cylinderComponent.SetVerticalSpeed(_Speed);
                cylinderComponent.SetDuration(_Duration);
            }
        }
    }

    protected IEnumerator MulticastAttack()
    {
        for(int i = 0; i < Const.MAX_WEAPON_SLOT; i++)
        {
            GameInstance.GetWeaponManager().ToggleArmAttack(true, i);
        }
        _IsLocked = true;

        yield return new WaitForSeconds(_ActiveSkillDuration);

        for (int i = 0; i < Const.MAX_WEAPON_SLOT; i++)
        {
            if ((WeaponSlot)i == WeaponSlot.Arm)
            {
                continue;
            }
            GameInstance.GetWeaponManager().ToggleArmAttack(false, i);
        }
        _IsLocked = false;
    }
}
