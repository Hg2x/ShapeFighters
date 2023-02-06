using System.Collections;
using System.Collections.Generic;
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

    public void Init(UnitStatusData playerStats)
    {
        if (playerStats == null)
        {
            Debug.Log("PlayerStats is null");
            return;
        }

        if (GameInstance.GetLevelManager().PlayerUnitReference.TryGetComponent<PlayerUnit>(out var playerUnitRef))
        {
            playerUnitRef.OnHealthChanged += UpdateHealthBar;
            playerUnitRef.OnExpChanged += UpdateExpBar;
            playerUnitRef.OnUnitDied += OnPlayerDied;
            playerUnitRef.OnLevelUp += SetLevelText;

            RefreshDisplay(playerStats);
        }
    }

    private void OnDisable()
    {
        // TODO: need unsubsricbe?
    }

    public void RefreshDisplay(UnitStatusData playerStats)
    {
        SetCurrentHealthText(playerStats.Health);
        SetMaxHealthText(playerStats.MaxHealth);
        UpdateExpBar(playerStats.CurrentExp, playerStats.NextLevelExp);
        SetLevelText(playerStats.Level);
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

    private void OnPlayerDied(UnitBase player)
    {
        if (player != null)
        {
            player.OnHealthChanged -= UpdateHealthBar;
            player.OnUnitDied -= OnPlayerDied;
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
