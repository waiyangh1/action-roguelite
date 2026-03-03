public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter() { }

    public override void Update()
    {
        TryDash();

        if (Player.MoveInput.sqrMagnitude < 0.01f)
        {
            StateMachine.SwitchState(StateMachine.IdleState);
            return;
        }

        UnityEngine.Vector2 normalized = Player.MoveInput.normalized;
        Player.LastMoveDir = normalized;

        Player.Animator.SetFloat(Player.MoveXHash, normalized.x);
        Player.Animator.SetFloat(Player.MoveYHash, normalized.y);
        Player.Animator.SetFloat(Player.MoveMagnitudeHash, 1f);
        Player.Animator.SetFloat(Player.LastMoveXHash, normalized.x);
        Player.Animator.SetFloat(Player.LastMoveYHash, normalized.y);
    }

    public override void FixedUpdate()
    {
        Player.SetVelocity(Player.MoveInput.normalized * Player.MoveSpeed);
    }

    public override void Exit() { }
}
