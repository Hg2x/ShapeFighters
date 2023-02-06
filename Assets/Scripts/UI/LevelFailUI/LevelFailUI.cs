public class LevelFailUI : UIBase
{
    public void OnRetryButtonClicked()
    {
        GameInstance.StartSandboxLevel();
    }

    public void OnSettingsButtonClicked()
    {

    }

    public void OnExitButtonClicked()
    {
        GameInstance.GoToInitialScene();
    }
}
