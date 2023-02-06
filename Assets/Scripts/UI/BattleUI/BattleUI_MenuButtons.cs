using UnityEngine;

public class BattleUI_MenuButtons : MonoBehaviour
{
    public void OnPauseButtonClicked()
    {
        UIManager.Show<PauseUI>();
    }

    public void OnToggleUIButtonCLicked()
    {
        // TODO: implement toggle UI
    }
}
