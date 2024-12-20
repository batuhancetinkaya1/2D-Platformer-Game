using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public PatrolPath m_patrolPath;
    public GameObject m_fireballPrefab;
    public Transform m_fireballSpawnPoint;

    public float m_health;
    public float m_speed = 1f;
    public float m_rangedRange = 5f;

    public int m_facingDirection;

    protected PatrolPath.Mover m_mover;
    protected Animator m_animator;
    protected Rigidbody2D m_rb;
    protected Transform m_player;

    protected bool m_playerDetected = false;

    protected virtual void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        if (m_patrolPath != null)
        {
            m_mover = m_patrolPath.CreateMover(m_speed);
        }
    }

    private void Update()
    {
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

    protected virtual void Patrol()
    {
        if (m_mover == null || m_patrolPath == null) return;

        Vector2 targetPosition = m_mover.Position;
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        m_rb.velocity = direction * m_speed;

        GetComponent<SpriteRenderer>().flipX = direction.x < 0;
        m_facingDirection = direction.x < 0 ? -1 : 1;

        m_animator.SetBool("IsMoving", true);

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            m_mover = m_patrolPath.CreateMover(m_speed);
        }
    }

    public void TakeDamage(float damage)
    {
        m_health -= damage;
        m_animator.SetTrigger("TakeHit");
        if (m_health <= 0) Die();
    }

    protected virtual void Die()
    {
        m_animator.SetTrigger("Death");
        m_rb.velocity = Vector2.zero;

        if (GetComponent<Collider2D>() != null)
            GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, 2f);
    }

    protected virtual void MoveTowardsPlayer(float direction)
    {
        m_rb.velocity = new Vector2(m_speed * direction, 0);
        m_animator.SetBool("IsMoving", true);
    }
}
