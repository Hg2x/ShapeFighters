using ICKT.ServiceLocator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpUI : UIBase
{
    [SerializeField] private WeaponSlotWidget _EquippedSlots;
    [SerializeField] private LevelUpUI_ChoiceSlot[] _ChoiceSlots;

    private readonly int[] _ChoiceID = new int[4];

    public override void Init()
    {
        base.Init();

        _EquippedSlots.Init();
        for (int i = 0; i < _ChoiceID.Length; i++)
        {
            // TODO: use a biased randomizer after having enough weapons and skills to choose from
            _ChoiceSlots[i].LoadWeapon(i + 1);
            _ChoiceID[i] = i + 1;

            var index = i;
            _ChoiceSlots[i].Button.onClick.AddListener(delegate { OnSlotClicked(index); });
        }
        _EquippedSlots.SelectNearestEmptySlot();
    }

    public override void Show()
    {
        base.Show();

        _EquippedSlots.RefreshSlots();
        _EquippedSlots.SelectNearestEmptySlot();
    }

    private void OnSlotClicked(int index)
    {
        if (ServiceLocator.Get<LevelManager>().GivePlayerSkill(_ChoiceID[index], _EquippedSlots.GetSelectedSlot()))
        {
            GameInstance.ResumeGame();
            Close();
        }
    }
}
