public class PlayerAttackState : PlayerBaseState
{
    int comboIndex;

    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        comboIndex = StateMachine.ComboIndex;
        BeginAttack();
    }

    public override void Update()
    {
        float norm = Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        bool finished = !Player.Animator.IsInTransition(0) && norm >= 1f;
        if (!finished) return;
        ExitToMovement();
    }

    public override void FixedUpdate()
    {
        if (Player.MoveInput.sqrMagnitude > 0.01f)
            Player.SetVelocity(Player.AttackDir * Player.Data.attackMoveSpeed);
        else
            Player.SetVelocity(UnityEngine.Vector2.zero);
    }

    public override void Exit()
    {
        StateMachine.ComboIndex = comboIndex;
        Player.Animator.SetBool(Player.IsAttackingHash, false);
        Player.AttackHitbox.Disable();
    }

    void BeginAttack()
    {
        Player.AttackHitbox.BeginSwing();
        Player.Animator.SetInteger(Player.AttackIndexHash, comboIndex + 1);
        Player.Animator.SetBool(Player.IsAttackingHash, true);
    }

    void ExitToMovement()
    {
        if (comboIndex < 2)
            StateMachine.LingerTimer = Player.Data.lingerDuration;

        if (Player.MoveInput.sqrMagnitude > 0.01f)
            StateMachine.SwitchState(StateMachine.RunState);
        else
            StateMachine.SwitchState(StateMachine.IdleState);
    }
}
