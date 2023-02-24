using ICKT.ServiceLocator;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInstance : MonoBehaviour
{
    private static GameInstance _Instance;
    // ServiceLocator is initialized after GameInstance Awake()

    public static LevelManager GetLevelManager() { return ServiceLocator.Get<LevelManager>(); }
    public static WeaponManager GetWeaponManager() { return ServiceLocator.Get<WeaponManager>(); }

    [SerializeField] private StageData _SandboxStageData;

    private void Awake()
    {
        if (_Instance != null)
        {
            Debug.LogWarning($"Duplicate GameInstance error on {gameObject.name}");
            Destroy(gameObject);
            return;
        }
        else
        {
            _Instance = this;
            DontDestroyOnLoad(_Instance);
        }
    }

    public static void StartSandboxLevel()
    {
        static void Callback()
        {
            GetLevelManager().LoadLevel(_Instance._SandboxStageData);
            UIManager.ClearAllUI();
            UIManager.Show<BattleUI>();
        }
        _Instance.StartCoroutine(LoadSceneAsync("SandboxScene", Callback));
    }

    public static void GoToInitialScene()
    {
        static void Callback()
        {
            UIManager.ClearAllUI();
            UIManager.Show<MainMenuUI>();
        }
        _Instance.StartCoroutine(LoadSceneAsync("InitialScene", Callback));
    }

    public static void PauseGame()
    {
        Time.timeScale = 0;
    }

    public static void ResumeGame()
    {
        Time.timeScale = 1;
    }

    private static IEnumerator LoadSceneAsync(string sceneName, Action callback)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        callback();
    }
}
