using UnityEngine;

public class EnemyStaggerState : EnemyBaseState
{
    private float timer;
    private const float STAGGER_DURATION = 0.3f; // Short flinch

    public EnemyStaggerState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Enemy.rb.linearVelocity = Vector2.zero;
        Enemy.MoveDir = Vector2.zero;
        Enemy.animator.SetBool(EnemyShell.IsStaggeredHash, true);
        timer = STAGGER_DURATION;
    }

    public override void Update(Transform player, float deltaTime)
    {
        timer -= deltaTime;
        if (timer <= 0f)
        {
            // After stagger, decide next state based on distance to player
            float distToPlayer = Vector2.Distance(Enemy.transform.position, player.position);
            float distFromSpawn = Vector2.Distance(Enemy.transform.position, Enemy.spawnPosition);

            if (distFromSpawn > Enemy.currentData.tetherRange)
                StateMachine.SwitchState(StateMachine.RecoveryState);
            else if (distToPlayer <= Enemy.currentData.aggroRange)
                StateMachine.SwitchState(StateMachine.ChaseState);
            else
                StateMachine.SwitchState(StateMachine.WanderState);
        }
    }

    public override void FixedUpdate(Transform player, float fixedDeltaTime)
    {
        Enemy.rb.linearVelocity = Vector2.zero;
    }

    public override void Exit()
    {
        Enemy.animator.SetBool(EnemyShell.IsStaggeredHash, false);
    }
}