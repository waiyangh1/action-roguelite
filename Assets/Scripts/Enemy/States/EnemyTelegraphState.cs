using UnityEngine;

public class EnemyTelegraphState : EnemyBaseState
{
    private float timer;

    public EnemyTelegraphState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Enemy.rb.linearVelocity = Vector2.zero;
        Enemy.MoveDir = Vector2.zero;   // ensure idle facing
        Enemy.animator.SetBool(EnemyShell.IsTelegraphingHash, true);
        timer = Enemy.currentData.telegraphDuration;
    }

    public override void Update(Transform player, float deltaTime)
    {
        timer -= deltaTime;
        if (timer <= 0f)
        {
            StateMachine.SwitchState(StateMachine.AttackState);
        }
    }

    public override void FixedUpdate(Transform player, float fixedDeltaTime)
    {
        Enemy.rb.linearVelocity = Vector2.zero;
    }

    public override void Exit()
    {
        Enemy.animator.SetBool(EnemyShell.IsTelegraphingHash, false);
    }
}