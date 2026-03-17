using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private SpawnerSettingsSO settings;
    [SerializeField] private EnemyManager enemyManager;

    [Header("Spawning Zones")]
    [SerializeField] private float circleSpawnRadius = 5f;
    [SerializeField] private Vector2 mapSize = new Vector2(20f, 15f);
    [SerializeField] private float edgeOffset = 2f;

    private Transform player;

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        if (enemyManager == null)
            enemyManager = FindObjectOfType<EnemyManager>();

        // Spawn initial wave of 5 enemies
        SpawnWave(5);
    }

    private void Update()
    {
        // When all enemies are dead, spawn another wave of 5
        if (enemyManager != null && enemyManager.ActiveEnemiesCount == 0)
        {
            SpawnWave(5);
        }
    }

    private void SpawnWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnOneEnemy();
        }
    }

    private void SpawnOneEnemy()
    {
        if (EnemyPoolManager.Instance == null) return;

        Vector3 spawnPos = (Random.value > 0.5f) ? GetCircleSpawnPos() : GetEdgeSpawnPos();
        EnemyConfigSO randomConfig = settings.enemyConfigs[Random.Range(0, settings.enemyConfigs.Length)];
        EnemyPoolManager.Instance.Spawn(spawnPos, randomConfig);
    }

    private Vector3 GetCircleSpawnPos()
    {
        Vector2 randomPoint = Random.insideUnitCircle * circleSpawnRadius;
        return transform.position + new Vector3(randomPoint.x, randomPoint.y, 0);
    }

    private Vector3 GetEdgeSpawnPos()
    {
        float x = 0, y = 0;
        int side = Random.Range(0, 4);
        switch (side)
        {
            case 0: // Top
                x = Random.Range(-mapSize.x / 2, mapSize.x / 2);
                y = (mapSize.y / 2) + edgeOffset;
                break;
            case 1: // Bottom
                x = Random.Range(-mapSize.x / 2, mapSize.x / 2);
                y = (-mapSize.y / 2) - edgeOffset;
                break;
            case 2: // Left
                x = (-mapSize.x / 2) - edgeOffset;
                y = Random.Range(-mapSize.y / 2, mapSize.y / 2);
                break;
            case 3: // Right
                x = (mapSize.x / 2) + edgeOffset;
                y = Random.Range(-mapSize.y / 2, mapSize.y / 2);
                break;
        }
        return new Vector3(x, y, 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, circleSpawnRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapSize.x + (edgeOffset * 2), mapSize.y + (edgeOffset * 2), 0));
    }
}