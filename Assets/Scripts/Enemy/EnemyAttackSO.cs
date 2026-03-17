using UnityEngine;

public abstract class EnemyAttackSO : ScriptableObject
{
    // The shell passes itself and its target to the strategy
    public abstract void PerformAttack(EnemyShell shell, Transform target);
}