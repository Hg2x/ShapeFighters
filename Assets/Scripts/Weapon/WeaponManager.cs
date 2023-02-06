using System.Collections;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private WeaponBase[] _WeaponCollection;
    private readonly WeaponBase[] _EquippedWeapons = new WeaponBase[Const.MAX_WEAPON_SLOT]; // 0 = head, 1 = upper body, 2 = lower body, 3 = arm
    private readonly Coroutine[] _WeaponCoroutines = new Coroutine[Const.MAX_WEAPON_SLOT];

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
        //var weaponGO = GetWeapon<WeaponCylinder>();
        var aa = GetWeapon<WeaponCube>();
        //var bb = GetWeapon<WeaponCone>();

        _EquippedWeapons[1] = Instantiate(aa);
        if (_EquippedWeapons[1] != null)
        {
            var player = GameInstance.GetLevelManager().PlayerUnitReference;
            _EquippedWeapons[1].Init(player);
            _EquippedWeapons[1].ChangeSlot(WeaponSlot.UpperBody);
        }

        //_EquippedWeapons[2] = Instantiate(bb);
        //if (_EquippedWeapons[2] != null)
        //{
        //    var player = GameInstance.GetLevelManager().PlayerUnitReference;
        //    _EquippedWeapons[2].Init(player);
        //    _EquippedWeapons[2].ChangeSlot(WeaponSlot.LowerBody);
        //}

        //_EquippedWeapons[3] = Instantiate(weaponGO);
        //if (_EquippedWeapons[3] != null)
        //{
        //    var player = GameInstance.GetLevelManager().PlayerUnitReference;
        //    _EquippedWeapons[3].Init(player);
        //    _EquippedWeapons[3].ChangeSlot(WeaponSlot.Arm);
        //}

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

    private bool CreateAndEquipWeapon(int weaponID, int index)
    {
        var weaponToEquip = GetWeapon(weaponID);
        _EquippedWeapons[index] = Instantiate(weaponToEquip); // TODO: check what happens to memory if there is alrdy a weapon equipped here  previously
        if (_EquippedWeapons[index] != null)
        {
            _EquippedWeapons[index].Init(GameInstance.GetLevelManager().PlayerUnitReference);
            _EquippedWeapons[index].ChangeSlot((WeaponSlot)index);
            ActivateWeapon(index);
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
            ActivateWeapon(i);
        }
    }

    private void ActivateWeapon(int index)
    {
        if (_EquippedWeapons[index] != null)
        {
            _WeaponCoroutines[index] = StartCoroutine(StartWeaponSkill(_EquippedWeapons[index]));
        }
    }

    private void DectivateAllWeapons()
    {
        for (int i = 0; i < Const.MAX_WEAPON_SLOT; ++i)
        {
            if (_WeaponCoroutines[i] != null)
            {
                StopCoroutine(_WeaponCoroutines[i]);
            }
        }
    }

    private IEnumerator StartWeaponSkill(WeaponBase weapon)
    {
        while (true)
        {
            if (weapon != null)
            {
                weapon.UseSkill();
            }
            yield return new WaitForSeconds(weapon.AttackSpeed / GameInstance.GetLevelManager().PlayerStatusData.AttackSpeedModifier);
        }
    }
}
