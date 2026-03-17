using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Attacks/Melee")]
public class MeleeAttackSO : EnemyAttackSO
{
    public override void PerformAttack(EnemyShell shell, Transform target)
    {
        // Deal Damage
        if (target.TryGetComponent(out IDamageable player))
        {
            player.TakeDamage(shell.currentData.attackDamage, shell.gameObject);
        }
    }
}