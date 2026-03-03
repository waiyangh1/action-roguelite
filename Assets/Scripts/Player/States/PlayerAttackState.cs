using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    int comboIndex;
    bool nextQueued;
    bool hitboxOpen;

    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        comboIndex = StateMachine.ComboIndex;
        BeginAttack();
    }

    public override void Update()
    {
        float norm = Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        // Queue next attack after hitbox closes
        if (Player.AttackPressed && !nextQueued && norm >= Player.Data.hitCloseNorm[comboIndex])
            nextQueued = true;

        // Open / close hitbox
        bool inWindow = norm >= Player.Data.hitOpenNorm[comboIndex] && norm <= Player.Data.hitCloseNorm[comboIndex];
        if (inWindow && !hitboxOpen)      { Player.AttackHitbox.Enable();  hitboxOpen = true; }
        else if (!inWindow && hitboxOpen) { Player.AttackHitbox.Disable(); hitboxOpen = false; }

        // Animation finished when normalizedTime >= 1 and not mid-transition
        bool finished = !Player.Animator.IsInTransition(0) && norm >= 1f;
        if (!finished) return;

        Player.AttackHitbox.Disable();
        hitboxOpen = false;

        if (nextQueued && comboIndex < 2)
        {
            comboIndex++;
            BeginAttack();
        }
        else
        {
            ExitToMovement();
        }
    }

    public override void FixedUpdate()
    {
        Player.SetVelocity(Vector2.zero);
    }

    public override void Exit()
    {
        StateMachine.ComboIndex = comboIndex;
        Player.Animator.SetBool(Player.IsAttackingHash, false);
        Player.AttackHitbox.Disable();
    }

    void BeginAttack()
    {
        nextQueued = false;
        hitboxOpen = false;

        Player.AttackHitbox.SetDirection(Player.AttackDir);
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
