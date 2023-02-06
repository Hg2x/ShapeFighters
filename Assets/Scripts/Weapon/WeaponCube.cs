using UnityEngine;

public class WeaponCube : WeaponBase
{
    [SerializeField] private GameObject _WideCubeRef;
    [SerializeField] private float _Speed = 10f;
    private float _Distance;
    private float _DurationLeft = 0f;
    private GameObject _WideCube;

    public override int GetID()
    {
        _WeaponID = 2;
        return base.GetID();
    }

    protected override void Awake()
    {
        base.Awake();

        AttackSpeed = 1f;

        _WideCube = Instantiate(_WideCubeRef, transform);
        _WideCube.SetActive(false);
    }

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
                    _Distance += _Speed * Time.fixedDeltaTime;
                    Vector3 pos = _Player.transform.position + new Vector3(0, 0, _Distance + 1f);
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

    override protected void UpperBodySkill() 
    {
        _DurationLeft = 0.5f;
    }
}
