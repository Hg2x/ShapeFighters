using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTestData : MonoBehaviour
{
    [SerializeField] protected WeaponBattleData[] _Data = new WeaponBattleData[Const.MAX_WEAPON_LEVEL];

    [SerializeField] protected string _ActiveBuff = "";
    [SerializeField] protected string[] _SlotPassiveBuffs = new string[Const.MAX_WEAPON_SLOT];
}
