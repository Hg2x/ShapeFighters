using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private EnemyUnit _TestEnemyRef;
    private Vector3 _SpawnArea;

    private const float _WaitTime = 3f;
    private float _CachedTime = 0f;
    private float _NextSpawnTime;
    private bool _ShouldSpawn = false;

    private readonly Queue<EnemyUnit> _EnemyPool = new();
    private readonly List<EnemyUnit> _EnemyAlive = new();

    private int LatestSpawnIndex = 0;

    public void Init(StageData data)
    {
        _TestEnemyRef = data._EnemyRef;
        _SpawnArea = data._SpawnArea;
    }

    private void Update()
    {
        if (_ShouldSpawn)
        {
            if (Time.time >= _NextSpawnTime)
            {
                var finalWaitTime = _WaitTime;
                _NextSpawnTime += finalWaitTime;
                LinearSpawnPattern();
            }
            _CachedTime += Time.deltaTime;
        }
    }

    public void StartSpawner()
    {
        // TODO: add if init
        _NextSpawnTime = Time.time + _WaitTime;
        _ShouldSpawn = true;
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
        LatestSpawnIndex = 0;
    }

    public void ClearEnemyPool()
    {
        foreach (var enemy in _EnemyPool)
        {
            Destroy(enemy.gameObject);
        }
        _EnemyPool.Clear();

        foreach (var enemy in _EnemyAlive)
        {
            Destroy(enemy.gameObject);
        }
        _EnemyAlive.Clear();
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
        var indices = FunctionLibrary.GetRandomNumbers(amount, 0, _EnemyAlive.Count - 1);
        for (int i = 0; i < amount; i++)
        {
            var index = indices[i];
            positions[i] = _EnemyAlive[index].gameObject.transform.position;
        }

        return positions;
    }

    private void LinearSpawnPattern()
    {
        var amountToSpawn = (int)(_CachedTime / 5) + 1;
        if (amountToSpawn > 20)
        {
            amountToSpawn = 20;
        }

        for (int i = 0; i < amountToSpawn; i++)
        {
            Vector3 spawnPosition = transform.position + new Vector3(
                Random.Range(-_SpawnArea.x / 2, _SpawnArea.x / 2),
                _SpawnArea.y,
                Random.Range(-_SpawnArea.z / 2, _SpawnArea.z / 2));

            var enemy = GetEnemyFromPool();
            enemy.transform.position = spawnPosition;
            enemy.SetTargetPlayer(GameInstance.GetLevelManager().PlayerUnitReference.transform); // TODO: change this 
            enemy.gameObject.SetActive(true);
        }
    }

    private EnemyUnit GetEnemyFromPool()
    {
        EnemyUnit enemy;
        if (_EnemyPool.Count == 0)
        {
            enemy = Instantiate(_TestEnemyRef);
            enemy.SetSpawnIndex(LatestSpawnIndex);
            LatestSpawnIndex++;
            enemy.OnUnitDied += ReturnToPool;
            enemy.gameObject.SetActive(false);
        }
        else
        {
            enemy = _EnemyPool.Dequeue();
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
            foreach (var toRemove in _EnemyAlive)
            {
                if (toRemove.SpawnIndex == spawnIndex)
                {
                    _EnemyAlive.RemoveAt(spawnIndex);
                    break;
                }
            }
            _EnemyPool.Enqueue(enemyUnit);
        }
        else
        {
            Destroy(enemy);
        }
    }
}