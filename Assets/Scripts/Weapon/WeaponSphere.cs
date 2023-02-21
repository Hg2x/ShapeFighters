using UnityEngine;

public class WeaponSphere : WeaponBase
{
    [SerializeField] private WeaponComponent _SphereRef;
    private float _Radius = 5f; // tweak this as well
    private float _Angle;
    private float _DurationLeft;
    private GameObject _Sphere;

    // TODO: scaling multipliers and multiple sphere as level goes up

    private void FixedUpdate()
    {
        if (_DurationLeft > 0)
        {
            _Sphere.TryGetComponent(out Rigidbody rb);
            if (rb != null)
            {
                _Sphere.SetActive(true);

                _Angle += _BattleData.Speed * Time.fixedDeltaTime;
                float x = Mathf.Cos(_Angle) * _Radius;
                float z = Mathf.Sin(_Angle) * _Radius;
                Vector3 pos = _Player.transform.position + new Vector3(x, 0, z);

                rb.MovePosition(pos);
            }
            _DurationLeft -= Time.fixedDeltaTime;
        }
        if (_DurationLeft <= 0)
        {
            _Sphere.SetActive(false);
        }
    }

    public override void LoadWeaponData(string weaponDataString)
    {
        base.LoadWeaponData(weaponDataString);
        _Sphere = Instantiate(_SphereRef.gameObject, transform);
        _Sphere.SetActive(false);
    }

    protected override void ArmSkill() 
    {
        base.ArmSkill();

        _Sphere.TryGetComponent(out WeaponComponent component);
        if (component != null)
        {
            component.SetKnockbackForce(_BattleData.KnockbackForce);
        }
        _DurationLeft = _BattleData.Duration;
    }
}
