using System.Collections;
using UnityEngine;

public class UnifiedEnemy : EnemyBase
{
    public EnemyType m_enemyType;

    public float m_meleeRange = 1.5f;

    private float m_lastAttackTime;

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
            m_idleTimer -= Time.deltaTime;
            if (m_idleTimer <= 0f)
            {
                m_isIdle = false;

                if (m_enemyType == EnemyType.Mushroom)
                    m_animator.SetBool("Run", true);

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

        if (Mathf.Abs(newXPosition - targetPosition.x) < 0.1f)
        {
            m_isIdle = true;
            m_idleTimer = m_idleWaitTime;
            m_rb.velocity = Vector2.zero;

            if (m_enemyType == EnemyType.Mushroom)
                m_animator.SetBool("Run", false);
        }
        else
        {
            if (m_enemyType == EnemyType.Mushroom)
                m_animator.SetBool("Run", true);
        }
    }

    protected override void EngagePlayer()
    {
        float directionToPlayer = Mathf.Sign(m_playerTransform.position.x - transform.position.x);

        if (Mathf.Sign(directionToPlayer) != Mathf.Sign(m_facingDirection))
        {
            m_facingDirection = (int)directionToPlayer;
            FlipSprite();
        }

        bool inMeleeRange = m_meleeRangeSensor.State();

        if (m_enemyType == EnemyType.Mushroom)
        {
            if (inMeleeRange)
            {
                m_rb.velocity = Vector2.zero;
                m_animator.SetBool("Run", false);
                MeleeAttack();
            }
            else
            {
                m_animator.SetBool("Run", true);
                MoveTowardsPlayer(directionToPlayer);
            }
        }
        else if (m_enemyType == EnemyType.FlyingEye)
        {
            if (inMeleeRange)
            {
                FlyingEyeMeleeAttack();
            }
            else
            {
                StartCoroutine(RangedAttack());
            }
        }
    }

    private void MeleeAttack()
    {
        if (m_isAttacking || Time.time - m_lastAttackTime < 1f)
            return;

        m_isAttacking = true;
        m_lastAttackTime = Time.time;

        m_animator.SetTrigger("Attack1");

        StartCoroutine(PerformMeleeDamage());
    }

    private IEnumerator PerformMeleeDamage()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (var collider in m_meleeRangeSensor.Colliders)
        {
            if (collider.CompareTag("Player"))
            {
                PlayerActionHandler player = collider.GetComponent<PlayerActionHandler>();
                player?.GetDamage(10f);
            }
        }

        m_isAttacking = false;
    }

    private void FlyingEyeMeleeAttack()
    {
        if (m_isAttacking || Time.time - m_lastAttackTime < 1f)
            return;

        m_isAttacking = true;
        m_lastAttackTime = Time.time;

        m_animator.SetTrigger("Attack2");

        StartCoroutine(PerformFlyingEyeMeleeDamage());
    }

    private IEnumerator PerformFlyingEyeMeleeDamage()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (var collider in m_meleeRangeSensor.Colliders)
        {
            if (collider.CompareTag("Player"))
            {
                PlayerActionHandler player = collider.GetComponent<PlayerActionHandler>();
                player?.GetDamage(5f); // Flying Eye melee damage
            }
        }

        m_isAttacking = false;
    }

    private IEnumerator RangedAttack()
    {
        if (m_isAttacking || Time.time - m_lastAttackTime < 2f)
            yield break;

        m_isAttacking = true;
        m_lastAttackTime = Time.time;

        m_animator.SetTrigger("Attack1");

        yield return new WaitForSeconds(0.5f);

        Vector3 fireballSpawnPosition = transform.position + Vector3.right * m_facingDirection;
        GameObject fireball = Instantiate(m_fireballPrefab, fireballSpawnPosition, Quaternion.identity);
        fireball.GetComponent<Fireball>().m_attackerfaceDirection = m_facingDirection;

        m_isAttacking = false;
    }

    protected override IEnumerator Die()
    {
        m_isDead = true;

        m_animator.ResetTrigger("Attack1");
        m_animator.ResetTrigger("Attack2");

        m_animator.SetTrigger("Death");

        m_rb.velocity = Vector2.zero;
        m_rb.gravityScale = 1;
        transform.Find("MeleeRangeSensor").gameObject.SetActive(false);

        yield return new WaitForSeconds(m_animator.GetCurrentAnimatorStateInfo(0).length);

        this.enabled = false;
    }
}
