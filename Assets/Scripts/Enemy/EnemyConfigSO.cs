using UnityEngine;

public enum EnemyType { Normal, Tank }

[CreateAssetMenu(fileName = "NewEnemyConfig", menuName = "Enemy/EnemyConfig")]
public class EnemyConfigSO : ScriptableObject
{
    public EnemyType enemyType;
    public RuntimeAnimatorController animatorController;
    public Sprite enemySprite;
    public float maxHealth = 10f;
    public float moveSpeed = 4f;
    public float aggroRange = 3f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;

    [Header("Telegraph")]
    public float telegraphDuration = 0.5f;

    [Header("Attack Movement")]
    public float attackMoveSpeed = 1.5f;   // NEW

    [Header("Tether")]
    public float tetherRange = 10f;

    public EnemyAttackSO attackStrategy;    // (optional, you may keep or remove)
}