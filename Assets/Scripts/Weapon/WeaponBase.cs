using System.Collections;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    protected int _WeaponID;
    protected int _WeaponLevel;
    protected WeaponSlot _CurrentSlot = WeaponSlot.None;
    protected int _BaseDamage;
    protected float _ActiveSkillCooldown;
    protected float _ActiveSkillCooldownLeft;
    protected bool _CanUseActiveSkill = false;
    protected float _BaseAttackSpeed;
    protected float _FinalAttackSpeed;
    protected bool _IsLocked = false; // TODO: broadcast event for IsLocked

    protected IEnumerator _ActiveSkill;
    protected BuffBase _ActiveSkillBuff;
    protected BuffBase _UpperBodyBuff;
    protected BuffBase _LowerBodyBuff;
    protected WeaponBaseData _WeaponData;

    private IEnumerator _AttackCoroutine;

    // TODO:
    // double buff system
    // double check weapons

    // head = active skill
    // body = strong buff passive
    // arm = attack
    // lower body = movement related

    protected PlayerUnit _Player;

    protected virtual void Awake()
    {
        _WeaponLevel = 1;
    }

    protected virtual void Start()
    {
        
    }

    protected void Update()
    {
        if (_ActiveSkillCooldownLeft > 0f)
        {
            _ActiveSkillCooldownLeft -= Time.deltaTime;
        }
    }

    public virtual void LoadWeaponData(string weaponDataString)
    {
        _AttackCoroutine = LoopArmAttack();

        // may want to remove _WeaponData OR remove redundant variables
        _WeaponData = FunctionLibrary.TryGetAssetSync<WeaponBaseData>(weaponDataString);
        if (_WeaponData == null)
        {
            return;
        }

        _WeaponID = _WeaponData.WeaponID;
        _BaseDamage = _WeaponData.BaseDamage;
        _BaseAttackSpeed = _WeaponData.BaseAttackSpeed;
        _ActiveSkillCooldown = _WeaponData.ActiveSkillCooldown;
        _BaseAttackSpeed = _WeaponData.BaseAttackSpeed;
        // may want to not use addresables for these?
        _ActiveSkillBuff = FunctionLibrary.TryGetAssetSync<BuffBase>(_WeaponData.GetBuffString(WeaponSlot.Head));
        _UpperBodyBuff = FunctionLibrary.TryGetAssetSync<BuffBase>(_WeaponData.GetBuffString(WeaponSlot.UpperBody));
        _LowerBodyBuff = FunctionLibrary.TryGetAssetSync<BuffBase>(_WeaponData.GetBuffString(WeaponSlot.LowerBody));
    }

    public virtual void SetPlayerReference(PlayerUnit player)
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
        if (_CurrentSlot == WeaponSlot.Arm)
        {
            StartAttacking(); // TODO: check this
        }
        ApplyPassive();
    }

    public void Deactivate()
    {
        StopAttacking();
        RemovePassive();
    }

    public void StartAttacking()
    {
        StartCoroutine(_AttackCoroutine);
    }

    public void StopAttacking()
    {
        StopCoroutine(_AttackCoroutine);
    }

    public bool GetIsLocked()
    {
        return _IsLocked;
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

    public virtual bool TryDoDamage(Collider collision, float extraDmgMultiplier = 1f)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable damageableObject))
        {
            damageableObject.Damage(CalculateOutgoingDamage(extraDmgMultiplier), DamageSource.Friendly);
            Debug.Log("damaged enemy by" + CalculateOutgoingDamage());
            return true;
        }
        return false;
    }

    protected virtual int CalculateOutgoingDamage(float extraDmgMultiplier = 1f)
    {
        var atkMod = GameInstance.GetLevelManager().PlayerStatusData.AttackModifier;
        return (int)(_BaseDamage * atkMod * extraDmgMultiplier) ;
    }

    public virtual void UseActiveSkill() 
    {
        if (CanUseActiveSkill())
        {
            _ActiveSkillCooldownLeft = _ActiveSkillCooldown;
            if (_ActiveSkillBuff != null)
            {
                StartCoroutine(_ActiveSkillBuff.ApplyBuff());
            }
            if (_ActiveSkill != null)
            {
                StartCoroutine(_ActiveSkill);
            }
        }
    }

    protected bool CanUseActiveSkill()
    {
        return _CanUseActiveSkill && _ActiveSkillCooldownLeft <= 0f;
    }

    protected virtual void HeadSkill() { }
    protected virtual void UpperBodySkill() { }
    protected virtual void LowerBodySkill() { }
    protected virtual void ArmSkill() { }

    protected virtual void ApplyHeadPassive() { _CanUseActiveSkill = true; }
    protected virtual void ApplyUpperBodyPassive() 
    {
        if (_UpperBodyBuff != null)
        {
            StartCoroutine(_UpperBodyBuff.ApplyBuff());
        }
    }
    protected virtual void ApplyLowerBodyPassive() 
    {
        if (_LowerBodyBuff != null)
        {
            StartCoroutine(_LowerBodyBuff.ApplyBuff());
        }
    }
    protected virtual void ApplyArmPassive() { }

    protected virtual void RemoveHeadPassive() { _CanUseActiveSkill = false; }
    protected virtual void RemoveUpperBodyPassive() 
    {
        if (_UpperBodyBuff != null)
        {
            _UpperBodyBuff.RemoveBuff();
        }
    }
    protected virtual void RemoveLowerBodyPassive() 
    {
        if (_LowerBodyBuff != null)
        {
            _LowerBodyBuff.RemoveBuff();
        }
    }
    protected virtual void RemoveArmPassive() { }

    protected IEnumerator LoopArmAttack() // do smth about this when not in arm
    {
        while (true)
        {
            ArmSkill();
            _FinalAttackSpeed = _BaseAttackSpeed * GameInstance.GetLevelManager().PlayerStatusData.AttackSpeedModifier;
            if (_FinalAttackSpeed <= 0f)
            {
                Debug.LogError("FinalAttackSpeed cannot be 0 or less");
                _FinalAttackSpeed = 1f;
            }
            yield return new WaitForSeconds(1 / _FinalAttackSpeed);
        }
    }
}
