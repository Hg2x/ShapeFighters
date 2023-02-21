using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "WeaponBaseData", menuName = "ScriptableObject/WeaponData/BaseData", order = 0)]
public class WeaponBaseData : ScriptableObject
{
    [SerializeField][Min(0)] protected int _WeaponID;
    public int WeaponID
    {
        get { return _WeaponID; }
    }

    [SerializeField] protected WeaponBattleData[] _Data = new WeaponBattleData[Const.MAX_WEAPON_LEVEL];

    [SerializeField] protected string _ActiveBuff = "";
    [SerializeField] protected string[] _SlotPassiveBuffs = new string[Const.MAX_WEAPON_SLOT];

    public WeaponBattleData GetWeaponBattleData(int level)
    {
        if (level < 0 || level >= _Data.Length)
        {
            Debug.LogError("WeaponBaseData.GetWeaponBattleData: level out of range");
            return new WeaponBattleData();
        }
        return _Data[level];
    }

    public string GetBuffString(WeaponSlot slot)
    {
        int index = (int)slot;
        if (index < 0 || index >= _SlotPassiveBuffs.Length)
        {
            Debug.LogError("WeaponBaseData.GetBuffString: slot out of range");
            return null;
        }
        return _SlotPassiveBuffs[index];
    }

    public string GetActiveBuff()
    {
        return _ActiveBuff;
    }

    public string GetSlotPassiveBuff(WeaponSlot slot)
    {
        var index = (int)slot;
        if (index < 0 || index >= _SlotPassiveBuffs.Length)
        {
            Debug.LogError("WeaponBaseData.GetSlotPassiveBuff: slot out of range");
            return null;
        }
        return _SlotPassiveBuffs[index];
    }

    public void FillBattleDataGap()
    {
        FunctionLibrary.FillZeroMemberFields(_Data);
    }
}

[System.Serializable]
public class WeaponBattleData
{
    public int Damage;
    [Min(0)] public int Amount;
    [Min(0f)] public float Size;
    [Min(0f)] public float Duration;
    [Min(0f)] public float Frequency;
    [Min(0f)] public float Speed;
    public float KnockbackForce;
    [Min(0f)] public float ActiveSkillDamageMulitplier;
    [Min(0f)] public float ActiveSkillDuration;
    [Min(0f)] public float ActiveSkillCooldown;
}

[CustomEditor(typeof(WeaponBaseData))]
public class FillWeaponBattleDataButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WeaponBaseData weaponData = (WeaponBaseData)target;

        GUILayout.BeginVertical();
        if (GUILayout.Button("Automatically fill Data that is 0"))
        {
            weaponData.FillBattleDataGap();
        }
        GUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }
}
