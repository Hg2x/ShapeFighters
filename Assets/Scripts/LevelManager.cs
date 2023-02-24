using ICKT.ServiceLocator;
using UnityEngine;

[AutoRegisteredService]
public class LevelManager : MonoBehaviour, IRegisterable
{
    public PlayerUnit PlayerUnitReference { get; private set; } // TODO: check this later
    public UnitStatusData PlayerStatusData;
    private EnemySpawner _EnemySpawner;

    // TODO: seperate in-battle and out-of-battle logic

    public bool IsPersistent()
    {
        return true;
    }

    public void LoadLevel(StageData data)
    {
        if (data.PlayerUnitReference != null)
        {
            PlayerUnitReference = Instantiate(data.PlayerUnitReference).GetComponent<PlayerUnit>();
            PlayerUnitReference.OnUnitDied += OnPlayerDied; // TODO: unsubscribe
            PlayerUnitReference.OnLevelUp += OnPlayerLevelUp;
        }
        else
        {
            Debug.LogError($"Place player and enemy reference in LevelManager");
        }

        PlayerStatusData = data.PlayerStatusData;

        if (_EnemySpawner == null)
        {
            GameObject go = new("EnemySpawner");
            _EnemySpawner = go.AddComponent<EnemySpawner>();
        }
        _EnemySpawner.Init(data);

        PlayerUnitReference.gameObject.SetActive(true);
        ServiceLocator.Get<CameraManager>().SetCameraTarget(PlayerUnitReference.gameObject);
        StartEnemySpawn();
    }

    public void ResetLevel()
    {
        // if in battle
        GameInstance.StartSandboxLevel();
        _EnemySpawner.ResetSpawner();
    }

    public void ExitLevel()
    {
        // if in battle
        if (_EnemySpawner != null)
        {
            Destroy(_EnemySpawner.gameObject);
            _EnemySpawner = null;
        }
        // TODO: unsubscribe stuff and clear stuff
        GameInstance.GoToInitialScene();
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

    public bool GivePlayerSkill(int skillID, WeaponSlot slot = WeaponSlot.None)
    {
        // if weapon
        return GameInstance.GetWeaponManager().GivePlayerWeapon(skillID, slot);
    }

    public void PlayerUseActiveSkill()
    {
        if (PlayerUnitReference != null)
        {
            if (GameInstance.GetWeaponManager().UseActiveSkill())
            {
                ServiceLocator.Get<CameraManager>().ActiveSkillSequence(10f, 1f); // TODO: remove hardcoded value
            }
        }
    }

    public Vector3[] GetRandomDifferentEnemyPositions(int amount)
    {
        if (_EnemySpawner != null)
        {
            return _EnemySpawner.GetRandomDifferentEnemyPositions(amount);
        }

        return null;
    }

    public void StartEnemySpawn()
    {
        if (_EnemySpawner != null)
        {
            _EnemySpawner.StartSpawner();
        }
    }

    public void StopEnemySpawn()
    {
        if (_EnemySpawner != null)
        {
            _EnemySpawner.StopSpawner();
        }
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
        StopEnemySpawn();
    }
}
