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

    public void OnResumeButtonClicked()
    {
        Close();
    }

    public void OnRetryButtonCliked()
    {
        GameInstance.StartSandboxLevel();
    }

    public void OnExitButtonClicked() 
    {
        GameInstance.GoToInitialScene();
    }
}
