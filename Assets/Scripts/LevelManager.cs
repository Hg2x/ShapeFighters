using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject _Player;
    [SerializeField] private EnemySpawner _EnemySpawnerRef;

    public PlayerUnit PlayerUnitReference { get; private set; } // TODO: check this later
    public UnitStatusData PlayerStatusData;
    private EnemySpawner _EnemySpawner;

    private void Awake()
    {
        if (_Player != null)
        {
            PlayerUnitReference = Instantiate(_Player).GetComponent<PlayerUnit>();
            PlayerUnitReference.OnUnitDied += OnPlayerDied;
            PlayerUnitReference.OnLevelUp += OnPlayerLevelUp;
        }
        else
        {
            Debug.LogError($"Place player and enemy reference in LevelManager");
        }

        if (_EnemySpawner == null)
        {
            _EnemySpawner = Instantiate(_EnemySpawnerRef);
        }
    }

    public void GivePlayerExp(int expAmount)
    {
        if (PlayerUnitReference != null)
        {
            if (PlayerUnitReference.TryGetComponent<PlayerUnit>(out var player))
            {
                player.GainExp(expAmount);
            }
        }
    }

    public void StartSandboxLevel()
    {
        // and some other checks
        if (PlayerUnitReference == null)
        {
            Debug.LogError("Failed StartLevel() check in LevelManager");
            return;
        }

        PlayerUnitReference.gameObject.SetActive(true);
        GameInstance.GetCameraManager().SetCameraTarget(PlayerUnitReference.gameObject);
        _EnemySpawner.StartSpawner();
    }

    public void ResetLevel()
    {
        // TODO: either make use this or delete if reset by reloading scene
        _EnemySpawner.ResetSpawner();
    }

    public bool GivePlayerSkill(int skillID, WeaponSlot slot = WeaponSlot.None)
    {
        // if weapon
        return GameInstance.GetWeaponManager().GivePlayerWeapon(skillID, slot);
    }

    public Vector3[] GetRandomDifferentEnemyPositions(int amount)
    {
        if (_EnemySpawner != null)
        {
            return _EnemySpawner.GetRandomDifferentEnemyPositions(amount);
        }

        return null;
    }

    private void OnPlayerLevelUp(int level)
    {
        GameInstance.PauseGame();
        UIManager.Show<LevelUpUI>();
    }

    private void OnPlayerDied(UnitBase player)
    {
        player.gameObject.SetActive(false);
        UIManager.Show<LevelFailUI>();
        _EnemySpawner.StopSpawner();
    }
}
