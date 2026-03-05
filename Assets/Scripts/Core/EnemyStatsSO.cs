using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "AI/EnemyStats")]
public class EnemyStatsSO : ScriptableObject
{
    public float health = 50f;
    public float moveSpeed = 3.5f;
    public float aggroRange = 10f;
    public float attackRange = 2f;
    public float telegraphDuration = 0.6f;
    public float attackCooldown = 1.5f;

    [Header("Audio")]
    public SoundDataSO attackSound;
    public SoundDataSO deathSound;
}