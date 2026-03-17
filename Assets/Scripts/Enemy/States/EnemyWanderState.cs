using UnityEngine;

public class EnemyWanderState : EnemyBaseState
{
    private enum WanderPhase { Moving, Idle }
    private WanderPhase phase;

    private float phaseTimer;          // counts down for current phase
    private Vector2 wanderDirection;    // direction while moving

    // Timing constants
    private const float MIN_MOVE_TIME = 1f;
    private const float MAX_MOVE_TIME = 3f;
    private const float IDLE_TIME = 5f; // fixed 5 seconds idle

    public EnemyWanderState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        // Cancel any attack/stagger animations
        Enemy.animator.SetBool(EnemyShell.IsAttackingHash, false);
        Enemy.animator.SetBool(EnemyShell.IsTelegraphingHash, false);
        Enemy.animator.SetBool(EnemyShell.IsStaggeredHash, false);

        // Start in moving phase
        phase = WanderPhase.Moving;
        PickNewWanderDirection();
    }

    public override void Update(Transform player, float deltaTime)
    {
        // Always check for higher priority transitions first
        float distToPlayer = Vector2.Distance(Enemy.transform.position, player.position);
        float distFromSpawn = Vector2.Distance(Enemy.transform.position, Enemy.spawnPosition);

        if (distFromSpawn > Enemy.currentData.tetherRange)
        {
            StateMachine.SwitchState(StateMachine.RecoveryState);
            return;
        }

        if (distToPlayer <= Enemy.currentData.aggroRange)
        {
            StateMachine.SwitchState(StateMachine.ChaseState);
            return;
        }

        // Phase logic
        phaseTimer -= deltaTime;
        if (phaseTimer <= 0f)
        {
            switch (phase)
            {
                case WanderPhase.Moving:
                    // Finished moving → start idling
                    phase = WanderPhase.Idle;
                    phaseTimer = IDLE_TIME;
                    Enemy.MoveDir = Vector2.zero;
                    break;

                case WanderPhase.Idle:
                    // Finished idling → start moving again
                    phase = WanderPhase.Moving;
                    PickNewWanderDirection();
                    break;
            }
        }

        // Update MoveDir based on current phase (used by animator in EnemyShell)
        if (phase == WanderPhase.Moving)
        {
            Enemy.MoveDir = wanderDirection;
        }
        else // Idle
        {
            Enemy.MoveDir = Vector2.zero;
        }
    }

    public override void FixedUpdate(Transform player, float fixedDeltaTime)
    {
        if (phase == WanderPhase.Moving)
        {
            Enemy.rb.linearVelocity = wanderDirection * Enemy.currentData.moveSpeed;
        }
        else
        {
            Enemy.rb.linearVelocity = Vector2.zero;
        }
    }

    public override void Exit() { }

    private void PickNewWanderDirection()
    {
        // Choose a random direction (full 360°)
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        wanderDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
        phaseTimer = Random.Range(MIN_MOVE_TIME, MAX_MOVE_TIME);
    }
}