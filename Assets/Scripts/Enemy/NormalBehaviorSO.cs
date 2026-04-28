using UnityEngine;

[CreateAssetMenu(fileName = "NormalBehavior", menuName = "Enemy Behaviors/Normal")]
public class NormalBehaviorSO : EnemyBehaviorSO
{
    public override void TryDash(
        EnemyMovement enemy,
        Vector2 playerPos,
        float deltaTime,
        ref bool startDash,
        ref Vector2 dashDirection)
    {
        // never dash
        startDash = false;
    }

    public override Vector2 UpdateDash(EnemyMovement enemy, Vector2 currentDashDirection, float deltaTime)
    {
        return Vector2.zero; // never called, but safe
    }
}