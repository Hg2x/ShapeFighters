using UnityEngine;

public class WeaponSphere : WeaponBase
{
    [SerializeField] private GameObject _SphereRef;
    [SerializeField] private float _Speed = 10f;
    [SerializeField] private float _Radius = 5f;
    private float _Angle;
    private float _DurationLeft = 0f;
    private GameObject _Sphere;

    protected override void Awake()
    {
        base.Awake();

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

    protected override void ArmSkill() 
    {
        base.ArmSkill();
        _DurationLeft = 0.5f;
    }
}
