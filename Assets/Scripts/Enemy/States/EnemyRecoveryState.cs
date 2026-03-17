using UnityEngine;

public class EnemyRecoveryState : EnemyBaseState
{
    public EnemyRecoveryState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Enemy.animator.SetBool(EnemyShell.IsAttackingHash, false);
        Enemy.animator.SetBool(EnemyShell.IsTelegraphingHash, false);
        Enemy.animator.SetBool(EnemyShell.IsStaggeredHash, false);
    }

    public override void Update(Transform player, float deltaTime)
    {
        float distToSpawn = Vector2.Distance(Enemy.transform.position, Enemy.spawnPosition);
        if (distToSpawn < 0.1f)
        {
            StateMachine.SwitchState(StateMachine.WanderState);
            return;
        }

        Vector2 dir = (Enemy.spawnPosition - (Vector2)Enemy.transform.position).normalized;
        Enemy.MoveDir = dir;
    }

    public override void FixedUpdate(Transform player, float fixedDeltaTime)
    {
        Vector2 dir = (Enemy.spawnPosition - (Vector2)Enemy.transform.position).normalized;
        Enemy.rb.linearVelocity = dir * Enemy.currentData.moveSpeed;
    }

    public override void Exit() { }
}