using UnityEngine;

public abstract class EnemyBehaviorSO : ScriptableObject
{
    /// <summary>
    /// Called each FixedUpdate when NOT dashing and cooldown is off.
    /// Should set <paramref name="startDash"/> to true when dash should begin.
    /// </summary>
    public abstract void TryDash(
        EnemyMovement enemy,
        Vector2 playerPos,
        float deltaTime,
        ref bool startDash,
        ref Vector2 dashDirection);

    /// <summary>
    /// Called each FixedUpdate while dashing.
    /// Return the velocity the enemy should have this frame.
    /// </summary>
    public abstract Vector2 UpdateDash(EnemyMovement enemy, Vector2 currentDashDirection, float deltaTime);
}