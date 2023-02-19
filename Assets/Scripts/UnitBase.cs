using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void HealthChangedDelegate(int currentHealth, int maxHealth);
public delegate void UnitDiedDelegate(UnitBase unit);

[RequireComponent(typeof(Rigidbody))]
public abstract class UnitBase : MonoBehaviour
{
    public event HealthChangedDelegate OnHealthChanged;
    public event UnitDiedDelegate OnUnitDied;

    [SerializeField] protected UnitStatusData _UnitData;
    protected Rigidbody _Rigidbody;
    [HideInInspector] public Vector3 CurrentDirection { get; protected set; }

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
        // unsubscribe subscribed event, TODO: do unit specific delegates as well
        if (OnHealthChanged != null)
            foreach (var d in OnHealthChanged.GetInvocationList())
                OnHealthChanged -= (d as HealthChangedDelegate);

        if (OnUnitDied != null)
            foreach (var d in OnUnitDied.GetInvocationList())
                OnUnitDied -= (d as UnitDiedDelegate);
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

            float singleStep = _UnitData.TurnSpeed * Time.deltaTime;
            CurrentDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            _Rigidbody.MoveRotation(Quaternion.LookRotation(CurrentDirection));
        }
    }

    protected virtual void SlipperyMove(Vector3 inputVector)
    {
        var targetDirection = _UnitData.MoveSpeed * 1f * Time.deltaTime * inputVector;
        _Rigidbody.AddForce(targetDirection, ForceMode.Impulse);

        float singleStep = _UnitData.TurnSpeed * Time.deltaTime;
        CurrentDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        _Rigidbody.MoveRotation(Quaternion.LookRotation(CurrentDirection));
    }

    protected virtual void TakeDamage(int damageTaken)
    {
        // TODO: move these to UnitStatusData
        if (!_UnitData.IsInvincible)
        {
            float dmgFloat = damageTaken;
            _UnitData.Health -= (int)(dmgFloat / Mathf.Pow(2, _UnitData.DefenseModifer));
            OnHealthChanged?.Invoke(_UnitData.Health, _UnitData.MaxHealth);
            if (_UnitData.Health <= 0)
            {
                _UnitData.Health = 0;
                StopCoroutine(nameof(TestShowDamagedShader));
                OnUnitDied?.Invoke(this);
            }
            else
            {
                StartCoroutine(TestShowDamagedShader(0.5f));
            }
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
