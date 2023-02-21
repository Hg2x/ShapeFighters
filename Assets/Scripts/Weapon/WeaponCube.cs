using UnityEngine;

public class WeaponCube : WeaponBase
{
    [SerializeField] private GameObject _WideCubeRef;
    private float _Distance;
    private float _DurationLeft;
    private bool _DoAttack;
    private GameObject _WideCube;

    // TODO: scaling multipliers and multiple cube in 4 directions as level goes up

    private void FixedUpdate()
    {
        if (_DurationLeft > 0)
        {
            _DurationLeft -= Time.fixedDeltaTime;
            _WideCube.TryGetComponent(out Rigidbody rb);
            if (rb != null)
            {
                if (_DurationLeft > 0)
                {
                    _WideCube.SetActive(true);
                    _Distance += _BattleData.Speed * Time.fixedDeltaTime;
                    Vector3 pos = _Player.transform.position + new Vector3(0, 0, _Distance * _FinalAttackSpeed + 1f);
                    rb.MovePosition(pos);
                }
                else
                {
                    _WideCube.SetActive(false);
                    _Distance = 0f;
                    rb.MovePosition(_Player.transform.position + new Vector3(0, 0, 1f));
                }
            }
        }
        else
        {
            _WideCube.SetActive(false);
            if (_DoAttack)
            {
                _DurationLeft = _BattleData.Duration / _FinalAttackSpeed;
                _DoAttack = false;
            }
        }
    }

    public override void LoadWeaponData(string weaponDataString)
    {
        base.LoadWeaponData(weaponDataString);
        _WideCube = Instantiate(_WideCubeRef, transform);
        _WideCube.TryGetComponent(out CubeWeaponComponent component);
        if (component != null)
        {
            component.SetKnockbackForce(_BattleData.KnockbackForce);
        }
    }

    protected override void ArmSkill()
    {
        base.ArmSkill();
        _DoAttack = true;
    }
}
