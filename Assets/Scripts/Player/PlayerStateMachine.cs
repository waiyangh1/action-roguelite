public class PlayerStateMachine : StateMachine<PlayerController>
{
    public PlayerIdleState IdleState { get; }
    public PlayerRunState RunState { get; }
    public PlayerDashState DashState { get; }
    public PlayerAttackState AttackState { get; }

    public int ComboIndex { get; set; }
    public float LingerTimer { get; set; }

    public PlayerStateMachine(PlayerController player) : base(player)
    {
        IdleState = new PlayerIdleState(this);
        RunState = new PlayerRunState(this);
        DashState = new PlayerDashState(this);
        AttackState = new PlayerAttackState(this);
    }
}
