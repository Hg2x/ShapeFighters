using ICKT.ServiceLocator;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerUnit : UnitBase, IDamageable
{
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

        ServiceLocator.Get<WeaponManager>().InitPlayerWeapon();
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
}
