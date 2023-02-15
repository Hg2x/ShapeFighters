using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatusData", menuName = "ScriptableObject/UnitStatusData", order = 1)]
public class UnitStatusData : ScriptableObject
{
    [Header("Base Stats, modify these to tweak stats")]
    [SerializeField] protected int _BaseMaxHealth = 100;
    [SerializeField] protected float _BaseAttack = 1f;
    [SerializeField] protected float _BaseAttackSpeed = 1f;
    [SerializeField] protected float _BaseDefense = 1f;
    [SerializeField] protected float _BaseSpeed = 5f;

    [Header("Calced Stats, for monitoring purposes only")]
    // TODO: generic SINGLE function to fetch these by value
    public int Health;
    public int MaxHealth;
    public float AttackModifier;
    public float AttackSpeedModifier;
    public float DefenseModifer;
    public float MoveSpeed;
    public int Level;
    [HideInInspector] public float TurnSpeed { get; protected set; }

    public bool IsInvincible = false;

    // player specific
    public int CurrentExp { get; protected set; }
    public int NextLevelExp { get; protected set; }
    //

    [Header("Enemy specific")]
    [SerializeField] private float _KilledExpModifier;

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

    public void Init()
    {
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