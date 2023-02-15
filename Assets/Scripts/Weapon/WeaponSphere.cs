using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class WeaponSphere : WeaponBase
{
    [SerializeField] private GameObject _SphereRef;
    [SerializeField] private float _Speed = 10f;
    [SerializeField] private float _Radius = 5f;
    private float _Angle;
    private float _DurationLeft = 0f;
    private GameObject _Sphere;

    protected BuffBase _BodyBuff;

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

        var request = Addressables.LoadAssetAsync<BuffBase>("SphereBodyBuff.asset");
        request.Completed += op =>
        {
            _BodyBuff = op.Result;
        };
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

        if (_BodyBuff != null)
        {
            _BodyBuff.ApplyBuff();
        }
        else
        {
            Debug.LogError("BodyBuff is null error");
        }
    }

    protected override void RemoveUpperBodyPassive()
    {
        base.RemoveUpperBodyPassive();

        if (_BodyBuff != null)
        {
            _BodyBuff.RemoveBuff();
        }
        else
        {
            Debug.LogError("BodyBuff is null error");
        }
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
