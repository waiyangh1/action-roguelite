using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;

    public Rigidbody2D Rb { get; private set; }
    public Animator Animator { get; private set; }

    // Animator parameter hashes
    public int MoveXHash { get; private set; }
    public int MoveYHash { get; private set; }
    public int LastMoveXHash { get; private set; }
    public int LastMoveYHash { get; private set; }
    public int MoveMagnitudeHash { get; private set; }

    // Runtime state (read by states)
    public Vector2 MoveInput { get; private set; }
    public Vector2 LastMoveDir { get; set; } = Vector2.down;
    public float MoveSpeed => moveSpeed;

    PlayerStateMachine stateMachine;
    Controls inputActions;

    void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();

        MoveXHash = Animator.StringToHash("MoveX");
        MoveYHash = Animator.StringToHash("MoveY");
        LastMoveXHash = Animator.StringToHash("LastMoveX");
        LastMoveYHash = Animator.StringToHash("LastMoveY");
        MoveMagnitudeHash = Animator.StringToHash("MoveMagnitude");

        inputActions = new Controls();
        stateMachine = new PlayerStateMachine(this);
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void Start()
    {
        // Seed idle facing direction into animator
        Animator.SetFloat(LastMoveXHash, LastMoveDir.x);
        Animator.SetFloat(LastMoveYHash, LastMoveDir.y);

        stateMachine.SwitchState(stateMachine.IdleState);
    }

    void Update()
    {
        MoveInput = inputActions.Player.Move.ReadValue<Vector2>();
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
}
