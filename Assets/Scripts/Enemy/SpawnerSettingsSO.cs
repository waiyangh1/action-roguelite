using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerSettings", menuName = "EnemySpawn/SpawnerSettings")]
public class SpawnerSettingsSO : ScriptableObject
{
    [Header("Enemy Configurations")]
    public EnemyConfigSO[] enemyConfigs;
}