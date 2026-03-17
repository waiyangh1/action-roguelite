using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    public EnemyChaseState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Enemy.animator.SetBool(EnemyShell.IsAttackingHash, false);
        Enemy.animator.SetBool(EnemyShell.IsTelegraphingHash, false);
        Enemy.animator.SetBool(EnemyShell.IsStaggeredHash, false);
    }

    public override void Update(Transform player, float deltaTime)
    {
        float distToPlayer = Vector2.Distance(Enemy.transform.position, player.position);
        float distFromSpawn = Vector2.Distance(Enemy.transform.position, Enemy.spawnPosition);

        if (distFromSpawn > Enemy.currentData.tetherRange)
        {
            StateMachine.SwitchState(StateMachine.RecoveryState);
            return;
        }

        if (distToPlayer <= Enemy.currentData.attackRange)
        {
            StateMachine.SwitchState(StateMachine.TelegraphState);
            return;
        }
        if (distToPlayer > Enemy.currentData.aggroRange)
        {
            StateMachine.SwitchState(StateMachine.WanderState);
            return;
        }

        // Set movement direction (shell will update animator)
        Vector2 dir = (player.position - Enemy.transform.position).normalized;
        Enemy.MoveDir = dir;
    }

    public override void FixedUpdate(Transform player, float fixedDeltaTime)
    {
        Vector2 dir = (player.position - Enemy.transform.position).normalized;
        Enemy.rb.linearVelocity = dir * Enemy.currentData.moveSpeed;
    }

    public override void Exit() { }
}