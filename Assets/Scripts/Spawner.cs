using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [SerializeField] private float _spawnIntervalSeconds = 2f;
    [SerializeField] private bool _isSpawning = true;
    [SerializeField]  private SpawnPoint[] _spawnPoints;

    private Dictionary<Enemy, ObjectPool<Enemy>> _enemiesPools = new Dictionary<Enemy, ObjectPool<Enemy>>();

    private int _poolDefaultCapacity = 20;
    private int _poolMaxSize = 200;

    private float _spawnPositionYOffset = 2f;

    private void Awake()
    {
        foreach (SpawnPoint spawnPoint in _spawnPoints)
        {
            ObjectPool<Enemy> newEnemiesPool = new ObjectPool<Enemy>(
                createFunc: () => OnCreateNewPoolableObject(spawnPoint.EnemyPrefab),
                actionOnDestroy: (enemy) => OnDestroyPoolableObject(enemy),
                actionOnRelease: (enemy) => OnReleasePoolableObject(enemy),
                defaultCapacity: _poolDefaultCapacity,
                maxSize: _poolMaxSize
                );

            _enemiesPools.Add(spawnPoint.EnemyPrefab, newEnemiesPool);
        }
    }

    private void Start()
    {
        StartSpawning();
    }

    private Enemy OnCreateNewPoolableObject(Enemy enemyPrefab)
    {
        Enemy newEnemy = Instantiate(enemyPrefab);
        newEnemy.CollisionOccured += OnCollisionOccured;
        return newEnemy;
    }

    private void OnReleasePoolableObject(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    private void OnDestroyPoolableObject(Enemy enemy)
    {
        enemy.CollisionOccured -= OnCollisionOccured;
        Destroy(enemy);
    }

    private SpawnPoint GetRandomSpawnPoint()
    {
        return _spawnPoints[Random.Range(0, _spawnPoints.Length)];
    }

    private Vector3 GetSpawnPosition(SpawnPoint spawnPoint)
    {
        float spawnPositionY = spawnPoint.EnemyPrefab.gameObject.transform.localScale.y / _spawnPositionYOffset;

        Vector3 spawnPosition = spawnPoint.transform.position;
        spawnPosition.y = spawnPositionY;
        return spawnPosition;
    }

    private void SpawnAtPosition(SpawnPoint spawnPoint)
    {
        Enemy newEnemy = _enemiesPools[spawnPoint.EnemyPrefab].Get();

        Vector3 moveDirectionOffset = Random.insideUnitSphere;

        Vector3 spawnPosition = GetSpawnPosition(spawnPoint);

        newEnemy.Initialize(spawnPosition, spawnPoint.Target);
        newEnemy.Activate();
    }

    private void OnCollisionOccured(Enemy enemy)
    {
        Debug.Log(enemy + " collidied!");
    }

    private void StartSpawning()
    {
        StartCoroutine(SpawnOverTime());
    }

    private IEnumerator SpawnOverTime()
    {
        WaitForSeconds countdownTimer = new WaitForSeconds(_spawnIntervalSeconds);

        while (_isSpawning)
        {
            SpawnAtPosition(GetRandomSpawnPoint());
            yield return countdownTimer;
        }
    }
}
