public abstract class StateMachine<TOwner>
{
    public TOwner Owner { get; }

    IState currentState;

    protected StateMachine(TOwner owner)
    {
        Owner = owner;
    }

    public void SwitchState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update() => currentState?.Update();

    public void FixedUpdate() => currentState?.FixedUpdate();
}
