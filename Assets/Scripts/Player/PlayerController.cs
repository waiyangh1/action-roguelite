using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
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
    public int MouseDirXHash { get; private set; }
    public int MouseDirYHash { get; private set; }

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

    PlayerStateMachine stateMachine;
    Controls inputActions;
    float dashCooldownTimer;

    void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        GhostTrail = GetComponent<GhostTrail>();
        AttackHitbox = GetComponentInChildren<PlayerAttackHitbox>();

        MoveXHash = Animator.StringToHash("MoveX");
        MoveYHash = Animator.StringToHash("MoveY");
        LastMoveXHash = Animator.StringToHash("LastMoveX");
        LastMoveYHash = Animator.StringToHash("LastMoveY");
        MoveMagnitudeHash = Animator.StringToHash("MoveMagnitude");
        IsDashingHash = Animator.StringToHash("IsDashing");
        IsAttackingHash = Animator.StringToHash("IsAttacking");
        AttackIndexHash = Animator.StringToHash("AttackIndex");
        MouseDirXHash = Animator.StringToHash("MouseDirX");
        MouseDirYHash = Animator.StringToHash("MouseDirY");

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
        AttackPressed = inputActions.Player.Attack.triggered;

        UpdateAttackDir();

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

    void UpdateAttackDir()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
            new Vector3(mouseScreen.x, mouseScreen.y, -Camera.main.transform.position.z));

        Vector2 toMouse = (Vector2)mouseWorld - (Vector2)transform.position;
        AttackDir = toMouse.sqrMagnitude > 0.001f ? toMouse.normalized : LastMoveDir;

        Vector2 snapped = SnapTo8Dir(AttackDir);
        Animator.SetFloat(MouseDirXHash, snapped.x);
        Animator.SetFloat(MouseDirYHash, snapped.y);
    }

    static Vector2 SnapTo8Dir(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float snapped = Mathf.Round(angle / 45f) * 45f;
        float rad = snapped * Mathf.Deg2Rad;
        return new Vector2(Mathf.RoundToInt(Mathf.Cos(rad)), Mathf.RoundToInt(Mathf.Sin(rad)));
    }

    public void SetVelocity(Vector2 velocity)
    {
        Rb.linearVelocity = velocity;
    }

    public void StartDashCooldown()
    {
        dashCooldownTimer = data.dashCooldown;
    }
}
