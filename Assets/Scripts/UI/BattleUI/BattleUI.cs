using ICKT.ServiceLocator;
using UnityEngine;

public class BattleUI : UIBase
{
    [SerializeField] private BattleUI_HealthBar _HealthBar;
    [SerializeField] private WeaponSlotWidget _WeaponSlots;
    [SerializeField] private ActiveSkillIcon _ActiveSkillIcon;

    [SerializeField] private PlayerStatusData _PlayerStats;
    private bool _DoneInit = false;

    private void Start()
    {
        _HealthBar.Init(_PlayerStats);
        _WeaponSlots.Init();
        _ActiveSkillIcon.Button.onClick.AddListener(UseActiveSkill);
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
    
    private void UseActiveSkill()
    {
        ServiceLocator.Get<LevelManager>().PlayerUseActiveSkill();
    }
}
