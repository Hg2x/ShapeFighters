using System;
using System.Collections;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    protected int _WeaponID;
    protected int _WeaponLevel;
    protected WeaponSlot _CurrentSlot = WeaponSlot.None;
    protected int _BaseDamage = 10;
    protected int _WeaponAmount = 1;
    protected float _ActiveSkillCooldown;
    protected float _ActiveSkillCooldownLeft;
    protected bool _CanUseActiveSkill = false;

    private Coroutine _UseSkillCoroutine;

    // TODO:
    // implement buff system
    // implement active skill

    // head = active skill
    // body = strong buff passive
    // arm = attack
    // lower body = movement related

    // sphere / well-rounded
    // head = gains body sphere body buff for a certain period, 2x if switched back to body while in duration
    // body = buff all stats by a small amount
    // arm = attack
    // lower body = moves faster but slides

    // cube / tank knight
    // head = becomes invincible for a set duration
    // body = buffs defense
    // arm = wide AoE that displaces enemy
    // lower body = cannot move but damage received dereased by half

    // cone / sniper
    // head = active skill, starts charging and stops all attack, after done charging, will release a really strong single target attack
    // body = buffs atk
    // arm = single target dps heavy focused
    // lower body = none for now

    // cylinder / mage
    // head = multi casting, uses arm skill of all equipments for aset duration
    // body = buffs atk
    // arm = attack
    // lower body = none for now

    protected PlayerUnit _Player;

    protected float _AttackSpeed;

    protected virtual void Awake()
    {
        _WeaponID = GetID();
        _WeaponLevel = 1;
    }

    protected void Update()
    {
        if (_ActiveSkillCooldownLeft > 0f)
        {
            _ActiveSkillCooldownLeft -= Time.deltaTime;
        }
    }

    public virtual void Init(PlayerUnit player)
    {
        if (player != null)
        {
            _Player = player;
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

    public void Activate()
    {
        _UseSkillCoroutine = StartCoroutine(LoopUseSkill()); // TODO: check this
        ApplyPassive();
    }

    public void Deactivate()
    {
        StopCoroutine(_UseSkillCoroutine);
        RemovePassive();
    }

    private void UseSkill()
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

    private void ApplyPassive()
    {
        switch (_CurrentSlot)
        {
            case WeaponSlot.Head:
                ApplyHeadPassive();
                break;
            case WeaponSlot.UpperBody:
                ApplyUpperBodyPassive();
                break;
            case WeaponSlot.LowerBody:
                ApplyLowerBodyPassive();
                break;
            case WeaponSlot.Arm:
                ApplyArmPassive();
                break;

            default:
                Debug.LogError("Weapon slot none error");
                break;
        }
    }

    private void RemovePassive()
    {
        switch (_CurrentSlot)
        {
            case WeaponSlot.Head:
                RemoveHeadPassive();
                break;
            case WeaponSlot.UpperBody:
                RemoveUpperBodyPassive();
                break;
            case WeaponSlot.LowerBody:
                RemoveLowerBodyPassive();
                break;
            case WeaponSlot.Arm:
                RemoveArmPassive();
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

    protected virtual void ActiveSkill() 
    {
        if (!_CanUseActiveSkill || _CurrentSlot != WeaponSlot.Head)
        {
            return;
        }
    }

    protected virtual void HeadSkill() { }
    protected virtual void UpperBodySkill() { }
    protected virtual void LowerBodySkill() { }
    protected virtual void ArmSkill() { }

    protected virtual void ApplyHeadPassive() { _CanUseActiveSkill = true; }
    protected virtual void ApplyUpperBodyPassive() { }
    protected virtual void ApplyLowerBodyPassive() { }
    protected virtual void ApplyArmPassive() { }

    protected virtual void RemoveHeadPassive() { _CanUseActiveSkill = false; }
    protected virtual void RemoveUpperBodyPassive() { }
    protected virtual void RemoveLowerBodyPassive() { }
    protected virtual void RemoveArmPassive() { }

    protected IEnumerator LoopUseSkill()
    {
        while (true)
        {
            UseSkill();
            yield return new WaitForSeconds(_AttackSpeed / GameInstance.GetLevelManager().PlayerStatusData.AttackSpeedModifier);
        }
    }
}
