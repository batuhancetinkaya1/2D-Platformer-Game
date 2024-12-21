using System.Collections;
using UnityEngine;

public class UnifiedEnemy : EnemyBase
{
    public EnemyType m_enemyType;

    public float m_meleeRange = 1.5f;

    private float m_lastAttackTime;
    private bool m_isAttacking;

    protected override void Start()
    {
        base.Start();

        if (m_enemyType == EnemyType.FlyingEye)
        {
            m_speed = 2f;
            m_health = 45f;
        }
        else if (m_enemyType == EnemyType.Mushroom)
        {
            m_speed = 1.5f;
            m_health = 75f;
        }
    }

    protected override void Patrol()
    {
        if (m_isIdle)
        {
            // Bekleme süresi içinde duruyor
            m_idleTimer -= Time.deltaTime;
            if (m_idleTimer <= 0f)
            {
                m_isIdle = false;
                if (m_enemyType == EnemyType.Mushroom)
                    m_animator.SetBool("Idle", false); // Bekleme bitti, Idle false. Sadece Mushroom için 
                // Bekleme bitince yön deðiþtir
                m_facingDirection *= -1;
                FlipSprite();
            }
            return;
        }

        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = m_facingDirection > 0 ? m_patrolPath.endPosition : m_patrolPath.startPosition;

        float step = m_speed * Time.deltaTime;
        float newXPosition = Mathf.MoveTowards(currentPosition.x, targetPosition.x, step);
        transform.position = new Vector2(newXPosition, currentPosition.y);

        // Hedefe ulaþtýk mý?
        if (Mathf.Abs(newXPosition - targetPosition.x) < 0.1f)
        {
            // 1 saniye Idle bekle
            m_isIdle = true;
            m_idleTimer = m_idleWaitTime;
            m_rb.velocity = Vector2.zero;

            if(m_enemyType == EnemyType.Mushroom)
                m_animator.SetBool("Idle", true); // Beklerken Idle true. Sadece Mushroom için
        }
    }

    protected override void EngagePlayer()
    {
        float directionToPlayer = Mathf.Sign(m_playerTransform.position.x - transform.position.x);

        // Oyuncuya dön
        if (Mathf.Sign(directionToPlayer) != Mathf.Sign(m_facingDirection))
        {
            m_facingDirection = (int)directionToPlayer;
            FlipSprite();
        }

        // Melee sensör durumuna bak
        bool inMeleeRange = m_meleeRangeSensor.State();

        if (m_enemyType == EnemyType.FlyingEye)
        {
            if (inMeleeRange)
            {
                // Melee saldýrý sadece sensör tetikliyse
                FlyingEyeMeleeAttack();
            }
            else
            {
                float distance = Mathf.Abs(m_playerTransform.position.x - transform.position.x);
                if (distance < 3f)
                {
                    // Oyuncuya çok yakýn ama sensör tetiklenmediyse saldýrma, sadece uzaklaþ
                    MoveAwayFromPlayer(-directionToPlayer);
                }
                else
                {
                    // Oyuncu yeterince uzakta, ranged attack yap
                    StartCoroutine(RangedAttack());
                }
            }
        }
        else if (m_enemyType == EnemyType.Mushroom)
        {
            if (inMeleeRange)
            {
                m_rb.velocity = Vector2.zero;
                MeleeAttack();
            }
            else
            {
                // Oyuncu algýlandý ama sensörde deðil, yaklaþ ama saldýrma
                MoveTowardsPlayer(directionToPlayer);
            }
        }
    }

    private void MeleeAttack()
    {
        if (Time.time - m_lastAttackTime > 1f)
        {
            m_lastAttackTime = Time.time;

            m_isAttacking = !m_isAttacking;
            string attackAnimation = m_isAttacking ? "Attack1" : "Attack2";

            m_animator.SetTrigger(attackAnimation);

            foreach (var collider in m_meleeRangeSensor.Colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    PlayerActionHandler player = collider.GetComponent<PlayerActionHandler>();
                    player?.GetDamage(10f); // Damage ver
                }
            }
        }
    }

    private void FlyingEyeMeleeAttack()
    {
        // Flying Eye için melee saldýrý: "Attack2"
        if (Time.time - m_lastAttackTime > 1f)
        {
            m_lastAttackTime = Time.time;
            m_animator.SetTrigger("Attack2");

            foreach (var collider in m_meleeRangeSensor.Colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    PlayerActionHandler player = collider.GetComponent<PlayerActionHandler>();
                    player?.GetDamage(5f); // Flying Eye melee damage
                }
            }
        }
    }

    private IEnumerator RangedAttack()
    {
        if (Time.time - m_lastAttackTime > 2f)
        {
            m_lastAttackTime = Time.time;

            m_animator.SetTrigger("Attack1");

            AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);

            yield return new WaitForSeconds(stateInfo.length);

            Vector3 fireballSpawnPosition = transform.position + Vector3.right * m_facingDirection;
            GameObject fireball = Instantiate(m_fireballPrefab, fireballSpawnPosition, Quaternion.identity);
            fireball.GetComponent<Fireball>().m_attackerfaceDirection = m_facingDirection;
        }
    }

    protected override void MoveTowardsPlayer(float direction)
    {
        base.MoveTowardsPlayer(direction);

        if (m_meleeRangeSensor.State())
        {
            m_rb.velocity = Vector2.zero;
        }
    }
    protected override void MoveAwayFromPlayer(float direction)
    {
        base.MoveAwayFromPlayer(direction);


    }

    protected override IEnumerator Die()
    {
        m_animator.SetTrigger("Death");

        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);

        m_rb.velocity = Vector2.zero;
        m_rb.gravityScale = 1;
        transform.Find("MeleeRangeSensor").gameObject.SetActive(false);
        this.enabled = false;

        if (m_enemyType == EnemyType.FlyingEye)
        {
            CapsuleCollider2D colliderToChange = GetComponent<CapsuleCollider2D>();
            if (colliderToChange != null)
            {
                colliderToChange.offset = new Vector2(0.05f, -0.15f);
                colliderToChange.size = new Vector2(0.1f, 0.1f);
            }
        }
        else if (m_enemyType == EnemyType.Mushroom)
        {
            // Eklenen collider ayarlarý
            BoxCollider2D colliderToChange = GetComponent<BoxCollider2D>();
            if (colliderToChange != null)
            {
                colliderToChange.offset = new Vector2(0.0015f, -0.2579f);
                colliderToChange.size = new Vector2(0.03f, 0.005f);
            }
        }
        yield return new WaitForSeconds(stateInfo.length);

        this.enabled = false;
    }
}
