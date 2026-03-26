using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] PlayerSO data;

    public PlayerSO Data => data;
    public Rigidbody2D Rb { get; private set; }
    public Animator Animator { get; private set; }
    public GhostTrail GhostTrail { get; private set; }
    public PlayerAttackHitbox AttackHitbox { get; private set; }

    // Animator parameter hashes
    public int MoveXHash { get; private set; }
    public int MoveYHash { get; private set; }
    public int LastMoveXHash { get; private set; }
    public int LastMoveYHash { get; private set; }
    public int MoveMagnitudeHash { get; private set; }
    public int IsDashingHash { get; private set; }
    public int IsAttackingHash { get; private set; }
    public int AttackIndexHash { get; private set; }

    // Runtime state (read by states)
    public Vector2 MoveInput { get; private set; }
    public Vector2 LastMoveDir { get; set; } = Vector2.down;
    public Vector2 AttackDir { get; private set; }
    public bool DashPressed { get; private set; }
    public bool AttackPressed { get; private set; }

    public float MoveSpeed => data.moveSpeed;
    public float DashSpeed => data.dashSpeed;
    public float DashDuration => data.dashDuration;
    public bool CanDash => dashCooldownTimer <= 0f;
    public bool IsAttackBuffered => attackBufferTimer > 0f;

    PlayerStateMachine stateMachine;
    Controls inputActions;
    float dashCooldownTimer;
    float attackBufferTimer;

    // Health & damage flash
    int currentHealth;
    SpriteRenderer spriteRenderer;
    MaterialPropertyBlock mpb;
    float flashTimer;
    static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");

    void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        GhostTrail = GetComponent<GhostTrail>();
        AttackHitbox = GetComponentInChildren<PlayerAttackHitbox>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();

        MoveXHash = Animator.StringToHash("MoveX");
        MoveYHash = Animator.StringToHash("MoveY");
        LastMoveXHash = Animator.StringToHash("LastMoveX");
        LastMoveYHash = Animator.StringToHash("LastMoveY");
        MoveMagnitudeHash = Animator.StringToHash("MoveMagnitude");
        IsDashingHash = Animator.StringToHash("IsDashing");
        IsAttackingHash = Animator.StringToHash("IsAttacking");
        AttackIndexHash = Animator.StringToHash("AttackIndex");

        inputActions = new Controls();
        stateMachine = new PlayerStateMachine(this);
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void Start()
    {
        currentHealth = data.maxHealth;

        Animator.SetFloat(LastMoveXHash, LastMoveDir.x);
        Animator.SetFloat(LastMoveYHash, LastMoveDir.y);

        stateMachine.SwitchState(stateMachine.IdleState);
    }

    void Update()
    {
        MoveInput = inputActions.Player.Move.ReadValue<Vector2>();
        DashPressed = inputActions.Player.Dash.triggered;
        AttackPressed = inputActions.Player.Attack.triggered;

        if (AttackPressed)
            attackBufferTimer = data.attackBufferDuration;
        else if (attackBufferTimer > 0f)
            attackBufferTimer -= Time.deltaTime;

        Vector2 mouseScreen = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        AttackDir = (mouseWorld - Rb.position).normalized;

        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0f)
                ResetFlash();
        }

        stateMachine.Update();
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    public void ConsumeAttackBuffer() => attackBufferTimer = 0f;

    // Animation events — called by attack animation clips on the player root.
    public void EnableHitbox()  => AttackHitbox.Enable();
    public void DisableHitbox() => AttackHitbox.Disable();

    public void SetVelocity(Vector2 velocity)
    {
        Rb.linearVelocity = velocity;
    }

    public void StartDashCooldown()
    {
        dashCooldownTimer = data.dashCooldown;
    }

    // --- IDamageable ---

    public void TakeDamage(int amount, GameObject source)
    {
        currentHealth -= amount;

        flashTimer = 0.1f;
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat(FlashAmountID, 1f);
        spriteRenderer.SetPropertyBlock(mpb);

        Debug.Log($"[Player] took {amount} damage, HP: {currentHealth}/{data.maxHealth}");

        if (currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        Debug.Log("[Player] Died!");
        gameObject.SetActive(false);
    }

    void ResetFlash()
    {
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat(FlashAmountID, 0f);
        spriteRenderer.SetPropertyBlock(mpb);
    }
}
