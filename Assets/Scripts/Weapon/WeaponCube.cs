using UnityEngine;

public class WeaponCube : WeaponBase
{
    [SerializeField] private GameObject _WideCubeRef;
    [SerializeField] private float _Speed = 10f;
    [SerializeField] private float _KnockbackForce = 5f;
    private float _Distance;
    private float _DurationLeft = 0f;
    private bool _DoAttack;
    private GameObject _WideCube;

    public override int GetID()
    {
        _WeaponID = 2;
        return base.GetID();
    }

    protected override void Awake()
    {
        base.Awake();

        _BaseAttackSpeed = 1f;
        _ActiveSkillCooldown = 60f;
        _ActiveBuffString = "CubeHeadBuff.asset";
        _UpperBuffString = "CubeBodyBuff.asset";
        _LowerBuffString = "CubeLowerBodyBuff.asset";
        _DoAttack = false;

        _WideCube = Instantiate(_WideCubeRef, transform);
        _WideCube.SetActive(false);
        _WideCube.TryGetComponent(out CubeWeaponComponent component);
        if (component != null)
        {
            component.SetKnockbackForce(_KnockbackForce);
        }    
    }

    private void FixedUpdate()
    {
        if (_DoAttack && _DurationLeft <= 0)
        {
            _DurationLeft = 0.5f / _FinalAttackSpeed;
            _DoAttack = false;
        }

        if (_DurationLeft > 0)
        {
            _DurationLeft -= Time.fixedDeltaTime;
            _WideCube.TryGetComponent(out Rigidbody rb);
            if (rb != null)
            {
                if (_DurationLeft > 0)
                {
                    _WideCube.SetActive(true);
                    _Distance += _Speed * Time.fixedDeltaTime;
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
    }

    protected override void ArmSkill()
    {
        base.ArmSkill();
        _DoAttack = true;
    }
}
