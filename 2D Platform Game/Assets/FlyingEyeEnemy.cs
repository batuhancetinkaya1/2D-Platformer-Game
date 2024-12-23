using System.Collections;
using UnityEngine;

public class FlyingEyeEnemy : EnemyBase
{
    private bool m_useAttack1 = true;

    private float m_lastAttackTime;
    private float m_meleeRange = 1.5f; // Yakýn saldýrý mesafesi
    private float m_damage = 5f;
    private float m_deathOffsetX = 0.05f;
    private float m_deathOffsetY = -0.15f;
    private float m_deathSizeX = 0.1f;
    private float m_deathSizeY = 0.1f;

    protected override void Start()
    {
        base.Start();
        // FlyingEye özel statlar
        m_speed = 2f;      // Normal hýzý
        m_health = 45f;    // Caný
    }

    protected override void Patrol()
    {
        if (m_isIdle)
        {
            m_idleTimer -= Time.deltaTime;
            if (m_idleTimer <= 0f)
            {
                m_isIdle = false;
                // Yeni devriye turunda ters yönde git
                m_facingDirection *= -1;
                FlipSprite();
            }
            return;
        }

        if (m_patrolPath == null)
            return;

        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = (m_facingDirection > 0)
            ? m_patrolPath.endPosition
            : m_patrolPath.startPosition;

        float step = m_speed * Time.deltaTime;
        float newX = Mathf.MoveTowards(currentPosition.x, targetPosition.x, step);
        transform.position = new Vector2(newX, currentPosition.y);

        if (Mathf.Abs(newX - targetPosition.x) < 0.1f)
        {
            m_isIdle = true;
            m_idleTimer = m_idleWaitTime;
            m_rb.velocity = Vector2.zero;
        }
    }

    protected override void EngagePlayer()
    {
        float distToPlayer = Vector2.Distance(m_playerTransform.position, transform.position);
        float directionToPlayer = Mathf.Sign(m_playerTransform.position.x - transform.position.x);

        // Yüzü daima player’a dönük tut
        m_facingDirection = (int)directionToPlayer;
        FlipSprite();

        bool inMeleeRange = distToPlayer < m_meleeRange;
        bool shouldRetreat = ShouldRetreat(inMeleeRange, 15f);

        // Rastgele karar
        float randomDecision = Random.Range(0f, 1f);

        if (shouldRetreat && randomDecision < 0.6f)
        {
            // 2x hýzlý bir þekilde “en uzak patrol uç noktasýna” kaç
            MoveAwayFromPlayer(-directionToPlayer);
        }
        else if (inMeleeRange)
        {
            // Yakýn mesafede Attack2
            FlyingEyeMeleeAttack();
        }
        else
        {
            // Menzilli saldýrý
            // Attack1
            if (randomDecision < 0.5f)
            {
                // Sabit kalýp range attack
                StartCoroutine(RangedAttack());
            }
            else
            {
                // Biraz yanadursun, velocity 0
                m_rb.velocity = Vector2.zero;
            }
        }
    }

    private void FlyingEyeMeleeAttack()
    {
        if (m_isAttacking || Time.time - m_lastAttackTime < 1f)
            return;

        m_isAttacking = true;
        m_lastAttackTime = Time.time;

        // Yakýn saldýrý animasyonu => Attack2
        m_animator.SetTrigger("Attack2");
        m_currentAttackCoroutine = StartCoroutine(PerformFlyingEyeMeleeDamage());
    }

    private IEnumerator PerformFlyingEyeMeleeDamage()
    {
        // Animasyon süresini al
        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length * 0.5f);
        // Attack vuruþ zamanlamasý tam ortada diyelim

        // Hasar ver
        foreach (var col in m_meleeRangeSensor.Colliders)
        {
            if (col.CompareTag("Player"))
            {
                PlayerHealthManager playerHealth = col.GetComponent<PlayerHealthManager>();
                if (playerHealth != null)
                {
                    playerHealth.ReceiveDamage(m_damage, transform);
                }
            }
        }

        yield return new WaitForSeconds(stateInfo.length * 0.5f);
        m_isAttacking = false;
    }

    private IEnumerator RangedAttack()
    {
        if (m_isAttacking || Time.time - m_lastAttackTime < 2f)
            yield break;

        m_isAttacking = true;
        m_lastAttackTime = Time.time;

        m_animator.SetTrigger("Attack1");

        // Animasyon baþlama
        yield return new WaitForSeconds(0.3f); // Attack1 animasyonunun baþýnda fireball ateþleyelim

        // Fireball
        if (m_fireballPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.right * m_facingDirection;
            GameObject fireballObj = Instantiate(m_fireballPrefab, spawnPos, Quaternion.identity);
            fireballObj.GetComponent<Fireball>().m_attackerfaceDirection = m_facingDirection;
        }

        // Animasyon tamamlanana kadar bekle
        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length - 0.3f);

        m_isAttacking = false;
    }

    protected override IEnumerator Die()
    {
        m_isDead = true;
        m_animator.SetTrigger("Death");

        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);

        m_rb.velocity = Vector2.zero;
        m_rb.gravityScale = 1;
        if (m_meleeRangeSensor != null)
            m_meleeRangeSensor.gameObject.SetActive(false);

        // Collider ayarlarý
        CapsuleCollider2D col = GetComponent<CapsuleCollider2D>();
        if (col != null)
        {
            col.offset = new Vector2(m_deathOffsetX, m_deathOffsetY);
            col.size = new Vector2(m_deathSizeX, m_deathSizeY);
        }

        yield return new WaitForSeconds(stateInfo.length);
        this.enabled = false;
    }

    // Override MoveAwayFromPlayer: 2x speed, en uzak uç nokta
    protected override void MoveAwayFromPlayer(float direction)
    {
        if (m_patrolPath == null)
            return;

        Vector2 currentPos = transform.position;
        float distToStart = Vector2.Distance(currentPos, m_patrolPath.startPosition);
        float distToEnd = Vector2.Distance(currentPos, m_patrolPath.endPosition);

        // En uzak noktayý bul
        Vector2 targetPos = (distToStart > distToEnd)
            ? m_patrolPath.startPosition
            : m_patrolPath.endPosition;

        float retreatSpeed = m_speed * 2f; // Kaçarken 2x
        Vector2 retreatDir = (targetPos - currentPos).normalized;
        m_rb.velocity = new Vector2(retreatDir.x * retreatSpeed, m_rb.velocity.y);

        // Yüzü daima player’a dönük => Yani “m_facingDirection” tekrar player’a baksýn
        float directionToPlayer = Mathf.Sign(m_playerTransform.position.x - transform.position.x);
        m_facingDirection = (int)directionToPlayer;
        FlipSprite();

        // Sýnýr
        float clampedX = Mathf.Clamp(transform.position.x,
                                     Mathf.Min(m_patrolPath.startPosition.x, m_patrolPath.endPosition.x),
                                     Mathf.Max(m_patrolPath.startPosition.x, m_patrolPath.endPosition.x));
        transform.position = new Vector2(clampedX, transform.position.y);
    }
}
