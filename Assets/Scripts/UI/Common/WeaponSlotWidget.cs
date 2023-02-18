using UnityEngine;

public class WeaponSlotWidget : MonoBehaviour
{
    [SerializeField] private CommonItemIcon[] _WeaponIcons;
    private int _SelectedIndex = -1;

    public void Init()
    {
        int[] weaponID = GameInstance.GetWeaponManager().GetEquippedWeaponID();
        for(int i = 0; i < _WeaponIcons.Length; i++)
        {
            _WeaponIcons[i].LoadWeaponImage((weaponID[i]));
            var index = i;
            _WeaponIcons[i].Button.onClick.AddListener(delegate { OnSlotClicked(index); } ); // TODO: check this later
        }
    }

    public void RefreshSlots()
    {
        int[] weaponID = GameInstance.GetWeaponManager().GetEquippedWeaponID();
        for (int i = 0; i < _WeaponIcons.Length; i++)
        {
            _WeaponIcons[i].LoadWeaponImage((weaponID[i]));
        }
    }

    public WeaponSlot GetSelectedSlot()
    {
        return (WeaponSlot)_SelectedIndex;
    }

    public bool SlotIsSelected()
    {
        return _SelectedIndex >= 0;
    }

    public void SelectNearestEmptySlot()
    {
        var index = GameInstance.GetWeaponManager().GetEmptySlotIndex();
        if (index >= 0)
        {
            foreach (var slot in _WeaponIcons)
            {
                slot.Unselected();
            }

            _SelectedIndex = index;
            _WeaponIcons[_SelectedIndex].Selected();
        }
    }

    private void OnSlotClicked(int index)
    {
        if (GameInstance.GetWeaponManager().GetIsSlotLocked((WeaponSlot)index))
        {
            return;
        }

        if (SlotIsSelected())
        {
            if (_SelectedIndex == index)
            {
                // unselect
                _WeaponIcons[_SelectedIndex].Unselected();
            }
            else
            {
                _WeaponIcons[_SelectedIndex].Unselected();

                var weaponManager = GameInstance.GetWeaponManager();
                int[] weaponID = weaponManager.GetEquippedWeaponID();

                weaponManager.SwitchWeapon(_SelectedIndex, index);
                _WeaponIcons[_SelectedIndex].LoadWeaponImage((weaponID[index]));
                _WeaponIcons[index].LoadWeaponImage((weaponID[_SelectedIndex]));
            }
            _SelectedIndex = -1;
        }
        else
        {
            _SelectedIndex = index;
            _WeaponIcons[_SelectedIndex].Selected();
        }
    }
}
