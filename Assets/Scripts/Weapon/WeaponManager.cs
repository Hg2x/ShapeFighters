using UnityEngine;

public delegate void WeaponSwitchedDelegate(int firstSlotIndex, int secondSlotIndex);

public class WeaponManager : MonoBehaviour
{
    public event WeaponSwitchedDelegate OnWeaponSwitched;

    [SerializeField] private WeaponBase[] _WeaponCollection;
    private readonly WeaponBase[] _EquippedWeapons = new WeaponBase[Const.MAX_WEAPON_SLOT]; // 0 = head, 1 = upper body, 2 = lower body, 3 = arm

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
        var initialWeapon = GetWeapon<WeaponCube>();

        int i = 3;
        _EquippedWeapons[i] = Instantiate(initialWeapon);
        if (_EquippedWeapons[i] != null)
        {
            var player = GameInstance.GetLevelManager().PlayerUnitReference;
            _EquippedWeapons[i].Init(player);
            _EquippedWeapons[i].ChangeSlot((WeaponSlot)i);
        }

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
                    // upgrade weapon.
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

    public void UseActiveSkill()
    {
        if (_EquippedWeapons[0] != null)
        {
            _EquippedWeapons[0].UseActiveSkill();
        }
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
        return OnWeaponEquipped(index);
    }

    private bool OnWeaponEquipped(int index)
    {
        if (_EquippedWeapons[index] != null)
        {
            _EquippedWeapons[index].Init(GameInstance.GetLevelManager().PlayerUnitReference); // TODO: maybe change name
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

    private T GetWeapon<T>() where T : WeaponBase
    {
        foreach (var weapon in _WeaponCollection)
        {
            if (weapon is T weaponType)
            {
                return weaponType;
            }
        }

        return null;
    }

    private WeaponBase GetWeapon(int weaponID)
    {
        foreach (var weapon in _WeaponCollection)
        {
            Debug.Log(weapon.GetID());
            if (weapon.GetID() == weaponID)
            {
                return weapon;
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
}
