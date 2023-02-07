using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenu : UIBase
{
    private void OnEnable()
    {
#if !DEBUG
        Close();
        return;
#endif

        GameInstance.PauseGame();
    }

    private void OnDisable()
    {
        GameInstance.ResumeGame();
    }

    public override void Show()
    {
        base.Show();
        GameInstance.PauseGame();
    }

    public void OnCloseButtonClicked()
    {
        Close();
    }

    public void DebugPlayerLevelUp()
    {
        var levelManager = GameInstance.GetLevelManager();
        levelManager.GivePlayerExp(levelManager.PlayerStatusData.NextLevelExp);
    }

    public void DebugTogglePlayerInvincible()
    {
        GameInstance.GetLevelManager().PlayerStatusData.IsInvincible = !GameInstance.GetLevelManager().PlayerStatusData.IsInvincible;
    }
}
