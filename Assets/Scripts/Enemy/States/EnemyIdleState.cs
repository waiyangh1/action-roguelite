using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Enemy.MoveDir = Vector2.zero;   // stop moving
        Enemy.animator.SetBool(EnemyShell.IsAttackingHash, false);
        Enemy.animator.SetBool(EnemyShell.IsTelegraphingHash, false);
    }

    public override void Update(Transform player, float deltaTime)
    {
        float distToPlayer = Vector2.Distance(Enemy.transform.position, player.position);
        if (distToPlayer <= Enemy.currentData.aggroRange)
            StateMachine.SwitchState(StateMachine.ChaseState);
    }

    public override void FixedUpdate(Transform player, float fixedDeltaTime)
    {
        Enemy.rb.linearVelocity = Vector2.zero;
    }

    public override void Exit() { }
}