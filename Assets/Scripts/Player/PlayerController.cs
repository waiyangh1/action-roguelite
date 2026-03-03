using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float dashSpeed = 18f;
    [SerializeField] float dashDuration = 0.18f;
    [SerializeField] float dashCooldown = 1f;

    public Rigidbody2D Rb { get; private set; }
    public Animator Animator { get; private set; }
    public GhostTrail GhostTrail { get; private set; }

    // Animator parameter hashes
    public int MoveXHash { get; private set; }
    public int MoveYHash { get; private set; }
    public int LastMoveXHash { get; private set; }
    public int LastMoveYHash { get; private set; }
    public int MoveMagnitudeHash { get; private set; }
    public int IsDashingHash { get; private set; }

    // Runtime state (read by states)
    public Vector2 MoveInput { get; private set; }
    public Vector2 LastMoveDir { get; set; } = Vector2.down;
    public bool DashPressed { get; private set; }

    public float MoveSpeed => moveSpeed;
    public float DashSpeed => dashSpeed;
    public float DashDuration => dashDuration;
    public bool CanDash => dashCooldownTimer <= 0f;

    PlayerStateMachine stateMachine;
    Controls inputActions;
    float dashCooldownTimer;

    void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        GhostTrail = GetComponent<GhostTrail>();

        MoveXHash = Animator.StringToHash("MoveX");
        MoveYHash = Animator.StringToHash("MoveY");
        LastMoveXHash = Animator.StringToHash("LastMoveX");
        LastMoveYHash = Animator.StringToHash("LastMoveY");
        MoveMagnitudeHash = Animator.StringToHash("MoveMagnitude");
        IsDashingHash = Animator.StringToHash("IsDashing");

        inputActions = new Controls();
        stateMachine = new PlayerStateMachine(this);
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void Start()
    {
        Animator.SetFloat(LastMoveXHash, LastMoveDir.x);
        Animator.SetFloat(LastMoveYHash, LastMoveDir.y);

        stateMachine.SwitchState(stateMachine.IdleState);
    }

    void Update()
    {
        MoveInput = inputActions.Player.Move.ReadValue<Vector2>();
        DashPressed = inputActions.Player.Dash.triggered;

        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

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

    public void SetVelocity(Vector2 velocity)
    {
        Rb.linearVelocity = velocity;
    }

    public void StartDashCooldown()
    {
        dashCooldownTimer = dashCooldown;
    }
}
