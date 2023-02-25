using ICKT.ServiceLocator;
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
        var levelManager = ServiceLocator.Get<LevelManager>();
        levelManager.GivePlayerExp(levelManager.PlayerStatusData.NextLevelExp);
    }

    public void DebugTogglePlayerInvincible()
    {
        var levelManager = ServiceLocator.Get<LevelManager>();
        levelManager.PlayerStatusData.IsInvincible = !levelManager.PlayerStatusData.IsInvincible;
    }

    public void DebugStartEnemySpawn()
    {
        ServiceLocator.Get<LevelManager>().StartEnemySpawn();
    }

    public void DebugStopEnemySpawn()
    {
        ServiceLocator.Get<LevelManager>().StopEnemySpawn();
    }

    public void Test()
    {
        
    }

    public void Test2()
    {
        
    }
}
