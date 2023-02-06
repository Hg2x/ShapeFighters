using UnityEngine;

public class BattleUI : UIBase
{
    [SerializeField] private BattleUI_HealthBar _HealthBar;
    [SerializeField] private WeaponSlotWidget _WeaponSlots;

    [SerializeField] private UnitStatusData _PlayerStats;
    private bool _DoneInit = false;

    private void Start()
    {
        _HealthBar.Init(_PlayerStats);
        _WeaponSlots.Init();
        _DoneInit = true;
    }

    public override void Show()
    {
        base.Show();
        if (_DoneInit)
        {
            _HealthBar.RefreshDisplay(_PlayerStats);
            _WeaponSlots.RefreshSlots();
        }
    }
}