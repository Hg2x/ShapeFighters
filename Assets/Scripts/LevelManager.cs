using ICKT.ServiceLocator;
using UnityEngine;

[AutoRegisteredService]
public class LevelManager : MonoBehaviour, IRegisterable
{
    public PlayerStatusData PlayerStatusData;
    public Transform PlayerTransform => _PlayerUnitReference.transform;
    private PlayerUnit _PlayerUnitReference;
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
            _PlayerUnitReference = Instantiate(data.PlayerUnitReference).GetComponent<PlayerUnit>();
        }
        else
        {
            Debug.LogError($"Place player and enemy reference in LevelManager");
        }

        PlayerStatusData = data.PlayerStatusData;
        if (PlayerStatusData != null)
        {
            PlayerStatusData.OnZeroHealth += OnPlayerDied;
            PlayerStatusData.OnLevelUp += OnPlayerLevelUp;
        }

        if (_EnemySpawner == null)
        {
            GameObject go = new("EnemySpawner");
            _EnemySpawner = go.AddComponent<EnemySpawner>();
        }
        _EnemySpawner.Init(data);

        _PlayerUnitReference.gameObject.SetActive(true);
        ServiceLocator.Get<CameraManager>().SetCameraTarget(_PlayerUnitReference.gameObject);
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
        if (_PlayerUnitReference != null)
        {
            Destroy(_PlayerUnitReference.gameObject);
            _PlayerUnitReference = null;
        }
        GameInstance.GoToInitialScene();
    }

    public void GivePlayerExp(int expAmount)
    {
        if (PlayerStatusData != null)
        {
            PlayerStatusData.GainExp(expAmount);
        }
    }

    public bool GivePlayerSkill(int skillID, WeaponSlot slot = WeaponSlot.None)
    {
        // if weapon
        return ServiceLocator.Get<WeaponManager>().GivePlayerWeapon(skillID, slot);
    }

    public void PlayerUseActiveSkill()
    {
        if (_PlayerUnitReference != null)
        {
            if (ServiceLocator.Get<WeaponManager>().UseActiveSkill())
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

    private void OnPlayerDied()
    {
        UIManager.Show<LevelFailUI>();
        StopEnemySpawn();
    }
}
