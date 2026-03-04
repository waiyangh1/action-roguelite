using UnityEngine;
using System.Collections; // Fixes WaitForSeconds/IEnumerator errors

public class ThugAI : EnemyAI
{
    protected override IEnumerator AttackSequence()
    {
        isBusy = true;
        rb.linearVelocity = Vector2.zero;

        ChangeAnimationState("Attack");

        yield return new WaitForSeconds(stats.telegraphDuration);
        EventBus.Publish(new PlaySoundEvent(stats.attackSound));

        yield return new WaitForSeconds(0.5f); // Duration of the swing
        isBusy = false;
    }
}