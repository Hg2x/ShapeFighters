using ICKT.ServiceLocator;
using System.Collections.Generic;
using UnityEngine;

public delegate void WeaponSwitchedDelegate(int firstSlotIndex, int secondSlotIndex);

[AutoRegisteredService]
public class WeaponManager : MonoBehaviour, IRegisterable
{
    public event WeaponSwitchedDelegate OnWeaponSwitched;

    private readonly Dictionary<int, WeaponBase> _WeaponDictionary = new();
    private readonly Dictionary<int, string> _IDStringDictionary = new();
    private readonly WeaponBase[] _EquippedWeapons = new WeaponBase[Const.MAX_WEAPON_SLOT]; // 0 = head, 1 = upper body, 2 = lower body, 3 = arm

    // TODO: seperate in-battle and out-of-battle logic

    public bool IsPersistent()
    {
        return true;
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        var dictionary = FunctionLibrary.TryGetAssetSync<IDStringDictionary>("WeaponIDDictionary.asset");
        foreach (var pair in dictionary)
        {
            if (FunctionLibrary.TryGetAssetSync<GameObject>("Weapon" + pair.Value + ".prefab").TryGetComponent(out WeaponBase weapon))
            {
                _WeaponDictionary.Add(pair.Key, weapon);
                _IDStringDictionary.Add(pair.Key, pair.Value);
            }
        }
    }

    public int[] GetEquippedWeaponID()
    {
        int[] weaponID = new int[Const.MAX_WEAPON_SLOT];
        for (int i = 0; i < Const.MAX_WEAPON_SLOT; ++i)
        {
            if (_EquippedWeapons[i] != null)
            {
                weaponID[i] = _EquippedWeapons[i].GetID();
            }
        }
        return weaponID;
    }

    public int GetEmptySlotIndex()
    {
        for (int i = 0; i < Const.MAX_WEAPON_SLOT; ++i)
        {
            if (_EquippedWeapons[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    public void InitPlayerWeapon()
    {
        CreateAndEquipWeapon(2, 3);
        ActivateAllWeapons();
    }

    public void SwitchWeapon(int firstSlot, int secondSlot)
    {
        SwitchWeapon((WeaponSlot)firstSlot, (WeaponSlot)secondSlot);
    }

    public bool GivePlayerWeapon(int weaponID, WeaponSlot slot = WeaponSlot.None)
    {
        // upgrade exisitng weapon if it arldy exists
        for (int i = 0; i < Const.MAX_WEAPON_SLOT; ++i)
        {
            if (_EquippedWeapons[i] != null)
            {
                if (weaponID == _EquippedWeapons[i].GetID())
                {
                    UpgradeWeapon(i);
                    // todo: return false if at max level
                    return true;
                }
            }
        }

        if (slot == WeaponSlot.None)
        {
            var index = GetEmptySlotIndex();
            if (index < 0)
            {
                // cannot upgrade/add
                return false;
            }
            // add new weapon to empty slot
            return CreateAndEquipWeapon(weaponID, index);
        }
        else
        {
            // replaces exisitng weapon with new weapon
            return CreateAndEquipWeapon(weaponID, (int)slot);
        }
    }

    public bool UseActiveSkill()
    {
        if (_EquippedWeapons[0] != null)
        {
            bool returnValue = _EquippedWeapons[0].CanUseActiveSkill();
            _EquippedWeapons[0].UseActiveSkill();
            return returnValue;
        }
        return false;
    }

    public void ToggleArmAttack(bool doAttack, int index = 3)
    {
        if (_EquippedWeapons[index] == null)
        {
            return;
        }

        if (doAttack)
        {
            _EquippedWeapons[index].StartAttacking();
        }
        else
        {
            _EquippedWeapons[index].StopAttacking();
        }
    }

    public bool GetIsSlotLocked(WeaponSlot slot)
    {
        if (_EquippedWeapons[(int)slot] != null)
        {
            return _EquippedWeapons[(int)slot].GetIsLocked();
        }
        return false;
    }

    private bool CreateAndEquipWeapon(int weaponID, int index)
    {
        var weaponToEquip = GetWeapon(weaponID);
        _EquippedWeapons[index] = Instantiate(weaponToEquip); // TODO: check what happens to memory if there is alrdy a weapon equipped here  previously
        if (_IDStringDictionary.TryGetValue(weaponID, out string value))
        {
            _EquippedWeapons[index].LoadWeaponData(value + "Data.asset");
        }
        return OnWeaponEquipped(index);
    }

    private bool OnWeaponEquipped(int index)
    {
        if (_EquippedWeapons[index] != null)
        {
            _EquippedWeapons[index].SetPlayerReference(ServiceLocator.Get<LevelManager>().PlayerUnitReference); // TODO: maybe change name
            _EquippedWeapons[index].ChangeSlot((WeaponSlot)index);
            _EquippedWeapons[index].Activate();
            return true;
        }
        return false;
    }

    private void SwitchWeapon(WeaponSlot firstSlot, WeaponSlot secondSlot)
    {
        // TODO: fix attack cooldown immediately resetting after switching
        DectivateAllWeapons();

        var index1 = (int)firstSlot;
        var index2 = (int)secondSlot;
        FunctionLibrary.SwapElements(_EquippedWeapons, index1, index2);

        // already swapped in array, calling ChangeSlot() left
        if (_EquippedWeapons[index1] != null)
        {
            _EquippedWeapons[index1].ChangeSlot(firstSlot);
        }
        if (_EquippedWeapons[index2] != null)
        {
            _EquippedWeapons[index2].ChangeSlot(secondSlot);
        }

        ActivateAllWeapons();
        OnWeaponSwitched?.Invoke(index1, index2);
    }

    private WeaponBase GetWeapon(int weaponID)
    {
        foreach (var pair in _WeaponDictionary)
        {
            if (pair.Key == weaponID)
            {
                return pair.Value;
            }
        }

        return null;
    }

    private void ActivateAllWeapons()
    {
        for (int i = 0; i < Const.MAX_WEAPON_SLOT; ++i)
        {
            if (_EquippedWeapons[i] != null)
            {
                _EquippedWeapons[i].Activate();
            }
        }
    }

    private void DectivateAllWeapons()
    {
        for (int i = 0; i < Const.MAX_WEAPON_SLOT; ++i)
        {
            if (_EquippedWeapons[i] != null)
            {
                _EquippedWeapons[i].Deactivate();
            }
        }
    }

    private void UpgradeWeapon(int index)
    {
        if (_EquippedWeapons[index] != null)
        {
            _EquippedWeapons[index].LevelUp();
        }
    }
}
