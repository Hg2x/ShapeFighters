using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    public virtual void Init()
    {

    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    protected void Close()
    {
        UIManager.CloseCurrentUI();
    }
}
