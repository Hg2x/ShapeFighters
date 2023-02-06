using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    protected int _WeaponID;
    protected int _WeaponLevel;
    protected WeaponSlot _CurrentSlot;
    protected int _BaseDamage = 10;
    protected int _WeaponAmount = 1;

    protected PlayerUnit _Player;

    public float AttackSpeed { get; protected set; }

    protected virtual void Awake()
    {
        _WeaponID = GetID();
        _WeaponLevel = 1;
    }

    public virtual void Init(PlayerUnit player)
    {
        if (player != null)
        {
            _Player = player;
            _CurrentSlot = WeaponSlot.None;
        }
        else
        {
            Debug.LogError("failed to init weapon because player is null");
            return;
        }
    }

    public virtual int GetID()
    {
        return _WeaponID;
    }

    public void UseSkill()
    {
        switch (_CurrentSlot)
        {
            case WeaponSlot.Head:
                HeadSkill();
                break;
            case WeaponSlot.UpperBody:
                UpperBodySkill();
                break;
            case WeaponSlot.LowerBody:
                LowerBodySkill();
                break;
            case WeaponSlot.Arm:
                ArmSkill();
                break;

            default:
                Debug.LogError("Weapon slot none error");
                break;
        }
    }

    public void ChangeSlot(WeaponSlot newSlot)
    {
        if (newSlot != WeaponSlot.None)
        {
            _CurrentSlot = newSlot;
        }
    }

    public virtual bool TryDoDamage(Collider collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable damageableObject))
        {
            damageableObject.Damage(CalculateOutgoingDamage(), DamageSource.Friendly);
            Debug.Log("damaged enemy by" + CalculateOutgoingDamage());
            return true;
        }
        return false;
    }

    protected virtual int CalculateOutgoingDamage()
    {
        var atkMod = GameInstance.GetLevelManager().PlayerStatusData.AttackModifier;
        return (int)(_BaseDamage * atkMod) ;
    }

    protected virtual void HeadSkill() { }
    protected virtual void UpperBodySkill() { }
    protected virtual void LowerBodySkill() { }
    protected virtual void ArmSkill() { }

    protected virtual void HeadPassive() { }
    protected virtual void UpperBodyPassive() { }
    protected virtual void LowerBodyPassive() { }
    protected virtual void ArmPassive() { }
}
