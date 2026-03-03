public abstract class PlayerBaseState : IState
{
    protected PlayerStateMachine StateMachine;
    protected PlayerController Player => StateMachine.Owner;

    protected PlayerBaseState(PlayerStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void Exit();

    protected void TryDash()
    {
        if (Player.DashPressed && Player.CanDash)
            StateMachine.SwitchState(StateMachine.DashState);
    }
}
