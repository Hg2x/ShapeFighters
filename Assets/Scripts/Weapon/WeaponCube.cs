using UnityEngine;

public class WeaponCube : WeaponBase
{
    [SerializeField] private CubeWeaponComponent _WideCubeRef;
    private float _Distance;
    private float _DurationLeft;
    private bool _DoAttack;
    private readonly CubeWeaponComponent[] _WideCubes = new CubeWeaponComponent[4];

    private void FixedUpdate()
    {
        if (_DurationLeft > 0)
        {
            _DurationLeft -= Time.fixedDeltaTime;
            for (int i = 0; i < _BattleData.Amount; i++)
            {
                if (_WideCubes[i].TryGetComponent<Rigidbody>(out var rb))
                {
                    if (_DurationLeft > 0)
                    {
                        _WideCubes[i].gameObject.SetActive(true);
                        _Distance += _BattleData.Speed * Time.fixedDeltaTime;

                        Vector3 pos = _PlayerTransform.position;
                        switch (i)
                        {
                            case 0:
                                pos += new Vector3(0, 0, _Distance * _FinalAttackSpeed + 1f);
                                break;
                            case 1:
                                pos += new Vector3(-(_Distance * _FinalAttackSpeed + 1f), 0, 0);
                                break;
                            case 2:
                                pos += new Vector3((_Distance * _FinalAttackSpeed + 1f), 0, 0);
                                break;
                            case 3:
                                pos += new Vector3(0, 0, -(_Distance * _FinalAttackSpeed + 1f));
                                break;
                        }

                        rb.MovePosition(pos);
                    }
                    else
                    {
                        _WideCubes[i].gameObject.SetActive(false);
                        _Distance = 0f;
                        rb.MovePosition(_PlayerTransform.position + new Vector3(0, 0, 1f));
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < _BattleData.Amount; i++)
            {
                _WideCubes[i].gameObject.SetActive(false);
            }

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
        for (int i = 0; i < _WideCubes.Length; i++)
        {
            if (_WideCubes[i] == null)
            {
                _WideCubes[i] = Instantiate(_WideCubeRef, transform);
            }
            _WideCubes[i].SetKnockbackForce(_BattleData.KnockbackForce);
            _WideCubes[i].SetSize(_BattleData.Size);
            _WideCubes[i].gameObject.SetActive(false);
            if (i == 1 || i == 2)
            {
                _WideCubes[i].transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            }
        }
    }

    protected override void OnLevelUp()
    {
        base.OnLevelUp();

        for (int i = 0; i < _WideCubes.Length; i++)
        {
            _WideCubes[i].SetKnockbackForce(_BattleData.KnockbackForce);
            _WideCubes[i].SetSize(_BattleData.Size);
        }
    }

    protected override void ArmSkill()
    {
        base.ArmSkill();
        _DoAttack = true;
    }
}
