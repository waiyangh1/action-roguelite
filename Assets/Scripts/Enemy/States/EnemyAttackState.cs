using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    private enum AttackPhase { Animating, Cooldown }
    private AttackPhase phase;
    private float cooldownTimer;

    public EnemyAttackState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Enemy.animator.SetBool(EnemyShell.IsAttackingHash, true);
        Enemy.rb.linearVelocity = Vector2.zero;
        Enemy.MoveDir = Vector2.zero;   // stop movement visually
        Enemy.BeginSwing();
        phase = AttackPhase.Animating;
    }

    public override void Update(Transform player, float deltaTime)
    {
        switch (phase)
        {
            case AttackPhase.Animating:
                AnimatorStateInfo stateInfo = Enemy.animator.GetCurrentAnimatorStateInfo(0);
                bool finished = !Enemy.animator.IsInTransition(0) && stateInfo.normalizedTime >= 1f;
                if (finished)
                {
                    phase = AttackPhase.Cooldown;
                    cooldownTimer = Enemy.currentData.attackCooldown;
                }
                break;

            case AttackPhase.Cooldown:
                cooldownTimer -= deltaTime;
                if (cooldownTimer <= 0f)
                {
                    float distFromSpawn = Vector2.Distance(Enemy.transform.position, Enemy.spawnPosition);
                    if (distFromSpawn > Enemy.currentData.tetherRange)
                    {
                        StateMachine.SwitchState(StateMachine.RecoveryState);
                        return;
                    }

                    float distToPlayer = Vector2.Distance(Enemy.transform.position, player.position);
                    if (distToPlayer <= Enemy.currentData.attackRange)
                        StateMachine.SwitchState(StateMachine.TelegraphState);
                    else if (distToPlayer <= Enemy.currentData.aggroRange)
                        StateMachine.SwitchState(StateMachine.ChaseState);
                    else
                        StateMachine.SwitchState(StateMachine.WanderState);
                }
                break;
        }
    }

    public override void FixedUpdate(Transform player, float fixedDeltaTime)
    {
        // Enemy stands still during the entire attack (both animating and cooldown)
        Enemy.rb.linearVelocity = Vector2.zero;
    }

    public override void Exit()
    {
        Enemy.animator.SetBool(EnemyShell.IsAttackingHash, false);
        Enemy.DisableHitbox();  // Safety: ensure hitbox is off
    }
}