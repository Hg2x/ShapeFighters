using ICKT.ServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI_HealthBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _CurrentHealthText;
    [SerializeField] private TextMeshProUGUI _MaxHealthText;
    [SerializeField] private TextMeshProUGUI _LevelText;

    [SerializeField] private Image _HealthBar;
    [SerializeField] private Image _ExpBar;

    public void Init(PlayerStatusData data)
    {
        if (data != null)
        {
            SubscribeToPlayer(data);
            RefreshDisplay(data);
        }
    }

    private void OnDisable()
    {
        var data = ServiceLocator.Get<LevelManager>().PlayerStatusData;
        if (data != null) // TODO : fix error when not being able to access PlayerUnit
        {
            UnsubscribeToPlayer(data);
        }
    }

    public void RefreshDisplay(UnitStatusData playerStats)
    {
        SetCurrentHealthText(playerStats.Health);
        SetMaxHealthText(playerStats.MaxHealth);
        UpdateExpBar(playerStats.CurrentExp, playerStats.NextLevelExp);
        SetLevelText(playerStats.Level);
    }

    private void SubscribeToPlayer(PlayerStatusData data)
    {
        data.OnHealthChanged += UpdateHealthBar;
        data.OnExpChanged += UpdateExpBar;
        data.OnZeroHealth += OnPlayerDied;
        data.OnLevelUp += SetLevelText;
    }

    private void UnsubscribeToPlayer(PlayerStatusData data)
    {
        data.OnHealthChanged -= UpdateHealthBar;
        data.OnExpChanged -= UpdateExpBar;
        data.OnZeroHealth -= OnPlayerDied;
        data.OnLevelUp -= SetLevelText;
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        _CurrentHealthText.text = currentHealth.ToString();
        _MaxHealthText.text = maxHealth.ToString();

        _HealthBar.fillAmount = (float)currentHealth / maxHealth;
    }

    private void UpdateExpBar(int currentExp, int MaxExp)
    {
        _ExpBar.fillAmount = (float)currentExp / MaxExp;
    }

    private void OnPlayerDied()
    {
        var data = ServiceLocator.Get<LevelManager>().PlayerStatusData;
        if (data != null)
        {
            UnsubscribeToPlayer(data);
        }
    }

    private void SetCurrentHealthText(int currentHealth)
    {
        _CurrentHealthText.text = currentHealth.ToString();
    }

    private void SetMaxHealthText(int maxHealth)
    {
        _MaxHealthText.text = maxHealth.ToString();
    }

    private void SetLevelText(int level)
    {
        _LevelText.text = level.ToString();
    }
}
