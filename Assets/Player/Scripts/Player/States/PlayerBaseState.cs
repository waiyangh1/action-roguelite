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

    protected void TryAttack()
    {
        if (!Player.IsAttackBuffered) return;
        Player.ConsumeAttackBuffer();

        if (StateMachine.LingerTimer > 0f && StateMachine.ComboIndex < 2)
            StateMachine.ComboIndex++;
        else
            StateMachine.ComboIndex = 0;

        StateMachine.SwitchState(StateMachine.AttackState);
    }

    protected void TickLinger()
    {
        if (StateMachine.LingerTimer > 0f)
        {
            StateMachine.LingerTimer -= UnityEngine.Time.deltaTime;
            if (StateMachine.LingerTimer <= 0f)
                StateMachine.ComboIndex = 0;
        }
    }

    protected void TryDash()
    {
        if (Player.DashPressed && Player.CanDash)
            StateMachine.SwitchState(StateMachine.DashState);
    }
}
