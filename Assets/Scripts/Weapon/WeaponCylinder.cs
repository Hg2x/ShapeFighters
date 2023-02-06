using UnityEngine;

public class WeaponCylinder : WeaponBase
{
    [SerializeField] private GameObject _CylinderRef;

    private readonly float _Duration = 2f;

    public override int GetID()
    {
        _WeaponID = 4;
        return base.GetID();
    }

    protected override void Awake()
    {
        base.Awake();

        AttackSpeed = 1f;
    }

    protected override void ArmSkill()
    {
        var tempAmount = 3; // change to _WeaponAmount later
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
                cylinderComponent.SetDuration(_Duration);
            }
        }
    }
}
