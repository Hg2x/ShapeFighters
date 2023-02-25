using UnityEngine;

public delegate void HealthChangedDelegate(int currentHealth, int maxHealth);
public delegate void ZeroHealthDelegate();

public delegate void ExpChangedDelegate(int currentExp, int NextLevelExp);
public delegate void LevelUpDelegate(int level);

[CreateAssetMenu(fileName = "UnitStatusData", menuName = "ScriptableObject/UnitStatusData/Unit", order = 0)]
public class UnitStatusData : ScriptableObject
{
    public event HealthChangedDelegate OnHealthChanged;
    public event ZeroHealthDelegate OnZeroHealth;

    public event ExpChangedDelegate OnExpChanged;
    public event LevelUpDelegate OnLevelUp;

    [Header("Base Stats, modify these to tweak stats")]
    [SerializeField][Min(0)] protected int _BaseMaxHealth = 100;
    [SerializeField][Min(0f)] protected float _BaseAttack = 1f;
    [SerializeField][Min(0f)] protected float _BaseAttackSpeed = 1f;
    [SerializeField][Min(0f)] protected float _BaseDefense = 0f;
    [SerializeField][Min(0f)] protected float _BaseSpeed = 5f;

    [Header("Calced Stats, for monitoring purposes only")]
    // TODO: generic SINGLE function to fetch these by value
    [ReadOnlyField] public int Health;
    [ReadOnlyField] public int MaxHealth;
    [ReadOnlyField] public float AttackModifier;
    [ReadOnlyField] public float AttackSpeedModifier;
    [ReadOnlyField] public float DefenseModifer;
    [ReadOnlyField] public float MoveSpeed;
    [ReadOnlyField] public int Level;
    [HideInInspector] public float TurnSpeed { get; protected set; }

    [Header("Unit Buff/Ailement Status")]
    public bool IsInvincible;
    // will need to find a better way to implement these status below
    public bool IsSlippery;
    public bool IsImmobile;

    [ReadOnlyField] public int CurrentExp { get; protected set; }
    [ReadOnlyField] public int NextLevelExp { get; protected set; }

    [Header("Enemy specific")]
    [SerializeField] private float _KilledExpModifier;

    protected UnitBase _Owner;

    public void ModifySetVariable<T>(string variableName, T value, string operation = "") // may want some error logs
    {
        System.Reflection.FieldInfo field = GetType().GetField(variableName);

        if (field == null)
        {
            // not found error
            return;
        }

        if (field.FieldType != typeof(int) && field.FieldType != typeof(float))
        {
            if (field.FieldType == typeof(T))
            {
                field.SetValue(this, value);
            }
            return;
        }

        float floatValue = (float)(object)value;
        float currentValue = (float)field.GetValue(this);
        switch (operation)
        {
            case "+":
                currentValue += floatValue;
                break;
            case "-":
                currentValue -= floatValue;
                break;
            case "*":
                currentValue *= floatValue;
                break;
            case "/":
                currentValue /= floatValue;
                break;
            default:
                break;
        }
        if (typeof(T) == typeof(int))
        {
            field.SetValue(this, (int)currentValue);
        }
        else
        {
            field.SetValue(this, currentValue);
        }
    }

    public void Init(UnitBase owner)
    {
        if (owner == null)
        {
            Debug.LogError("Unit owner is null");
            return;
        }
        _Owner = owner;
        ResetData();
    }

    public void RefreshData()
    {
        
    }

    public void ResetData()
    {
        Health = _BaseMaxHealth;
        MaxHealth = _BaseMaxHealth;
        AttackModifier = _BaseAttack;
        AttackSpeedModifier = _BaseAttackSpeed;
        DefenseModifer = _BaseDefense;
        MoveSpeed = _BaseSpeed;
        TurnSpeed = MoveSpeed * 2;

        Level = 1;

        CurrentExp = 0;
        NextLevelExp = CalcNextLevelExp();

        IsInvincible = false;
        IsSlippery = false;
        IsImmobile = false;
    }

    public void ResetHealth()
    {
        Health = _BaseMaxHealth;
        MaxHealth = _BaseMaxHealth;
    }

    public void TakeDamage(int damageTaken)
    {
        if (!IsInvincible)
        {
            float dmgFloat = damageTaken;
            Health -= (int)(dmgFloat / Mathf.Pow(2, DefenseModifer));
            OnHealthChanged?.Invoke(Health, MaxHealth);

            if (Health <= 0)
            {
                Health = 0;
                OnZeroHealth?.Invoke();
            }
        }
    }

    public void UnsubcribeDelegates()
    {
        if (OnHealthChanged != null)
            foreach (var d in OnHealthChanged.GetInvocationList())
                OnHealthChanged -= (d as HealthChangedDelegate);

        if (OnZeroHealth != null)
            foreach (var d in OnZeroHealth.GetInvocationList())
                OnZeroHealth -= (d as ZeroHealthDelegate);


        if (OnExpChanged != null)
            foreach (var d in OnExpChanged.GetInvocationList())
                OnExpChanged -= (d as ExpChangedDelegate);

        if (OnLevelUp != null)
            foreach (var d in OnLevelUp.GetInvocationList())
                OnLevelUp -= (d as LevelUpDelegate);
    }

    // Player specific below
    private int CalcNextLevelExp()
    {
        return Level * 100;
    }

    public void GainExp(int expAmount)
    {
        CurrentExp += expAmount;
        OnExpChanged?.Invoke(CurrentExp, NextLevelExp);
        if (CurrentExp >= NextLevelExp)
        {
            Level++;
            CurrentExp -= NextLevelExp;
            NextLevelExp = CalcNextLevelExp();
            ResetHealth();
            OnLevelUp?.Invoke(Level);
        }
    }

    //Enemy Specific
    public int GetExpWorth()
    {
        return (int)(10 * _KilledExpModifier);
    }
}