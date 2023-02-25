using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void UnitDiedDelegate(UnitBase unit);

[RequireComponent(typeof(Rigidbody))]
public abstract class UnitBase : MonoBehaviour
{
    public event UnitDiedDelegate OnUnitDied; // TODO: maybe do smth about this duplicate, alrdy exists in unitStatusData

    [SerializeField] protected UnitStatusData _UnitData;
    protected Rigidbody _Rigidbody;
    protected const int _TurnSpeedMultiplier = 40;

    protected List<BuffBase> _Buffs = new();

    protected virtual void Awake()
    {
        _UnitData.Init(this);
        _Rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        gameObject.transform.rotation = Quaternion.identity;
    }

    protected virtual void Update()
    {
    }

    protected virtual void OnEnable()
    {
    }

    protected virtual void OnDisable()
    {   
    }

    protected virtual void OnDestroy()
    {
        if (OnUnitDied != null)
            foreach (var d in OnUnitDied.GetInvocationList())
                OnUnitDied -= (d as UnitDiedDelegate);

        _UnitData.UnsubcribeDelegates();
    }

    public void ResetStats()
    {
        _UnitData.ResetData();
    }

    protected virtual void Move(Vector3 inputVector)
    {
        if (_UnitData.IsSlippery)
        {
            SlipperyMove(inputVector);
        }
        else
        {
            _Rigidbody.angularVelocity = Vector3.zero;

            var targetDirection = _UnitData.MoveSpeed * Time.deltaTime * inputVector;
            if (!_UnitData.IsImmobile)
            {
                _Rigidbody.MovePosition(_Rigidbody.transform.position + targetDirection);
            }

            if (targetDirection != Vector3.zero)
            {
                float singleStep = _UnitData.TurnSpeed * _TurnSpeedMultiplier * Time.deltaTime;
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                _Rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, singleStep));
            }
        }
    }

    protected virtual void SlipperyMove(Vector3 inputVector) // TODO: maybe make the rotation chaotic
    {
        var targetDirection = _UnitData.MoveSpeed * Time.deltaTime * inputVector;
        _Rigidbody.AddForce(targetDirection, ForceMode.Impulse);

        float singleStep = _UnitData.TurnSpeed * _TurnSpeedMultiplier * Time.deltaTime;
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, singleStep);
        _Rigidbody.MoveRotation(transform.rotation);
    }

    protected virtual void TakeDamage(int damageTaken)
    {
        _UnitData.TakeDamage(damageTaken);
        if (_UnitData.Health > 0)
        {
            StartCoroutine(TestShowDamagedShader(0.5f));
        }
        else
        {
            OnUnitDied?.Invoke(this);
        }
    }

    protected virtual void OnDeath(UnitBase unit)
    {
        if (unit == this)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator TestShowDamagedShader(float delay)
    {
        foreach (Transform child in transform)
        {
            Material material = child.GetComponent<Renderer>().material;
            if (material != null)
            {
                material.SetInt("_ShowDamaged", 1);
            }
        }

        yield return new WaitForSeconds(delay);

        foreach (Transform child in transform)
        {
            Material material = child.GetComponent<Renderer>().material;
            if (material != null)
            {
                material.SetInt("_ShowDamaged", 0);
            }
        }
    }
}
