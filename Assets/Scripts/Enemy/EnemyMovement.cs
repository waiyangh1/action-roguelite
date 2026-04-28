using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class EnemyMovement : MonoBehaviour, IDamageable
{
    [Header("Target")]
    [SerializeField] private TransformAnchor playerAnchor;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float pathUpdateRate = 0.5f;
    [SerializeField] private float waypointArrivalDistance = 0.2f;

    [Header("Behavior")]
    [SerializeField] private EnemyBehaviorSO behavior;

    [Header("Combat")]
    [SerializeField] private EnemyStatsSO stats;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Debug")]
    [SerializeField] private bool debugDashRays = true;

    // Components
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _col;
    private EnemyAttackHitbox _attackHitbox;

    // Pathfinding
    private List<Vector2> _path;
    private int _currentWaypointIndex;
    private float _nextPathTime;
    private bool _facingRight = true;

    // Dash
    private bool _isDashing;
    private float _dashTimer;
    private float _cooldownTimer;
    private Vector2 _dashDirection;

    // Attack state
    private bool _isAttacking;
    private float _attackCooldownTimer;
    private float _sqrAttackRange;

    // Health & state
    private int _currentHealth;
    private bool _isDead;

    // Dash detection timer (used by DashBehaviorSO)
    public float DashClearTimer;

    private Vector2 _lastVelocity;
    private Vector2 _nextWaypointPos;
    private Vector2[] _cornerOrigins;
    private Vector2[] _cornerLocalOffsets;

    // Animation state machine
    private enum EnemyAnimationState
    {
        Idle,
        Run,
        Attack,      // normal enemy only
        DashAttack   // dash enemy only
    }
    private EnemyAnimationState _currentAnimState;
    private int _idleHash, _runHash, _attackHash, _dashAttackHash;

    // Animator parameter hashes
    private int _moveXHash, _moveYHash, _lastMoveXHash, _lastMoveYHash, _moveMagnitudeHash;

    public Vector2[] GetCornerOrigins() => _cornerOrigins;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _col = GetComponent<Collider2D>();
        _attackHitbox = GetComponentInChildren<EnemyAttackHitbox>();

        _cornerOrigins = new Vector2[4];
        _cornerLocalOffsets = new Vector2[4];

        if (playerAnchor == null) Debug.LogError($"{name}: TransformAnchor missing!", this);
        if (behavior == null) Debug.LogError($"{name}: EnemyBehaviorSO missing!", this);
        if (stats == null) Debug.LogError($"{name}: EnemyStatsSO missing!", this);

        // Cache corner offsets
        Bounds bounds = _col.bounds;
        Vector3 pos = transform.position;
        _cornerLocalOffsets[0] = (Vector2)(bounds.min - pos);
        _cornerLocalOffsets[1] = (Vector2)(new Vector3(bounds.max.x, bounds.min.y) - pos);
        _cornerLocalOffsets[2] = (Vector2)(new Vector3(bounds.min.x, bounds.max.y) - pos);
        _cornerLocalOffsets[3] = (Vector2)(bounds.max - pos);

        _currentHealth = Mathf.RoundToInt(stats.MaxHealth);
        _sqrAttackRange = stats.AttackRange * stats.AttackRange;

        // Cache animation state hashes
        _idleHash = Animator.StringToHash("Idle");
        _runHash = Animator.StringToHash("Run");
        _attackHash = Animator.StringToHash("Attack");
        _dashAttackHash = Animator.StringToHash("DashAttack");

        // Cache animator parameter hashes
        _moveXHash = Animator.StringToHash("MoveX");
        _moveYHash = Animator.StringToHash("MoveY");
        _lastMoveXHash = Animator.StringToHash("LastMoveX");
        _lastMoveYHash = Animator.StringToHash("LastMoveY");
        _moveMagnitudeHash = Animator.StringToHash("MoveMagnitude");

        // Set initial animation state instantly
        PlayAnimation(EnemyAnimationState.Idle, 0f);

        if (_attackHitbox != null)
            _attackHitbox.damage = stats.AttackDamage;
    }

    private void Update()
    {
        // ---- Flipping ----
        if (_rb.linearVelocity.x > 0.01f && !_facingRight)
        {
            _facingRight = true;
            if (_spriteRenderer) _spriteRenderer.flipX = false;
        }
        else if (_rb.linearVelocity.x < -0.01f && _facingRight)
        {
            _facingRight = false;
            if (_spriteRenderer) _spriteRenderer.flipX = true;
        }

        // ---- Feed blend tree parameters ----
        Vector2 directionForBlend;

        if (_isDashing)
        {
            directionForBlend = _dashDirection;
        }
        else if (_isAttacking)
        {
            // During attack, use the last movement direction (already stored)
            directionForBlend = new Vector2(
                animator.GetFloat(_lastMoveXHash),
                animator.GetFloat(_lastMoveYHash)
            );
        }
        else
        {
            Vector2 vel = _rb.linearVelocity;
            if (vel.sqrMagnitude > 0.01f)
            {
                directionForBlend = vel.normalized;
                // Update last move direction when moving
                animator.SetFloat(_lastMoveXHash, directionForBlend.x);
                animator.SetFloat(_lastMoveYHash, directionForBlend.y);
            }
            else
            {
                // Use last stored direction when stationary
                directionForBlend = new Vector2(
                    animator.GetFloat(_lastMoveXHash),
                    animator.GetFloat(_lastMoveYHash)
                );
            }
        }

        animator.SetFloat(_moveXHash, directionForBlend.x);
        animator.SetFloat(_moveYHash, directionForBlend.y);
        animator.SetFloat(_moveMagnitudeHash, _rb.linearVelocity.magnitude);

        // ---- Determine and play animation state ----
        UpdateAnimationState();
    }

    private void FixedUpdate()
    {
        Transform player = playerAnchor?.Value;
        if (player == null)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 playerPos = player.position;
        Vector2 myPos = transform.position;

        // Update corner origins for dash checks
        _cornerOrigins[0] = myPos + _cornerLocalOffsets[0];
        _cornerOrigins[1] = myPos + _cornerLocalOffsets[1];
        _cornerOrigins[2] = myPos + _cornerLocalOffsets[2];
        _cornerOrigins[3] = myPos + _cornerLocalOffsets[3];

        // Cooldowns
        if (_attackCooldownTimer > 0f)
            _attackCooldownTimer -= Time.fixedDeltaTime;

        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.fixedDeltaTime;

        // Dash handling
        if (_isDashing)
        {
            _dashTimer -= Time.fixedDeltaTime;
            if (_dashTimer <= 0f)
                EndDash();
            else
                _rb.linearVelocity = behavior.UpdateDash(this, _dashDirection, Time.fixedDeltaTime);
            return;
        }

        // Attack (stops movement) – only used by normal enemies
        if (_isAttacking)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        // Movement & pathfinding
        if (Time.time >= _nextPathTime)
        {
            RequestPath(playerPos);
            _nextPathTime = Time.time + pathUpdateRate;
        }
        MoveAlongPath();

        // Try dash
        if (_cooldownTimer <= 0f)
        {
            bool shouldDash = false;
            Vector2 dashDir = Vector2.zero;
            behavior.TryDash(this, playerPos, Time.fixedDeltaTime, ref shouldDash, ref dashDir);
            if (shouldDash) StartDash(dashDir);
        }

        // Attack check – only for normal enemies (dash enemies never attack this way)
        if (!_isAttacking && _attackCooldownTimer <= 0f)
        {
            if (!(behavior is DashBehaviorSO))
            {
                float sqrDist = (playerPos - myPos).sqrMagnitude;
                if (sqrDist <= _sqrAttackRange)
                {
                    StartAttack();
                }
            }
        }

        _lastVelocity = _rb.linearVelocity;
        _nextWaypointPos = (_path != null && _currentWaypointIndex < _path.Count)
            ? _path[_currentWaypointIndex]
            : myPos;
    }

    // ---------- Animation State Machine ----------
    private void UpdateAnimationState()
    {
        if (_isDead) return;

        if (_isDashing && behavior is DashBehaviorSO)
        {
            PlayAnimation(EnemyAnimationState.DashAttack);
            return;
        }

        if (_isAttacking && !(behavior is DashBehaviorSO))
        {
            PlayAnimation(EnemyAnimationState.Attack);
            return;
        }

        // Idle or Run based on velocity
        bool moving = _rb.linearVelocity.sqrMagnitude > 0.01f;
        PlayAnimation(moving ? EnemyAnimationState.Run : EnemyAnimationState.Idle);
    }

    private void PlayAnimation(EnemyAnimationState state, float fadeDuration = 0.1f)
    {
        if (_currentAnimState == state) return;
        _currentAnimState = state;

        int hash = state switch
        {
            EnemyAnimationState.Idle => _idleHash,
            EnemyAnimationState.Run => _runHash,
            EnemyAnimationState.Attack => _attackHash,
            EnemyAnimationState.DashAttack => _dashAttackHash,
            _ => _idleHash
        };

        animator.CrossFadeInFixedTime(hash, fadeDuration, 0);
    }

    // ---------- Attack (normal enemy) ----------
    private void StartAttack()
    {
        _isAttacking = true;
        _attackCooldownTimer = stats.AttackCooldown;

        // Freeze the animation direction to the current facing
        Vector2 dir = new Vector2(
            animator.GetFloat(_lastMoveXHash),
            animator.GetFloat(_lastMoveYHash)
        );
        animator.SetFloat(_moveXHash, dir.x);
        animator.SetFloat(_moveYHash, dir.y);

        if (_attackHitbox != null)
            _attackHitbox.BeginSwing();
    }

    public void EndAttack()
    {
        _isAttacking = false;
        if (_attackHitbox != null)
            _attackHitbox.Disable();
    }

    public void EnableHitbox()
    {
        if (_attackHitbox != null) _attackHitbox.Enable();
    }

    public void DisableHitbox()
    {
        if (_attackHitbox != null) _attackHitbox.Disable();
    }

    // ---------- Dash (all enemies) ----------
    private void StartDash(Vector2 direction)
    {
        _isDashing = true;
        _dashTimer = (behavior is DashBehaviorSO dashBeh) ? dashBeh.DashDuration : 0.2f;
        _dashDirection = direction;

        // Set initial velocity
        if (behavior is DashBehaviorSO)
            _rb.linearVelocity = _dashDirection * ((DashBehaviorSO)behavior).DashSpeed;
        else
            _rb.linearVelocity = _dashDirection * 10f; // fallback

        // Dash enemy: hitbox active during dash
        if (behavior is DashBehaviorSO && _attackHitbox != null)
        {
            _attackHitbox.BeginSwing();
            _attackHitbox.Enable();
        }
    }

    private void EndDash()
    {
        _isDashing = false;
        _cooldownTimer = (behavior is DashBehaviorSO dashBeh) ? dashBeh.DashCooldown : 2f;

        if (_attackHitbox != null)
            _attackHitbox.Disable();

        _rb.linearVelocity = Vector2.zero;
    }

    // ---------- Pathfinding ----------
    private void RequestPath(Vector2 target)
    {
        if (GridAStar.Instance == null) return;
        _path = GridAStar.Instance.FindPath(transform.position, target);
        if (_path == null || _path.Count == 0)
        {
            if (!_isDashing) _rb.linearVelocity = Vector2.zero;
            return;
        }
        _currentWaypointIndex = 0;
    }

    private void MoveAlongPath()
    {
        if (_isDashing) return;
        if (_path == null || _currentWaypointIndex >= _path.Count)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 targetWaypoint = _path[_currentWaypointIndex];
        Vector2 direction = (targetWaypoint - (Vector2)transform.position).normalized;
        _rb.linearVelocity = direction * moveSpeed;

        if (Vector2.Distance(transform.position, targetWaypoint) <= waypointArrivalDistance)
            _currentWaypointIndex++;
    }

    // ---------- IDamageable ----------
    public void TakeDamage(int amount, GameObject source)
    {
        _currentHealth -= amount;
        Debug.Log($"[{name}] took {amount} damage, HP: {_currentHealth}/{stats.MaxHealth}");

        if (_currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        if (_isDead) return;
        _isDead = true;

        _isDashing = false;
        _isAttacking = false;
        _rb.linearVelocity = Vector2.zero;

        if (_attackHitbox != null)
            _attackHitbox.Disable();

        Debug.Log($"[{name}] died!");
        gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        if (_path != null && _path.Count > 1)
        {
            Gizmos.color = Color.green;
            for (int i = 1; i < _path.Count; i++)
                Gizmos.DrawLine(_path[i - 1], _path[i]);
        }

        Gizmos.color = Color.yellow;
        Vector3 pos = transform.position;
        Gizmos.DrawLine(pos, pos + (Vector3)_lastVelocity);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, _nextWaypointPos);

        if (_cornerOrigins != null && playerAnchor?.Value != null && behavior is DashBehaviorSO)
        {
            Vector2 playerPos = playerAnchor.Value.position;
            Vector2 dir = (playerPos - (Vector2)pos).normalized;
            float drawLen = Mathf.Min(Vector2.Distance((Vector2)pos, playerPos) + 0.5f, 10f);
            Gizmos.color = _isDashing ? Color.cyan : (_cooldownTimer > 0f ? Color.red : Color.yellow);
            for (int i = 0; i < _cornerOrigins.Length; i++)
                Gizmos.DrawLine(_cornerOrigins[i], _cornerOrigins[i] + dir * drawLen);
        }
    }
#endif
}