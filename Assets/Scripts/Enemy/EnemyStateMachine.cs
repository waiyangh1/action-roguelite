using UnityEngine;
public class EnemyStateMachine
{
    public EnemyShell Owner { get; private set; }
    public Transform Player { get; set; }

    public EnemyBaseState CurrentState { get; private set; }

    public EnemyIdleState IdleState { get; }
    public EnemyChaseState ChaseState { get; }
    public EnemyTelegraphState TelegraphState { get; }
    public EnemyAttackState AttackState { get; }
    public EnemyRecoveryState RecoveryState { get; }
    public EnemyStaggerState StaggerState { get; } 

    public EnemyBaseState WanderState { get; private set; }

    public EnemyStateMachine(EnemyShell owner)
    {
        Owner = owner;
        IdleState = new EnemyIdleState(this);
        ChaseState = new EnemyChaseState(this);
        TelegraphState = new EnemyTelegraphState(this);
        AttackState = new EnemyAttackState(this);
        RecoveryState = new EnemyRecoveryState(this);
        StaggerState = new EnemyStaggerState(this);
        WanderState = new EnemyWanderState(this);
    }

    public void SwitchState(EnemyBaseState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update(Transform player, float deltaTime)
    {
        Player = player;
        CurrentState?.Update(player, deltaTime);
    }

    public void FixedUpdate(Transform player, float fixedDeltaTime)
    {
        Player = player;
        CurrentState?.FixedUpdate(player, fixedDeltaTime);
    }
}