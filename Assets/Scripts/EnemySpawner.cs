using ICKT.ServiceLocator;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private EnemyUnit _TestEnemyRef;
    private Vector3 _SpawnArea;

    private bool _IsInitialized = false;
    private const float _WaitTime = 3f;
    private float _CachedTime = 0f;
    private float _NextSpawnTime;
    private bool _ShouldSpawn = false;

    private readonly List<EnemyUnit> _EnemyPool = new();
    private readonly HashSet<EnemyUnit> _EnemyAlive = new();

    private int _LatestSpawnIndex = 0;
    private Transform _PlayerTransform;

    public void Init(StageData data)
    {
        _TestEnemyRef = data._EnemyRef;
        _SpawnArea = data._SpawnArea;
        _IsInitialized = true;
    }

    private void Update()
    {
        if (_ShouldSpawn && _IsInitialized)
        {
            if (Time.time >= _NextSpawnTime)
            {
                var finalWaitTime = _WaitTime;
                _NextSpawnTime = Time.time + finalWaitTime;
                LinearSpawnPattern();
            }
            _CachedTime += Time.deltaTime;
        }
    }

    public void StartSpawner()
    {
        if (_IsInitialized)
        {
            _NextSpawnTime = Time.time + _WaitTime;
            _ShouldSpawn = true;
        }
    }

    public void StopSpawner()
    {
        _ShouldSpawn = false;
    }

    public void ResetSpawner()
    {
        _CachedTime = 0f;
        ClearEnemyPool();
        StartSpawner();
        _LatestSpawnIndex = 0;
    }

    public Vector3[] GetRandomDifferentEnemyPositions(int amount)
    {
        if (_EnemyAlive.Count == 0)
        {
            return null;
        }

        if (amount > _EnemyAlive.Count)
        {
            amount = _EnemyAlive.Count;
        }

        Vector3[] positions = new Vector3[amount];
        var indices = FunctionLibrary.GetRandomNumbers(amount, 0, _EnemyAlive.Count - 1, false);
        for (int i = 0; i < amount; i++)
        {
            var index = indices[i];
            positions[i] = _EnemyAlive.ElementAt(index).gameObject.transform.position;
        }

        return positions;
    }

    private void ClearEnemyPool()
    {
        foreach (var enemy in _EnemyPool.Concat(_EnemyAlive))
        {
            Destroy(enemy.gameObject);
        }
        _EnemyPool.Clear();
        _EnemyAlive.Clear();
    }

    private void LinearSpawnPattern(int amountCapPerSpawn = 20)
    {
        var amountToSpawn = (int)(_CachedTime / 5) + 1;
        if (amountCapPerSpawn <= 0)
        {
            return;
        }
        amountToSpawn = Mathf.Min(amountToSpawn, amountCapPerSpawn);

        var halfSpawnAreaX = _SpawnArea.x / 2;
        var halfSpawnAreaZ = _SpawnArea.z / 2;
        if (_PlayerTransform == null)
        {
            _PlayerTransform = ServiceLocator.Get<LevelManager>().PlayerTransform;
        }
        
        for (int i = 0; i < amountToSpawn; i++)
        {
            Vector3 spawnPosition = transform.position + new Vector3(
                Random.Range(-halfSpawnAreaX, halfSpawnAreaX),
                _SpawnArea.y,
                Random.Range(-halfSpawnAreaZ, halfSpawnAreaZ));

            var enemy = GetEnemyFromPool();
            enemy.transform.position = spawnPosition;
            enemy.SetTargetPlayer(_PlayerTransform);
            enemy.gameObject.SetActive(true);
        }
    }

    private EnemyUnit GetEnemyFromPool()
    {
        EnemyUnit enemy;
        if (_EnemyPool.Count == 0)
        {
            enemy = Instantiate(_TestEnemyRef);
            enemy.SetSpawnIndex(_LatestSpawnIndex);
            _LatestSpawnIndex++;
            enemy.OnUnitDied += ReturnToPool;
            enemy.gameObject.SetActive(false);
        }
        else
        {
            enemy = _EnemyPool[0];
            _EnemyPool.RemoveAt(0);
        }

        enemy.ResetStats();
        _EnemyAlive.Add(enemy);
        return enemy;
    }

    private void ReturnToPool(UnitBase enemy)
    {
        enemy.gameObject.SetActive(false);
        if (enemy is EnemyUnit enemyUnit)
        {
            var spawnIndex = enemyUnit.SpawnIndex;
            foreach (var toRemove in _EnemyAlive.Where(toRemove => toRemove.SpawnIndex == spawnIndex).ToList())
            {
                _EnemyAlive.Remove(toRemove);
                break;
            }
            _EnemyPool.Insert(0, enemyUnit);
        }
        else
        {
            Destroy(enemy);
        }
    }
}