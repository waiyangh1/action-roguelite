public class PlayerStateMachine : StateMachine<PlayerController>
{
    public PlayerIdleState IdleState { get; }
    public PlayerRunState RunState { get; }
    public PlayerDashState DashState { get; }

    public PlayerStateMachine(PlayerController player) : base(player)
    {
        IdleState = new PlayerIdleState(this);
        RunState = new PlayerRunState(this);
        DashState = new PlayerDashState(this);
    }
}
