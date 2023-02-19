using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatusData", menuName = "ScriptableObject/UnitStatusData/Unit", order = 0)]
public class UnitStatusData : ScriptableObject
{
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
        // TODO: implement a more scalable and dev-friendly way to fetch stats
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

    // Player specific below
    private int CalcNextLevelExp()
    {
        return Level * 100;
    }

    public bool GainExp(int expAmount) // return if leveled up, doesnt work when 1x exp gain gains more than 1 level
    {
        CurrentExp += expAmount;
        if (CurrentExp >= NextLevelExp)
        {
            Level++;
            CurrentExp -= NextLevelExp;
            NextLevelExp = CalcNextLevelExp();
            ResetHealth();
            return true;
        }
        return false;
    }



    //Enemy Specific
    public int GetExpWorth()
    {
        return (int)(10 * _KilledExpModifier);
    }
}