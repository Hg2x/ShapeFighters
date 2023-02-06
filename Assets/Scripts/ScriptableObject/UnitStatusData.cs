using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatusData", menuName = "ScriptableObject/UnitStatusData", order = 1)]
public class UnitStatusData : ScriptableObject
{
    [Header("Base Stats, modify these to tweak stats")]
    [SerializeField] protected int _BaseMaxHealth = 100;
    [SerializeField] protected float _BaseAttackModifier = 1f;
    [SerializeField] protected float _BaseAttackSpeed = 1f;
    [SerializeField] protected float _BaseSpeed = 5f;

    [Header("Calced Stats, for monitoring purposes only")]
    // TODO: generic SINGLE function to fetch these by value
    public int Health;
    public int MaxHealth;
    public float AttackModifier;
    public float AttackSpeedModifier;
    public int Defense;
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

    public void Init()
    {
        ResetData();
        // TODO: implement a more scalable and dev-friendly way to fetch stats
    }

    public void ResetData()
    {
        Health = _BaseMaxHealth;
        MaxHealth = _BaseMaxHealth;
        AttackModifier = _BaseAttackModifier;
        AttackSpeedModifier = _BaseAttackSpeed;
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