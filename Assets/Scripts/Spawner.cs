using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private float _spawnIntervalSeconds = 2f;

    private SpawnPoint[] _spawnPoints;
    private ObjectPool<Enemy> _enemiesPool;

    private int _poolDefaultCapacity = 20;
    private int _poolMaxSize = 200;

    private float spawnPositionY;

    private void Awake()
    {
        spawnPositionY = _enemyPrefab.gameObject.transform.localScale.y / 2f;

        _spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);

        _enemiesPool = new ObjectPool<Enemy>(
            createFunc: () => CreateNew(),
            actionOnDestroy: (enemy) => DestroyPooledObject(enemy),
            actionOnRelease: (enemy) => ReturnToPool(enemy),
            defaultCapacity: _poolDefaultCapacity,
            maxSize: _poolMaxSize
            );
    }

    private void Start()
    {
        StartSpawning();
    }

    private Enemy CreateNew()
    {
        Enemy newEnemy = Instantiate(_enemyPrefab);
        newEnemy.ReadyToBePooled += OnReadyToBePooled;
        return newEnemy;
    }

    private void ReturnToPool(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    private void DestroyPooledObject(Enemy enemy)
    {
        enemy.ReadyToBePooled -= OnReadyToBePooled;
        Destroy(enemy);
    }

    private Vector3 PickSpawnPoint()
    {
        Vector3 spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)].transform.position;
        spawnPoint.y = spawnPositionY;
        return spawnPoint;
    }

    private void SpawnAtPosition(Enemy enemy, Vector3 spawnPosition)
    {
        Enemy newEnemy = _enemiesPool.Get();

        Vector3 moveDirectionOffset = Random.insideUnitSphere;

        Vector3 targetPosition = spawnPosition + moveDirectionOffset;
        targetPosition.y = spawnPosition.y;

        newEnemy.Activate(spawnPosition, targetPosition);
    }

    private void OnReadyToBePooled(Enemy enemy)
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

        while (true)
        {
            SpawnAtPosition(_enemyPrefab, PickSpawnPoint());
            yield return countdownTimer;
        }
    }
}
