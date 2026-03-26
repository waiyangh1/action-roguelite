using UnityEngine;
using UnityEngine.Pool;

public class EnemyShell : MonoBehaviour, IDamageable
{
    public EnemyConfigSO currentData;
    [HideInInspector] public int enemyListIndex = -1;

    public Rigidbody2D rb { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public Animator animator { get; private set; }
    public EnemyAttackHitbox AttackHitbox { get; private set; }

    public Vector2 spawnPosition { get; private set; }

    // --- Direction tracking (like PlayerController) ---
    public Vector2 MoveDir { get; set; }          // Current movement direction (set by states)
    public Vector2 LastMoveDir { get; set; }      // Last non‑zero direction (for idle facing)

    private IObjectPool<EnemyShell> pool;
    private float currentHealth;
    private float flashTimer;

    private static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");
    private MaterialPropertyBlock mpb;

    public EnemyStateMachine StateMachine { get; private set; }

    // Animator hashes
    public static readonly int MoveXHash = Animator.StringToHash("MoveX");
    public static readonly int MoveYHash = Animator.StringToHash("MoveY");
    public static readonly int MoveMagnitudeHash = Animator.StringToHash("MoveMagnitude");
    public static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    public static readonly int IsTelegraphingHash = Animator.StringToHash("IsTelegraphing");
    public static readonly int IsStaggeredHash = Animator.StringToHash("IsStaggered");
    public static readonly int LastMoveXHash = Animator.StringToHash("LastMoveX");
    public static readonly int LastMoveYHash = Animator.StringToHash("LastMoveY");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        AttackHitbox = GetComponentInChildren<EnemyAttackHitbox>();
        mpb = new MaterialPropertyBlock();
    }

    public void Initialize(EnemyConfigSO config, IObjectPool<EnemyShell> pool)
    {
        currentData = config;
        this.pool = pool;
        currentHealth = config.maxHealth;
        spriteRenderer.sprite = config.enemySprite;
        spawnPosition = transform.position;
        ResetFlash();

        // Default facing down
        LastMoveDir = Vector2.down;

        if (config.animatorController != null)
            animator.runtimeAnimatorController = config.animatorController;

        StateMachine = new EnemyStateMachine(this);
        StateMachine.SwitchState(StateMachine.WanderState);

        EnemyManager.Instance.RegisterEnemy(this);
    }

    public void OnUpdate(Transform player, float deltaTime)
    {
        StateMachine.Update(player, deltaTime);

        // --- Update animator with movement direction ---
        float moveMagnitude = MoveDir.sqrMagnitude;
        animator.SetFloat(MoveMagnitudeHash, moveMagnitude);

        if (moveMagnitude > 0.01f)
        {
            animator.SetFloat(MoveXHash, MoveDir.x);
            animator.SetFloat(MoveYHash, MoveDir.y);
            LastMoveDir = MoveDir; // remember last direction
        }
        else
        {
            // When idle, use last direction so blend tree faces correctly
            animator.SetFloat(MoveXHash, LastMoveDir.x);
            animator.SetFloat(MoveYHash, LastMoveDir.y);
        }

        // Always keep "last move" parameters updated (used for idle)
        animator.SetFloat(LastMoveXHash, LastMoveDir.x);
        animator.SetFloat(LastMoveYHash, LastMoveDir.y);
        // ------------------------------------------------

        if (flashTimer > 0)
        {
            flashTimer -= deltaTime;
            if (flashTimer <= 0)
                ResetFlash();
        }
    }

    public void OnFixedUpdate(Transform player, float fixedDeltaTime)
    {
        StateMachine.FixedUpdate(player, fixedDeltaTime);
    }

    public void TakeDamage(int amount, GameObject source)
    {
        Debug.Log($"{currentData.enemyType} took {amount} damage from {source.name}");
        currentHealth -= amount;

        flashTimer = 0.1f;
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat(FlashAmountID, 1f);
        spriteRenderer.SetPropertyBlock(mpb);

        if (StateMachine.CurrentState == StateMachine.TelegraphState)
        {
            if (currentData.enemyType == EnemyType.Tank)
                return;
            StateMachine.SwitchState(StateMachine.StaggerState);
        }

        if (currentHealth <= 0) Die();
    }

    private void ResetFlash()
    {
        if (mpb == null) mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat(FlashAmountID, 0f);
        spriteRenderer.SetPropertyBlock(mpb);
    }

    public void Die()
    {
        EnemyManager.Instance.UnregisterEnemy(this);
        pool.Release(this);
    }

    // Animation event methods
    public void BeginSwing() => AttackHitbox?.BeginSwing();
    public void EnableHitbox() => AttackHitbox?.Enable();
    public void DisableHitbox() => AttackHitbox?.Disable();
}