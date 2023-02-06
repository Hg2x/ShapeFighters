using System.Collections;
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

    protected virtual void Awake()
    {
        _UnitData.Init();
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

    public void RefreshStats()
    {
        _UnitData.ResetHealth();
    }

    protected virtual void Move(Vector2 moveVector)
    {
        _Rigidbody.angularVelocity = Vector3.zero;

        var targetDirection = new Vector3(moveVector.x, 0f, moveVector.y) * Time.fixedDeltaTime;
        _Rigidbody.MovePosition(_Rigidbody.transform.position + targetDirection);

        float singleStep = _UnitData.TurnSpeed * Time.fixedDeltaTime;
        CurrentDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        _Rigidbody.MoveRotation(Quaternion.LookRotation(CurrentDirection));
    }

    protected virtual void TakeDamage(int damageTaken)
    {
        // TODO: move these to UnitStatusData
        if (!_UnitData.IsInvincible)
        {
            _UnitData.Health -= damageTaken;
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
