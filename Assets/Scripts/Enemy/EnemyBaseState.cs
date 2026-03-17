using UnityEngine;

public abstract class EnemyBaseState
{
    protected EnemyStateMachine StateMachine;
    protected EnemyShell Enemy => StateMachine.Owner;

    protected EnemyBaseState(EnemyStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    public abstract void Enter();
    public abstract void Update(Transform player, float deltaTime);
    public abstract void FixedUpdate(Transform player, float fixedDeltaTime);
    public abstract void Exit();
}