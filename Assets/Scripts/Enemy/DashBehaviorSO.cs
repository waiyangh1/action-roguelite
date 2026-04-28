using UnityEngine;

[CreateAssetMenu(fileName = "DashBehavior", menuName = "Enemy Behaviors/Dash")]
public class DashBehaviorSO : EnemyBehaviorSO
{
    [Header("Dash Parameters")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float maxDashCheckDistance = 10f;
    [SerializeField] private float clearSightConfirmationDelay = 0.15f;
    [SerializeField] private LayerMask obstacleMask = -1;

    public float DashSpeed => dashSpeed;
    public float DashDuration => dashDuration;
    public float DashCooldown => dashCooldown;

    public override void TryDash(
        EnemyMovement enemy,
        Vector2 playerPos,
        float deltaTime,
        ref bool startDash,
        ref Vector2 dashDirection)
    {
        startDash = false;

        Vector2[] cornerOrigins = enemy.GetCornerOrigins();
        if (cornerOrigins == null) return;

        Vector2 enemyPos = enemy.transform.position;
        Vector2 direction = (playerPos - enemyPos).normalized;
        float distance = Vector2.Distance(enemyPos, playerPos);
        float rayLength = Mathf.Min(distance + 0.5f, maxDashCheckDistance);

        bool allClear = true;
        for (int i = 0; i < cornerOrigins.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(cornerOrigins[i], direction, rayLength, obstacleMask);
            if (hit.collider != null)
            {
                allClear = false;
                break;
            }
        }

        if (allClear)
        {
            enemy.DashClearTimer += deltaTime;
            if (enemy.DashClearTimer >= clearSightConfirmationDelay)
            {
                startDash = true;
                dashDirection = direction;
                enemy.DashClearTimer = 0f;
            }
        }
        else
        {
            enemy.DashClearTimer = 0f;
        }
    }

    public override Vector2 UpdateDash(EnemyMovement enemy, Vector2 currentDashDirection, float deltaTime)
    {
        return currentDashDirection * dashSpeed;
    }
}