using UnityEngine;

public class WeaponSphere : WeaponBase
{
    [SerializeField] private WeaponComponent _SphereRef;
    private readonly float[] _Radius = new float[10];
    private readonly float[] _Angle = new float[10];
    private float _DurationLeft;
    private readonly WeaponComponent[] _Sphere = new WeaponComponent[10];

    private void FixedUpdate()
    {
        if (_DurationLeft > 0)
        {
            _DurationLeft -= Time.fixedDeltaTime;
            for (int i = 0; i < _BattleData.Amount; i++)
            {
                _Sphere[i].TryGetComponent(out Rigidbody rb);
                if (rb != null)
                {
                    _Sphere[i].gameObject.SetActive(true);

                    _Angle[i] += _BattleData.Speed * Time.fixedDeltaTime;
                    float x = Mathf.Cos(_Angle[i]) * _Radius[i];
                    float z = Mathf.Sin(_Angle[i]) * _Radius[i];
                    Vector3 pos = _PlayerTransform.position + new Vector3(x, 0, z);

                    rb.MovePosition(pos);
                }
            }
        }
        if (_DurationLeft <= 0)
        {
            for (int i = 0; i < _BattleData.Amount; i++)
            {
                _Sphere[i].gameObject.SetActive(false);
            }
        }
    }

    public override void LoadWeaponData(string weaponDataString)
    {
        base.LoadWeaponData(weaponDataString);

        for (int i = 0; i < _Sphere.Length; i++)
        {
            if (_Sphere[i] == null)
            {
                _Sphere[i] = Instantiate(_SphereRef, transform);
            }
            _Sphere[i].SetKnockbackForce(_BattleData.KnockbackForce);
            _Sphere[i].SetSize(_BattleData.Size);
            _Sphere[i].gameObject.SetActive(false);
        }

        InitSphereData();
    }

    protected void InitSphereData()
    {
        int[,] radiusAnglePairs = new int[,]
        {
            {3, 0},
            {5, 0},
            {7, 0},
            {9, 0},
            {3, 180},
            {5, 120},
            {7, 180},
            {3, 90},
            {5, 240},
            {3, 270}
        };

        for (int i = 0; i < _Sphere.Length; i++)
        {
            _Radius[i] = radiusAnglePairs[i, 0];
            _Angle[i] = radiusAnglePairs[i, 1];
        }
    }

    protected override void ArmSkill() 
    {
        base.ArmSkill();
        _DurationLeft = _BattleData.Duration;
    }
}
