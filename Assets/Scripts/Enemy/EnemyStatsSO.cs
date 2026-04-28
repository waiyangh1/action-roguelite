using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "ScriptableObjects/EnemyStats")]
public class EnemyStatsSO : ScriptableObject
{
    [Header("Movement")]
    public float MoveSpeed = 3f;
    public float TurnSpeed = 10f; // For smooth direction lerping

    [Header("Combat")]
    public float MaxHealth = 100f;
    public int AttackDamage = 15;
    public float AttackRange = 1.5f;
    public float AttackCooldown = 1f;

    [Header("Optimizations")]
    public float NudgeStrength = 0.5f; // Force to push away from other enemies
}