using ICKT.ServiceLocator;
using System.Collections.Generic;
using UnityEngine;

[AutoRegisteredService]
public class UIManager : MonoBehaviour, IRegisterable
{
    private static UIManager _Instance;

    private UIBase _StartingUI;
    private readonly Dictionary<System.Type, UIBase> _UICollection = new();
    private readonly List<UIBase> _CreatedUIList = new();
    private readonly Stack<UIBase> _OpenUIhistory = new();
    private UIBase _CurrentUI;

    public bool IsPersistent() => true;

    private void Awake()
    {
        var data = FunctionLibrary.TryGetAssetSync<UIManagerData>("UIManagerData.asset"); // TODO: remove this hardcode
        Init(data);
    }

    private void Init(UIManagerData data)
    {
        if (_Instance != null)
        {
            Debug.LogError("UI Manager should not Init more than once");
        }

        _Instance = this;
        _StartingUI = data.StartingUI;
        var uiCollectionArray = data.UICollection;
        foreach (var ui in uiCollectionArray)
        {
            _UICollection.Add(ui.GetType(), ui);
        }
        
        if (_StartingUI != null) // TODO: change implementation later
        {
            var createdUI = Instantiate(_StartingUI);
            createdUI.Init();
            _Instance._CurrentUI = createdUI;
            _Instance._CreatedUIList.Add(createdUI);
        }
        else
        {
            Debug.LogError("please place starting UI in UIManager");
        }
    }

    public static void Show<T>(bool hidePreviousUI = false) where T : UIBase
    {
        if (_Instance._CurrentUI is T)
        {
            return;
        }

        // Gets and show UI that was already created previously
        foreach (var uiToShow in _Instance._CreatedUIList)
        {
            if (uiToShow is T)
            {
                if (uiToShow != null)
                {
                    _Instance._OpenUIhistory.Push(_Instance._CurrentUI);
                    if (hidePreviousUI)
                    {
                        _Instance._CurrentUI.Hide();
                    }

                    uiToShow.Show();
                    _Instance._CurrentUI = uiToShow;
                    return;
                }
            }
        }

        // Creates new instance of UI and show
        var uiToCreate = GetUI<T>();
        if (uiToCreate != null)
        {
            if (_Instance._CurrentUI != null)
            {
                _Instance._OpenUIhistory.Push(_Instance._CurrentUI);
                if (hidePreviousUI)
                    _Instance._CurrentUI.Hide();
            }

            var createdUI = Instantiate(uiToCreate);
            createdUI.Init();
            createdUI.Show();
            _Instance._CurrentUI = createdUI;
            _Instance._CreatedUIList.Add(createdUI);
        }
    }

    public static void CloseCurrentUI()
    {
        if (_Instance._OpenUIhistory.Count > 0 && _Instance._CurrentUI != null)
        {
            _Instance._CurrentUI.Hide();
            var previousUI = _Instance._OpenUIhistory.Pop();
            previousUI.Show();
            _Instance._CurrentUI = previousUI;
        }
        else
        {
            Debug.LogError("Cannot CloseCurrentUI when current UI is the last UI");
        }
    }

    public static void ClearAllUI()
    {
        foreach (UIBase ui in _Instance._CreatedUIList)
        {
            if (ui != null)
            {
                Destroy(ui.gameObject);
            }
        }
        _Instance._CreatedUIList.Clear();
        _Instance._CurrentUI = null;
        _Instance._OpenUIhistory.Clear();
    }

    private static T GetUI<T>() where T : UIBase
    {
        if (_Instance._UICollection.TryGetValue(typeof(T), out var ui) && ui is T typeUI)
        {
            return typeUI;
        }

        return null;
    }
}
