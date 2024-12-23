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

    // Devriye deðiþkenleri
    protected bool m_isIdle = false;
    protected float m_idleTimer = 0f;
    public float m_idleWaitTime = 1f;

    // Oyuncu algýlama mesafesi
    public float m_detectionDistance = 5f;
    public float m_distanceAngle = 90f;

    protected Coroutine m_currentAttackCoroutine; // Aktif saldýrý coroutine'i

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
            m_facingDirection = 1; // Baþlangýç yönü sað
            transform.position = new Vector2(m_patrolPath.startPosition.x, transform.position.y);
        }
    }

    private void Update()
    {
        if (m_isDead || m_health <= 0) return;

        // Ana raycast algýlama
        m_playerDetected = RaycastHelper.IsTargetInView(
            (Vector2)transform.position,
            m_playerTransform,
            m_detectionDistance, // Görüþ mesafesi
            m_distanceAngle,     // Görüþ açýsý
            m_facingDirection,   // Düþmanýn baktýðý yön
            LayerMask.GetMask("Player")
        );

        // Yakýn çevresel raycast algýlama (m_detectionDistance / 2)
        if (!m_playerDetected)
        {
            m_playerDetected = RaycastHelper.IsTargetInView(
                (Vector2)transform.position,
                m_playerTransform,
                m_detectionDistance / 2f, // Daha kýsa mesafede kontrol
                360f,                     // Tüm yönlerde (tam çember)
                1,                        // Her zaman sað (düz vektör)
                LayerMask.GetMask("Player")
            );
        }

        // Oyuncu algýlandýysa
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
            // Eðer saldýrý coroutine'i çalýþýyorsa iptal et
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

        // Güncel pozisyon
        Vector2 currentPosition = transform.position;

        // Start ve End pozisyonlarýna uzaklýklarý hesapla
        float distanceToStart = Vector2.Distance(currentPosition, m_patrolPath.startPosition);
        float distanceToEnd = Vector2.Distance(currentPosition, m_patrolPath.endPosition);

        // En uzak noktaya kaç
        Vector2 targetPosition;
        if (distanceToStart > distanceToEnd)
        {
            targetPosition = m_patrolPath.startPosition;
        }
        else
        {
            targetPosition = m_patrolPath.endPosition;
        }

        // Yüz yönünü oyuncuya göre sabit tut
        float directionToPlayer = Mathf.Sign(m_playerTransform.position.x - transform.position.x);
        m_facingDirection = (int)directionToPlayer;
        FlipSprite();

        // Hedef pozisyona doðru hýzlandýrýlmýþ bir þekilde hareket et
        float retreatSpeed = m_speed * 2f; // Kaçýþ hýzý
        Vector2 retreatDirection = (targetPosition - currentPosition).normalized;
        m_rb.velocity = new Vector2(retreatDirection.x * retreatSpeed, m_rb.velocity.y);

        // Devriye sýnýrlarýnýn dýþýna çýkýlmasýný önle
        float clampedX = Mathf.Clamp(transform.position.x, m_patrolPath.startPosition.x, m_patrolPath.endPosition.x);
        transform.position = new Vector2(clampedX, transform.position.y);
    }




    private void OnDrawGizmos()
    {
        // Ana raycast görüþ alanýný çiz
        RaycastHelper.DrawGizmos(
            (Vector2)transform.position,
            m_detectionDistance, // Görüþ mesafesi
            m_distanceAngle,     // Görüþ açýsý
            m_facingDirection    // Düþmanýn baktýðý yön
        );

        // Yakýn çevresel algýlama çemberini çiz
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_detectionDistance / 2f);
    }

}
