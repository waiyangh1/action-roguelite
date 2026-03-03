public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Player.SetVelocity(UnityEngine.Vector2.zero);
        Player.Animator.SetFloat(Player.MoveXHash, 0f);
        Player.Animator.SetFloat(Player.MoveYHash, 0f);
        Player.Animator.SetFloat(Player.MoveMagnitudeHash, 0f);
    }

    public override void Update()
    {
        TryDash();

        if (Player.MoveInput.sqrMagnitude > 0.01f)
            StateMachine.SwitchState(StateMachine.RunState);
    }

    public override void FixedUpdate() { }

    public override void Exit() { }
}
