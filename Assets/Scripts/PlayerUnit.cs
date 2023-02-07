using UnityEngine;

public delegate void ExpChangedDelegate(int currentExp, int NextLevelExp);
public delegate void LevelUpDelegate(int level);

public class PlayerUnit : UnitBase, IDamageable
{
    public event ExpChangedDelegate OnExpChanged;
    public event LevelUpDelegate OnLevelUp;
    
    private InputHandler _InputHandler;
    private Vector2 _MoveVector;

    protected override void Awake()
    {
        base.Awake();

        _InputHandler = GameInstance.GetInputHandler();
    }

    protected override void Start()
    {
        base.Start();

        GameInstance.GetWeaponManager().InitPlayerWeapon();
    }

    public void FixedUpdate()
    {
        if (_MoveVector != Vector2.zero)
        {
            Move(_MoveVector);
        }
    }

    protected override void Update()
    {
        base.Update();

        _MoveVector = _UnitData.MoveSpeed * _InputHandler.InputMoveVector;
    }

    public void Damage(int damageTaken, DamageSource source)
    {
        if (source != DamageSource.Friendly)
        {
            TakeDamage(damageTaken);
        }
    }

    public void GainExp(int expAmount)
    {
        var leveledUp = _UnitData.GainExp(expAmount);
        OnExpChanged?.Invoke(_UnitData.CurrentExp, _UnitData.NextLevelExp);
        if (leveledUp)
        {
            OnLevelUp?.Invoke(_UnitData.Level);
        }
    }
}
