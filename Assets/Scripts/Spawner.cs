using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private float _spawnIntervalSeconds = 2f;
    [SerializeField] private bool isSpawning = true;
    [SerializeField]  private SpawnPoint[] _spawnPoints;
    private ObjectPool<Enemy> _enemiesPool;

    private int _poolDefaultCapacity = 20;
    private int _poolMaxSize = 200;

    private float spawnPositionY;
    private float spawnPositionYOffset = 2f;

    private void Awake()
    {
        spawnPositionY = _enemyPrefab.gameObject.transform.localScale.y / spawnPositionYOffset;

        _enemiesPool = new ObjectPool<Enemy>(
            createFunc: () => OnCreateNewPoolableObject(),
            actionOnDestroy: (enemy) => OnDestroyPoolableObject(enemy),
            actionOnRelease: (enemy) => OnReleasePoolableObject(enemy),
            defaultCapacity: _poolDefaultCapacity,
            maxSize: _poolMaxSize
            );
    }

    private void Start()
    {
        StartSpawning();
    }

    private Enemy OnCreateNewPoolableObject()
    {
        Enemy newEnemy = Instantiate(_enemyPrefab);
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

    private Vector3 GenerateSpawnPosition()
    {
        Vector3 spawnPosition = _spawnPoints[Random.Range(0, _spawnPoints.Length)].transform.position;
        spawnPosition.y = spawnPositionY;
        return spawnPosition;
    }

    private void SpawnAtPosition(Enemy enemy, Vector3 spawnPosition)
    {
        Enemy newEnemy = _enemiesPool.Get();

        Vector3 moveDirectionOffset = Random.insideUnitSphere;

        Vector3 targetPosition = spawnPosition + moveDirectionOffset;
        targetPosition.y = spawnPosition.y;

        newEnemy.Initialize(spawnPosition, targetPosition);
        newEnemy.Activate();
    }

    private void OnCollisionOccured(Enemy enemy)
    {
        _enemiesPool.Release(enemy);
    }

    private void StartSpawning()
    {
        StartCoroutine(SpawnOverTime());
    }

    private IEnumerator SpawnOverTime()
    {
        WaitForSeconds countdownTimer = new WaitForSeconds(_spawnIntervalSeconds);

        while (isSpawning)
        {
            SpawnAtPosition(_enemyPrefab, GenerateSpawnPosition());
            yield return countdownTimer;
        }
    }
}
