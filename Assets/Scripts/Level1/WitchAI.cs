using UnityEngine;
using System.Collections;

public class WitchAI : EnemyAI
{
    [SerializeField] private Transform firePoint;

    protected override IEnumerator AttackSequence()
    {
        isBusy = true;
        rb.linearVelocity = Vector2.zero;
        spriteRenderer.flipX = player.position.x < transform.position.x;

        // Determine the origin point (Fire Point)
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        Vector2 dirToPlayer = (player.position - spawnPos).normalized;

        // 1. Telegraph Phase
        ChangeAnimationState("Telegraph");

        // Spawn LINE telegraph at the firePoint
        GameObject indicator = ObjectPooler.Instance.SpawnFromPool("LineTelegraph", spawnPos, Quaternion.identity);
        if (indicator != null)
        {
            // Setup for a straight line
            indicator.GetComponent<TelegraphIndicator>().SetupLine(stats.telegraphDuration, dirToPlayer);
        }

        yield return new WaitForSeconds(stats.telegraphDuration);

        // 2. Attack Phase
        ChangeAnimationState("Attack");
        GameObject orb = ObjectPooler.Instance.SpawnFromPool("WitchOrb", spawnPos, Quaternion.identity);

        if (orb != null) orb.GetComponent<EnemyProjectile>().Launch(dirToPlayer);

        EventBus.Publish(new PlaySoundEvent(stats.attackSound));
        yield return new WaitForSeconds(stats.attackCooldown);
        isBusy = false;
    }
}