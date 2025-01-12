using System.Collections;
using UnityEngine;

public class FlyingEyeEnemy : EnemyBase
{
    //private bool m_useAttack1 = true;

    private float m_lastAttackTime;
    private float m_meleeRange = 1.5f; // Yak�n sald�r� mesafesi
    private float m_damage = 5f;
    private float m_deathOffsetX = 0.05f;
    private float m_deathOffsetY = -0.15f;
    private float m_deathSizeX = 0.1f;
    private float m_deathSizeY = 0.1f;

    // ---- Eklenen mesafe e�ikleri ----
    // "�ok yak�n" mesafe (ka� ya da melee sald�r)
    // �rnek olarak meleeRange * 2 kullan�yoruz.
    private float m_closeDistanceThreshold;

    // "Yeterince uzak" mesafe (range attack)
    // �rnek olarak detectionDistance'�n bir oran�.
    private float m_farDistanceThreshold;

    protected override void Start()
    {
        base.Start();
        // FlyingEye �zel statlar
        m_speed = 2f;      // Normal h�z�
        m_health = 45f;    // Can�

        // E�ik de�erlerini iste�inize g�re uyarlayabilirsiniz
        m_closeDistanceThreshold = m_meleeRange * 2f;
        m_farDistanceThreshold = m_detectionDistance * 0.8f;
    }

    /// <summary>
    /// ENGAGE PLAYER - "�ok �ok �ok �ok daha geli�mi�" versiyon
    /// Player yak�na gelince: b�y�k ihtimal (�rn. %70) ka�, az ihtimalle Attack2
    /// Player uzaktaysa Attack1
    /// Orta menzil aral���nda ise sabit dur
    /// Ne olursa olsun patrol s�n�rlar� d���na ��kmas�n
    /// </summary>
    protected override void EngagePlayer()
    {
        if (m_patrolPath == null)
            return;

        float distToPlayer = Vector2.Distance(m_playerTransform.position, transform.position);
        float directionToPlayer = Mathf.Sign(m_playerTransform.position.x - transform.position.x);

        // Y�z� daima player�a d�n�k tut
        m_facingDirection = (int)directionToPlayer;
        FlipSprite();

        // E�er zaten sald�r� animasyonundaysak, bir �ey yapmayabiliriz:
        if (m_isAttacking)
        {
            return;
        }

        // Yak�n mesafede => genelde ka�ma, bazen melee
        if (distToPlayer < m_closeDistanceThreshold)
        {
            // %70 ka�, %30 Melee Attack
            float randomValue = Random.Range(0f, 1f);
            if (randomValue < 0.7f)
            {
                // Ka� => MoveAwayFromPlayer(�)
                // (Bu metodun i�inde �en uzak u� noktay�� se�iyoruz)
                MoveAwayFromPlayer(-directionToPlayer);
            }
            else
            {
                // Sald�r => Attack2 (melee)
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

        // Son olarak konumumuzu PatrolPath s�n�rlar� i�inde tutal�m
        ClampToPatrolLimits();
    }

    /// <summary>
    /// Oyuncu yak�nda ise Melee Attack
    /// </summary>
    private void FlyingEyeMeleeAttack()
    {
        if (m_isAttacking || Time.time - m_lastAttackTime < 1f)
            return;

        m_isAttacking = true;
        m_lastAttackTime = Time.time;

        // Yak�n sald�r� animasyonu => Attack2
        m_animator.SetTrigger("Attack2");
        m_currentAttackCoroutine = StartCoroutine(PerformFlyingEyeMeleeDamage());
    }

    private IEnumerator PerformFlyingEyeMeleeDamage()
    {
        // Animasyon s�resini al
        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length * 0.5f);
        // Attack vuru� zamanlamas� tam ortada diyelim

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
    /// Range Attack (Attack1) � Fireball vs.
    /// </summary>
    private IEnumerator RangedAttack()
    {
        // �ki sald�r� aras�nda en az 2 saniye olsun (�rn.)
        if (m_isAttacking || Time.time - m_lastAttackTime < 2f)
            yield break;

        m_isAttacking = true;
        m_lastAttackTime = Time.time;

        m_animator.SetTrigger("Attack1");

        // Animasyon ba�lama
        yield return new WaitForSeconds(0.3f); // Attack1 animasyonunun ba��nda fireball ate�leyelim

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
    /// Ka�arken en uzak patrol noktas�n� se�ip 2x h�zda gider,
    /// yine de y�z� player'a d�n�k kal�r,
    /// en son konumu clamp'ler.
    /// </summary>
    protected override void MoveAwayFromPlayer(float direction)
    {
        if (m_patrolPath == null)
            return;

        Vector2 currentPos = transform.position;
        float distToStart = Vector2.Distance(currentPos, m_patrolPath.m_startPosition);
        float distToEnd = Vector2.Distance(currentPos, m_patrolPath.m_endPosition);

        // En uzak noktay� bul
        Vector2 targetPos = (distToStart > distToEnd)
            ? m_patrolPath.m_startPosition
            : m_patrolPath.m_endPosition;

        float retreatSpeed = m_speed * 2f; // Ka�arken 2x
        Vector2 retreatDir = (targetPos - currentPos).normalized;
        m_rb.velocity = new Vector2(retreatDir.x * retreatSpeed, m_rb.velocity.y);

        // Y�z� daima player�a d�n�k
        float directionToPlayer = Mathf.Sign(m_playerTransform.position.x - transform.position.x);
        m_facingDirection = (int)directionToPlayer;
        FlipSprite();

        // S�n�rlar� a�mas�n
        ClampToPatrolLimits();
    }

    /// <summary>
    /// Devriye Path'i s�n�rlar�na ��kmamas� i�in X konumunu clamp eder.
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
        // Orijinal devriye kodlar�n�z:
        if (m_isIdle)
        {
            m_idleTimer -= Time.deltaTime;
            if (m_idleTimer <= 0f)
            {
                m_isIdle = false;
                // Yeni devriye turunda ters y�nde git
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

        // Collider ayarlar�
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
