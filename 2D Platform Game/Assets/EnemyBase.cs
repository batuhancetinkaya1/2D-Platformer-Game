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

    // Devriye bekleme ile ilgili deðiþkenler
    protected bool m_isIdle = false;
    protected float m_idleTimer = 0f;
    public float m_idleWaitTime = 1f; // Bekleme süresi

    // Oyuncu tespiti için ek bir mesafe: Oyuncu yakýndaysa düþman yaklaþýr.
    public float m_detectionDistance = 5f;

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
            m_facingDirection = 1; // Baþlangýçta saða bak
            transform.position = new Vector2(m_patrolPath.startPosition.x, transform.position.y);
        }
    }

    private void Update()
    {
        float distToPlayer = Mathf.Abs(m_playerTransform.position.x - transform.position.x);
        // Oyuncu belirli bir mesafedeyse düþman player'ý "tespit etmiþ" olur
        m_playerDetected = distToPlayer < m_detectionDistance;

        if (m_playerDetected)
        {
            EngagePlayer();
        }
        else
        {
            Patrol();
        }
    }

    protected abstract void EngagePlayer();

    protected abstract void Patrol();

    protected void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * m_facingDirection;
        transform.localScale = scale;
    }

    public void TakeDamage(float damage)
    {
        if (m_health <= 0)
            return;

        m_health -= damage;

        if (m_health <= 0) StartCoroutine(Die());
        else
        {
            m_animator.SetTrigger("TakeHit");
        }
    }

    protected abstract IEnumerator Die();

    protected virtual void MoveTowardsPlayer(float direction)
    {
        // Oyuncuya döndür
        if (Mathf.Sign(direction) != Mathf.Sign(m_facingDirection))
        {
            m_facingDirection = (int)Mathf.Sign(direction);
            FlipSprite();
        }

        m_rb.velocity = new Vector2(m_speed * direction, m_rb.velocity.y);
    }

    protected virtual void MoveAwayFromPlayer(float direction)
    {
        // Oyuncudan uzaklaþýrken de yüzü ayarla
        if (Mathf.Sign(direction) != Mathf.Sign(m_facingDirection))
        {
            m_facingDirection = (int)Mathf.Sign(direction);
            FlipSprite();
        }

        m_rb.velocity = new Vector2(m_speed * direction, m_rb.velocity.y);
    }
}
