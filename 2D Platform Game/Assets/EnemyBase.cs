using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public PatrolPath m_patrolPath;
    public GameObject m_fireballPrefab;

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

    // Devriye de�i�kenleri
    protected bool m_isIdle = false;
    protected float m_idleTimer = 0f;
    public float m_idleWaitTime = 1f;

    // Oyuncu alg�lama mesafesi
    public float m_detectionDistance = 5f;
    public float m_distanceAngle = 90f;

    protected Coroutine m_currentAttackCoroutine; // Aktif sald�r� coroutine'i

    protected virtual void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody2D>();
        m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_meleeRangeSensor = transform.Find("MeleeRangeSensor").GetComponent<SensorPlayer>();
    }

    protected virtual void Start()
    {
        if (m_patrolPath != null)
        {
            m_facingDirection = 1; // Ba�lang�� y�n� sa�
            transform.position = new Vector2(m_patrolPath.startPosition.x, transform.position.y);
        }
    }

    private void Update()
    {
        if (m_isDead || m_health <= 0) return;

        // Ana raycast alg�lama
        m_playerDetected = RaycastHelper.IsTargetInView(
            (Vector2)transform.position,
            m_playerTransform,
            m_detectionDistance, // G�r�� mesafesi
            m_distanceAngle,     // G�r�� a��s�
            m_facingDirection,   // D��man�n bakt��� y�n
            LayerMask.GetMask("Player")
        );

        // Yak�n �evresel raycast alg�lama (m_detectionDistance / 2)
        if (!m_playerDetected)
        {
            m_playerDetected = RaycastHelper.IsTargetInView(
                (Vector2)transform.position,
                m_playerTransform,
                m_detectionDistance / 2f, // Daha k�sa mesafede kontrol
                360f,                     // T�m y�nlerde (tam �ember)
                1,                        // Her zaman sa� (d�z vekt�r)
                LayerMask.GetMask("Player")
            );
        }

        // Oyuncu alg�land�ysa
        if (m_playerDetected)
        {
            EngagePlayer();
        }
        else
        {
            Patrol();
        }
    }


    public void TakeDamage(float damage)
    {
        if (m_isDead || m_health <= 0)
            return;

        if (m_isAttacking)
        {
            // E�er sald�r� coroutine'i �al���yorsa iptal et
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

        m_rb.velocity = new Vector2(m_speed * direction, m_rb.velocity.y);
    }

    protected virtual void MoveAwayFromPlayer(float direction)
    {
        if (m_patrolPath == null)
            return;

        // G�ncel pozisyon
        Vector2 currentPosition = transform.position;

        // Start ve End pozisyonlar�na uzakl�klar� hesapla
        float distanceToStart = Vector2.Distance(currentPosition, m_patrolPath.startPosition);
        float distanceToEnd = Vector2.Distance(currentPosition, m_patrolPath.endPosition);

        // En uzak noktaya ka�
        Vector2 targetPosition;
        if (distanceToStart > distanceToEnd)
        {
            targetPosition = m_patrolPath.startPosition;
        }
        else
        {
            targetPosition = m_patrolPath.endPosition;
        }

        // Y�z y�n�n� oyuncuya g�re sabit tut
        float directionToPlayer = Mathf.Sign(m_playerTransform.position.x - transform.position.x);
        m_facingDirection = (int)directionToPlayer;
        FlipSprite();

        // Hedef pozisyona do�ru h�zland�r�lm�� bir �ekilde hareket et
        float retreatSpeed = m_speed * 2f; // Ka��� h�z�
        Vector2 retreatDirection = (targetPosition - currentPosition).normalized;
        m_rb.velocity = new Vector2(retreatDirection.x * retreatSpeed, m_rb.velocity.y);

        // Devriye s�n�rlar�n�n d���na ��k�lmas�n� �nle
        float clampedX = Mathf.Clamp(transform.position.x, m_patrolPath.startPosition.x, m_patrolPath.endPosition.x);
        transform.position = new Vector2(clampedX, transform.position.y);
    }




    private void OnDrawGizmos()
    {
        // Ana raycast g�r�� alan�n� �iz
        RaycastHelper.DrawGizmos(
            (Vector2)transform.position,
            m_detectionDistance, // G�r�� mesafesi
            m_distanceAngle,     // G�r�� a��s�
            m_facingDirection    // D��man�n bakt��� y�n
        );

        // Yak�n �evresel alg�lama �emberini �iz
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_detectionDistance / 2f);
    }

}
