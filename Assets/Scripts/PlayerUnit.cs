using UnityEngine;

public delegate void ExpChangedDelegate(int currentExp, int NextLevelExp);
public delegate void LevelUpDelegate(int level);

[RequireComponent(typeof(PlayerController))]
public class PlayerUnit : UnitBase, IDamageable
{
    public event ExpChangedDelegate OnExpChanged;
    public event LevelUpDelegate OnLevelUp;
    
    private PlayerController _playerController;
    private Vector2 _moveVector;

    protected override void Awake()
    {
        base.Awake();

        _playerController = GetComponent<PlayerController>();
    }

    protected override void Start()
    {
        base.Start();

        _UnitData.IsInvincible = true;
        GameInstance.GetWeaponManager().InitPlayerWeapon();
    }

    public void FixedUpdate()
    {
        if (_moveVector != Vector2.zero)
        {
            Move(_moveVector);
        }
    }

    protected override void Update()
    {
        base.Update();

        _moveVector = _UnitData.MoveSpeed * _playerController.InputMoveVector;
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
