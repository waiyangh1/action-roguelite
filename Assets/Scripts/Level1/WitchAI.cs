using UnityEngine;
using System.Collections;

public class WitchAI : EnemyAI
{
    [Header("Witch Specific")]
    [SerializeField] private Transform firePoint;

    protected override IEnumerator AttackSequence()
    {
        isBusy = true;
        rb.linearVelocity = Vector2.zero;

        ChangeAnimationState("Attack");

        yield return new WaitForSeconds(stats.telegraphDuration);

        // Spawn Projectile from Pool
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        GameObject orb = ObjectPooler.Instance.SpawnFromPool("WitchOrb", spawnPos, Quaternion.identity);

        if (orb != null)
        {
            Vector2 dir = (player.position - spawnPos).normalized;
            orb.GetComponent<EnemyProjectile>().Launch(dir);
        }

        EventBus.Publish(new PlaySoundEvent(stats.attackSound));

        yield return new WaitForSeconds(stats.attackCooldown);
        isBusy = false;
    }
}