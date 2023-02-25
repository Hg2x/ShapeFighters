using ICKT.ServiceLocator;

public class PauseUI : UIBase
{
    private void OnEnable()
    {
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

    public void OnResumeButtonClicked()
    {
        Close();
    }

    public void OnRetryButtonCliked()
    {
        ServiceLocator.Get<LevelManager>().ResetLevel();
    }

    public void OnExitButtonClicked() 
    {
        ServiceLocator.Get<LevelManager>().ExitLevel();
    }
}
