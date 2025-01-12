using System.Collections;
using UnityEngine;

public class FlyingEyeEnemy : EnemyBase
{
    //private bool m_useAttack1 = true;

    private float m_lastAttackTime;
    private float m_meleeRange = 1.5f; // Yakýn saldýrý mesafesi
    private float m_damage = 5f;
    private float m_deathOffsetX = 0.05f;
    private float m_deathOffsetY = -0.15f;
    private float m_deathSizeX = 0.1f;
    private float m_deathSizeY = 0.1f;

    // ---- Eklenen mesafe eþikleri ----
    // "Çok yakýn" mesafe (kaç ya da melee saldýr)
    // Örnek olarak meleeRange * 2 kullanýyoruz.
    private float m_closeDistanceThreshold;

    // "Yeterince uzak" mesafe (range attack)
    // Örnek olarak detectionDistance'ýn bir oraný.
    private float m_farDistanceThreshold;

    protected override void Start()
    {
        base.Start();
        // FlyingEye özel statlar
        m_speed = 2f;      // Normal hýzý
        m_health = 45f;    // Caný

        // Eþik deðerlerini isteðinize göre uyarlayabilirsiniz
        m_closeDistanceThreshold = m_meleeRange * 2f;
        m_farDistanceThreshold = m_detectionDistance * 0.8f;
    }

    /// <summary>
    /// ENGAGE PLAYER - "Çok çok çok çok daha geliþmiþ" versiyon
    /// Player yakýna gelince: büyük ihtimal (örn. %70) kaç, az ihtimalle Attack2
    /// Player uzaktaysa Attack1
    /// Orta menzil aralýðýnda ise sabit dur
    /// Ne olursa olsun patrol sýnýrlarý dýþýna çýkmasýn
    /// </summary>
    protected override void EngagePlayer()
    {
        if (m_patrolPath == null)
            return;

        float distToPlayer = Vector2.Distance(m_playerTransform.position, transform.position);
        float directionToPlayer = Mathf.Sign(m_playerTransform.position.x - transform.position.x);

        // Yüzü daima player’a dönük tut
        m_facingDirection = (int)directionToPlayer;
        FlipSprite();

        // Eðer zaten saldýrý animasyonundaysak, bir þey yapmayabiliriz:
        if (m_isAttacking)
        {
            return;
        }

        // Yakýn mesafede => genelde kaçma, bazen melee
        if (distToPlayer < m_closeDistanceThreshold)
        {
            // %70 kaç, %30 Melee Attack
            float randomValue = Random.Range(0f, 1f);
            if (randomValue < 0.7f)
            {
                // Kaç => MoveAwayFromPlayer(…)
                // (Bu metodun içinde “en uzak uç noktayý” seçiyoruz)
                MoveAwayFromPlayer(-directionToPlayer);
            }
            else
            {
                // Saldýr => Attack2 (melee)
                FlyingEyeMeleeAttack();
            }
        }
        // Uzak mesafede => Range Attack (Attack1)
        else if (distToPlayer > m_farDistanceThreshold)
        {
            StartCoroutine(RangedAttack());
        }
        // Arada kalan mesafede => Sabit durabilir
        else
        {
            m_rb.velocity = Vector2.zero;
        }

        // Son olarak konumumuzu PatrolPath sýnýrlarý içinde tutalým
        ClampToPatrolLimits();
    }

    /// <summary>
    /// Oyuncu yakýnda ise Melee Attack
    /// </summary>
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

    /// <summary>
    /// Range Attack (Attack1) – Fireball vs.
    /// </summary>
    private IEnumerator RangedAttack()
    {
        // Ýki saldýrý arasýnda en az 2 saniye olsun (örn.)
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

    /// <summary>
    /// Kaçarken en uzak patrol noktasýný seçip 2x hýzda gider,
    /// yine de yüzü player'a dönük kalýr,
    /// en son konumu clamp'ler.
    /// </summary>
    protected override void MoveAwayFromPlayer(float direction)
    {
        if (m_patrolPath == null)
            return;

        Vector2 currentPos = transform.position;
        float distToStart = Vector2.Distance(currentPos, m_patrolPath.m_startPosition);
        float distToEnd = Vector2.Distance(currentPos, m_patrolPath.m_endPosition);

        // En uzak noktayý bul
        Vector2 targetPos = (distToStart > distToEnd)
            ? m_patrolPath.m_startPosition
            : m_patrolPath.m_endPosition;

        float retreatSpeed = m_speed * 2f; // Kaçarken 2x
        Vector2 retreatDir = (targetPos - currentPos).normalized;
        m_rb.velocity = new Vector2(retreatDir.x * retreatSpeed, m_rb.velocity.y);

        // Yüzü daima player’a dönük
        float directionToPlayer = Mathf.Sign(m_playerTransform.position.x - transform.position.x);
        m_facingDirection = (int)directionToPlayer;
        FlipSprite();

        // Sýnýrlarý aþmasýn
        ClampToPatrolLimits();
    }

    /// <summary>
    /// Devriye Path'i sýnýrlarýna çýkmamasý için X konumunu clamp eder.
    /// </summary>
    private void ClampToPatrolLimits()
    {
        if (m_patrolPath == null) return;

        float minX = Mathf.Min(m_patrolPath.m_startPosition.x, m_patrolPath.m_endPosition.x);
        float maxX = Mathf.Max(m_patrolPath.m_startPosition.x, m_patrolPath.m_endPosition.x);

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;
    }

    protected override void Patrol()
    {
        // Orijinal devriye kodlarýnýz:
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
            ? m_patrolPath.m_endPosition
            : m_patrolPath.m_startPosition;

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

    protected override IEnumerator Die()
    {
        m_isDead = true;
        AudioManager.Instance.PlaySFXWithNewSource("EyeDie", transform.position);
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
}
