using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void StartSceneDelegate();

public class GameInstance : MonoBehaviour
{
    private static GameInstance _Instance;

    public static LevelManager GetLevelManager() { return _Instance._LevelManager; }
    [SerializeField] private GameObject _LevelManagerGO;
    private LevelManager _LevelManager;
    
    public static CameraManager GetCameraManager() { return _Instance._CameraManager; }
    [SerializeField] private GameObject _CameraManagerGO;
    private CameraManager _CameraManager;

    public static WeaponManager GetWeaponManager() { return _Instance._WeaponManager; }
    [SerializeField] private GameObject _WeaponManagerGO;
    private WeaponManager _WeaponManager;

    [SerializeField] private GameObject _UIManagerGO;
    private UIManager _UIManager;

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

        if (_UIManager == null)
        {
            _UIManager = Instantiate(_UIManagerGO).GetComponent<UIManager>();
            _UIManager.gameObject.transform.SetParent(gameObject.transform);
        }
    }

    public static void StartSandboxLevel()
    {
        StartSceneDelegate callback = LoadSandboxLevel;
        _Instance.StartCoroutine(LoadSceneAsync("SandboxScene", callback));
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

    private static IEnumerator LoadSceneAsync(string sceneName, StartSceneDelegate callback)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        callback();
    }

    private static void LoadSandboxLevel()
    {
        if (_Instance._WeaponManager == null)
        {
            _Instance._WeaponManager = Instantiate(_Instance._WeaponManagerGO).GetComponent<WeaponManager>();
        }

        if (_Instance._LevelManager == null)
        {
            _Instance._LevelManager = Instantiate(_Instance._LevelManagerGO).GetComponent<LevelManager>();
        }

        if (_Instance._CameraManager == null)
        {
            _Instance._CameraManager = Instantiate(_Instance._CameraManagerGO).GetComponent<CameraManager>();
        }

        _Instance._LevelManager.GetComponent<LevelManager>().StartSandboxLevel();
        UIManager.ClearAllUI();
        UIManager.Show<BattleUI>();
    }
}
