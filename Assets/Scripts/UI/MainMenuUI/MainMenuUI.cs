using UnityEngine;

public class MainMenuUI : UIBase
{
    public void OnPlayButtonClicked()
    {
        GameInstance.StartSandboxLevel();
    }

    public void OnSettingsButtonClicked()
    {
        
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
