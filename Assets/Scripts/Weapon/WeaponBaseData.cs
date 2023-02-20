using UnityEngine;

[CreateAssetMenu(fileName = "WeaponBaseData", menuName = "ScriptableObject/WeaponData/BaseData", order = 0)]
public class WeaponBaseData : ScriptableObject
{
    [SerializeField][Min(0)] protected int _WeaponID;
    public int WeaponID
    {
        get { return _WeaponID; }
    }

    [SerializeField][Min(0)] protected int _BaseDamage;
    public int BaseDamage
    {
        get { return _BaseDamage; }
    }

    [SerializeField][Min(0f)][Tooltip("How long weapon stays on field")] protected float _AttackDuration;
    public float AttackDuration
    {
        get { return _AttackDuration; }
    }

    [SerializeField][Min(0f)][Tooltip("Attacks done per second")] protected float _BaseAttackSpeed;
    public float BaseAttackSpeed
    {
        get { return _BaseAttackSpeed; }
    }

    [SerializeField][Min(0f)][Tooltip("Weapon travel speed, if applicable")] protected float _Speed;
    public float Speed
    {
        get { return _Speed; }
    }

    [SerializeField][Tooltip("Enemy knockback when hit")] protected float _KnockbackForce;
    public float KnockbackForce
    {
        get { return _KnockbackForce; }
    }

    [SerializeField][Tooltip("In seconds, for non-buff")] protected float _ActiveSkillDamageMulitplier;
    public float ActiveSkillDamageMulitplier
    {
        get { return _ActiveSkillDamageMulitplier; }
    }

    [SerializeField][Min(0f)][Tooltip("In seconds, for non-buff")] protected float _ActiveSkillDuration;
    public float ActiveSkillDuration
    {
        get { return _ActiveSkillDuration; }
    }

    [SerializeField][Min(0f)][Tooltip("In seconds")] protected float _ActiveSkillCooldown;
    public float ActiveSkillCooldown
    {
        get { return _ActiveSkillCooldown; }
    }

    [SerializeField] protected string _ActiveBuffString = "";
    [SerializeField] protected string _UpperBuffString = "";
    [SerializeField] protected string _LowerBuffString = "";

    public string GetBuffString(WeaponSlot slot)
    {
        // TODO: change active/head buff, dont forget to change it in WeaponBase as well
        switch (slot)
        {
            case WeaponSlot.Head:
                return _ActiveBuffString;
            case WeaponSlot.UpperBody:
                return _UpperBuffString;
            case WeaponSlot.LowerBody:
                return _LowerBuffString;
            default:
                return null;
        }
    }
}
