using ICKT.ServiceLocator;
using System.Collections;
using UnityEngine;

public class WeaponCylinder : WeaponBase
{
    [SerializeField] private CylinderWeaponComponent _CylinderRef;
    protected LevelManager _LevelManager;

    // TODO: scaling multipliers and more cylinder spawns as level goes up

    protected override void ArmSkill()
    {
        var tempAmount = 3; // TODO: change to _WeaponAmount later
        if (_LevelManager == null)
        {
            _LevelManager = ServiceLocator.Get<LevelManager>();
        }
        Vector3[] enemyPositions = _LevelManager.GetRandomDifferentEnemyPositions(tempAmount);
        if (enemyPositions?.Length > 0)
        {
            if (tempAmount > enemyPositions.Length)
            {
                tempAmount = enemyPositions.Length;
            }

            for (int i = 0; i < tempAmount; i++)
            {
                var location = enemyPositions[i];
                var cylinder = Instantiate(_CylinderRef, location, Quaternion.identity, transform);
                cylinder.TryGetComponent(out CylinderWeaponComponent cylinderComponent);
                if (cylinderComponent != null)
                {
                    cylinderComponent.SetKnockbackForce(_BattleData.KnockbackForce);
                    cylinderComponent.SetVerticalSpeed(_BattleData.Speed);
                    cylinderComponent.SetDuration(_BattleData.Duration);
                }
            }
        }
    }

    public override void LoadWeaponData(string weaponDataString)
    {
        base.LoadWeaponData(weaponDataString);
        _ActiveSkill = MulticastAttack();
    }

    protected IEnumerator MulticastAttack()
    {
        var weaponManager = ServiceLocator.Get<WeaponManager>();

        for (int i = 0; i < Const.MAX_WEAPON_SLOT; i++)
        {
            weaponManager.ToggleArmAttack(true, i);
        }
        _IsLocked = true;

        yield return new WaitForSeconds(_BattleData.ActiveSkillDuration);

        for (int i = 0; i < Const.MAX_WEAPON_SLOT; i++)
        {
            if ((WeaponSlot)i == WeaponSlot.Arm)
            {
                continue;
            }
            weaponManager.ToggleArmAttack(false, i);
        }
        _IsLocked = false;
    }
}
