using UnityEngine;

public class WeaponSphere : WeaponBase
{
    [SerializeField] private GameObject _SphereRef;
    [SerializeField] private float _Speed = 10f;
    [SerializeField] private float _Radius = 5f;
    private float _Angle;
    private float _DurationLeft = 0f;
    private GameObject _Sphere;

    public override int GetID()
    {
        _WeaponID = 1;
        return base.GetID();
    }

    protected override void Awake()
    {
        base.Awake();

        _AttackSpeed = 1f;
        _ActiveSkillCooldown = 60f;

        _Sphere = Instantiate(_SphereRef, transform);
        _Sphere.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (_DurationLeft > 0)
        {
            _Sphere.TryGetComponent(out Rigidbody rb);
            if (rb != null)
            {
                _Sphere.SetActive(true);

                _Angle += _Speed * Time.fixedDeltaTime;
                float x = Mathf.Cos(_Angle) * _Radius;
                float z = Mathf.Sin(_Angle) * _Radius;
                Vector3 pos = _Player.transform.position + new Vector3(x, 0, z);

                rb.MovePosition(pos);
            }

            _DurationLeft -= Time.fixedDeltaTime;
            if (_DurationLeft <= 0)
            {
                _Sphere.SetActive(false);
            }
        }
    }

    protected override void HeadSkill() 
    {
        base.HeadSkill();
        _DurationLeft = 0.5f;
    }

    protected override void ApplyUpperBodyPassive()
    {
        base.ApplyUpperBodyPassive();

        // implement as buff later
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("AttackModifier", 0.2f, "+");
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("DefenseModifier", 0.2f, "+");
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("AttackSpeedModifier", 0.2f, "+");
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("MoveSpeed", 1f, "+");
    }

    protected override void RemoveUpperBodyPassive()
    {
        base.RemoveUpperBodyPassive();

        // implement as buff later
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("AttackModifier", -0.2f, "+");
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("DefenseModifier", -0.2f, "+");
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("AttackSpeedModifier", -0.2f, "+");
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("MoveSpeed", -1f, "+");
    }

    protected override void ApplyLowerBodyPassive()
    {
        base.ApplyLowerBodyPassive();

        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("MoveSpeed", 5f, "+");
        // but slides
    }

    protected override void RemoveLowerBodyPassive()
    {
        base.RemoveLowerBodyPassive();

        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("MoveSpeed", -5f, "+");
        //but slides
    }
}
