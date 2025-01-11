using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class EnemyBase : MonoBehaviour
{
    public PatrolPath m_patrolPath;
    public GameObject m_fireballPrefab;

    [Header("Stats")]
    public float m_health;
    public float m_speed = 1f;
    public int m_facingDirection;

    protected Animator m_animator;
    protected Rigidbody2D m_rb;
    protected Transform m_playerTransform;
    public SensorPlayer m_meleeRangeSensor;

    protected bool m_playerDetected = false;
    protected bool m_isDead = false;
    protected bool m_isAttacking = false;

    [Header("Idle Settings")]
    protected bool m_isIdle = false;
    protected float m_idleTimer = 0f;
    public float m_idleWaitTime = 1f; // Devriye sýrasýnda bekleme süresi

    [Header("Detection Settings")]
    public float m_detectionDistance = 5f;
    public float m_distanceAngle = 90f;

    protected Coroutine m_currentAttackCoroutine;

    protected virtual void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody2D>();
        
        m_meleeRangeSensor = transform.Find("MeleeRangeSensor").GetComponent<SensorPlayer>();
    }
    private void OnEnable()
    {
        PlayerCore.OnPlayerSpawn += HandlePlayerSpawn;
    }

    private void OnDisable()
    {
        PlayerCore.OnPlayerSpawn -= HandlePlayerSpawn;
    }

    private void HandlePlayerSpawn()
    {
        m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void Start()
    {
        if (m_patrolPath != null)
        {
            m_facingDirection = 1; // Default sað
            transform.position = new Vector2(m_patrolPath.m_startPosition.x, transform.position.y);
        }
    }

    private void Update()
    {
        if (m_isDead || m_health <= 0)
            return;

        // Oyuncu görüþ alanýnda mý?
        m_playerDetected = RaycastHelper.IsTargetInView(
            (Vector2)transform.position,
            m_playerTransform,
            m_detectionDistance,
            m_distanceAngle,
            m_facingDirection,
            LayerMask.GetMask("Player")
        );

        // Ek / yakýndan algýlama
        if (!m_playerDetected)
        {
            m_playerDetected = RaycastHelper.IsTargetInView(
                (Vector2)transform.position,
                m_playerTransform,
                m_detectionDistance / 2f,
                360f,
                1, // sabit
                LayerMask.GetMask("Player")
            );
        }

        if (m_playerDetected)
            EngagePlayer();
        else
            Patrol();
    }

    // Ortak hasar alma
    public void TakeDamage(float damage)
    {
        if (m_isDead || m_health <= 0)
            return;

        // Saldýrý iptal
        if (m_isAttacking)
        {
            if (m_currentAttackCoroutine != null)
            {
                StopCoroutine(m_currentAttackCoroutine);
                m_currentAttackCoroutine = null;
            }
            m_animator.ResetTrigger("Attack1");
            m_animator.ResetTrigger("Attack2");
            m_isAttacking = false;
        }

        m_health -= damage;

        if (m_health <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            AudioManager.Instance.PlaySFXWithNewSource("Cut1", transform.position);
            m_animator.SetTrigger("TakeHit");
        }
    }

    protected abstract void EngagePlayer();
    protected abstract void Patrol();
    protected abstract IEnumerator Die();

    protected void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * m_facingDirection;
        transform.localScale = scale;
    }

    protected virtual void MoveTowardsPlayer(float direction)
    {
        if (Mathf.Sign(direction) != Mathf.Sign(m_facingDirection))
        {
            m_facingDirection = (int)Mathf.Sign(direction);
            FlipSprite();
        }

        m_rb.velocity = new Vector2(m_speed * direction * 2f, m_rb.velocity.y);
    }

    protected virtual void MoveAwayFromPlayer(float direction)
    {
        if (m_patrolPath == null)
            return;

        Vector2 currentPosition = transform.position;
        float distanceToStart = Vector2.Distance(currentPosition, m_patrolPath.m_startPosition);
        float distanceToEnd = Vector2.Distance(currentPosition, m_patrolPath.m_endPosition);

        Vector2 targetPosition;
        if (distanceToStart > distanceToEnd)
            targetPosition = m_patrolPath.m_startPosition;
        else
            targetPosition = m_patrolPath.m_endPosition;

        float directionToPlayer = Mathf.Sign(m_playerTransform.position.x - transform.position.x);
        m_facingDirection = (int)directionToPlayer;
        FlipSprite();

        float retreatSpeed = m_speed * 2f;
        Vector2 retreatDirection = (targetPosition - currentPosition).normalized;
        m_rb.velocity = new Vector2(retreatDirection.x * retreatSpeed, m_rb.velocity.y);

        // Devriye sýnýrýna oturt
        float clampedX = Mathf.Clamp(transform.position.x,
                                     m_patrolPath.m_startPosition.x,
                                     m_patrolPath.m_endPosition.x);
        transform.position = new Vector2(clampedX, transform.position.y);
    }

    protected bool ShouldRetreat(bool inMeleeRange, float healthThreshold = 15f)
    {
        if (m_health <= healthThreshold)
            return true;
        if (inMeleeRange)
            return true;
        return false;
    }

    // (Sahnede çizim - debug amaçlý)
    private void OnDrawGizmos()
    {
        RaycastHelper.DrawGizmos(
            (Vector2)transform.position,
            m_detectionDistance,
            m_distanceAngle,
            m_facingDirection
        );

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_detectionDistance / 2f);
    }
}
