using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    float timer;
    Vector2 dashDir;

    public PlayerDashState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        timer = Player.DashDuration;
        dashDir = Player.LastMoveDir;

        Player.StartDashCooldown();
        Player.GhostTrail.Play();

        Player.Animator.SetFloat(Player.LastMoveXHash, dashDir.x);
        Player.Animator.SetFloat(Player.LastMoveYHash, dashDir.y);
        Player.Animator.SetBool(Player.IsDashingHash, true);
    }

    public override void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if (Player.MoveInput.sqrMagnitude > 0.01f)
                StateMachine.SwitchState(StateMachine.RunState);
            else
                StateMachine.SwitchState(StateMachine.IdleState);
        }
    }

    public override void FixedUpdate()
    {
        Player.SetVelocity(dashDir * Player.DashSpeed);
    }

    public override void Exit()
    {
        Player.Animator.SetBool(Player.IsDashingHash, false);
        Player.GhostTrail.Stop();
        Player.SetVelocity(UnityEngine.Vector2.zero);
    }
}
