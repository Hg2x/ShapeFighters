using ICKT.ServiceLocator;
using UnityEngine;
using UnityEngine.VFX;

public delegate void ExpChangedDelegate(int currentExp, int NextLevelExp);
public delegate void LevelUpDelegate(int level);

public delegate void MoveDelegate(Vector3 inputVector);

public class PlayerUnit : UnitBase, IDamageable
{
    public event ExpChangedDelegate OnExpChanged;
    public event LevelUpDelegate OnLevelUp;
    public MoveDelegate MoveDelegate;

    protected VisualEffect _ActiveSkillVFX;
    private InputHandler _InputHandler;

    protected override void Awake()
    {
        base.Awake();

        _InputHandler = ServiceLocator.Get<InputHandler>();
        _ActiveSkillVFX = GetComponentInChildren<VisualEffect>();
        _ActiveSkillVFX.Stop();
    }

    protected override void Start()
    {
        base.Start();

        GameInstance.GetWeaponManager().InitPlayerWeapon();
    }

    protected override void Update()
    {
        base.Update();

        Vector3 inputVector3 = new(_InputHandler.InputMoveVector.x, 0f, _InputHandler.InputMoveVector.y);
        Move(inputVector3);
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
