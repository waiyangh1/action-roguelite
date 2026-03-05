using UnityEngine;
using System.Collections;

public abstract class EnemyAI : MonoBehaviour
{
    public EnemyStatsSO stats;
    protected Rigidbody2D rb;
    protected Transform player;
    protected bool isBusy = false;
    protected bool isDead = false;

    [Header("Wander Settings")]
    [SerializeField] private float wanderRadius = 3f;
    private Vector2 startPosition;
    private Vector2 wanderDirection;
    private float wanderTimer;
    private bool isWandering;

    [Header("Visuals")]
    [SerializeField] protected Animator anim;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] private Color flashColor = Color.white;
    private string _currentAnimState;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position; // Lock their "home" spot

        if (anim == null) anim = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) Initialize(playerObj.transform);
    }

    public virtual void Initialize(Transform playerTarget)
    {
        player = playerTarget;
        isBusy = false;
        isDead = false;
        FindObjectOfType<EnemyManager>().RegisterEnemy(this);
    }

    public void Think()
    {
        if (isBusy || isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= stats.attackRange)
        {
            StartCoroutine(AttackSequence());
        }
        else if (distanceToPlayer <= stats.aggroRange)
        {
            StopWandering();
            MoveTowards(player.position); // Chase Player
        }
        else
        {
            HandleWander(); // Idle/Wander Logic
        }
    }

    // --- RANDOM WANDER LOGIC ---
    private void HandleWander()
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0)
        {
            // Reset Timer: Move for 0.5 to 1.5 seconds, or stay still
            wanderTimer = Random.Range(0.5f, 2.0f);
            isWandering = !isWandering; // Toggle between moving and resting

            if (isWandering)
            {
                // Pick a random direction
                wanderDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            }
        }

        if (isWandering)
        {
            // Stay within the "Leash" radius of start position
            float distFromHome = Vector2.Distance(transform.position, startPosition);
            if (distFromHome > wanderRadius)
            {
                wanderDirection = (startPosition - (Vector2)transform.position).normalized;
            }

            MoveTowards((Vector2)transform.position + wanderDirection);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            ChangeAnimationState("Idle");
        }
    }

    private void StopWandering()
    {
        isWandering = false;
        wanderTimer = 0;
    }

    private void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;

        // Move Kinematic Rigidbody
        Vector2 newPos = rb.position + direction * stats.moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);

        ChangeAnimationState("Walk");
        if (direction.x != 0) spriteRenderer.flipX = direction.x < 0;
    }

    // --- ANIMATION & DAMAGE ---
    protected void ChangeAnimationState(string newState)
    {
        if (_currentAnimState == newState) return;
        anim.Play(newState);
        _currentAnimState = newState;
    }

    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;
        StartCoroutine(HitFlashRoutine());
        if (!isBusy) ChangeAnimationState("Hurt");
    }

    private IEnumerator HitFlashRoutine()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    protected abstract IEnumerator AttackSequence();

    public virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        ChangeAnimationState("Die");
        FindObjectOfType<EnemyManager>().UnregisterEnemy(this);
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        Invoke("DisableObject", 2f);
    }

    private void DisableObject() => gameObject.SetActive(false);

    private void OnDrawGizmosSelected()
    {
        if (stats == null) return;
        // Wander Radius (Blue)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Application.isPlaying ? startPosition : transform.position, wanderRadius);
        // Aggro Range (Yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stats.aggroRange);
        // Attack Range (Red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.attackRange);
    }
}