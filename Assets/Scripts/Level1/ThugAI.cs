using UnityEngine;
using System.Collections;

public class ThugAI : EnemyAI
{
    [SerializeField] private Transform attackPoint; // Assign in Inspector

    protected override IEnumerator AttackSequence()
    {
        isBusy = true;
        rb.linearVelocity = Vector2.zero;

        Vector3 spawnPos = attackPoint != null ? attackPoint.position : transform.position;
        Vector2 dirToPlayer = (player.position - spawnPos).normalized;

        // 1. Telegraph Phase
        ChangeAnimationState("Telegraph");

        // Spawn CURVE telegraph at the attackPoint
        GameObject indicator = ObjectPooler.Instance.SpawnFromPool("CurveTelegraph", spawnPos, Quaternion.identity);
        if (indicator != null)
        {
            // Setup for a curved melee arc
            indicator.GetComponent<TelegraphIndicator>().SetupCurve(stats.telegraphDuration, dirToPlayer);
        }

        yield return new WaitForSeconds(stats.telegraphDuration);

        // 2. Attack Phase
        ChangeAnimationState("Attack");
        EventBus.Publish(new PlaySoundEvent(stats.attackSound));

        yield return new WaitForSeconds(0.5f);
        isBusy = false;
    }
}