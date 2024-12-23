using System.Collections;
using UnityEngine;

public class MushroomEnemy : EnemyBase
{
    private bool m_useAttack1 = true;

    private float m_lastAttackTime;
    private float m_damage = 100f;
    private float m_deathOffsetX = 0.0015f;
    private float m_deathOffsetY = -0.2579f;
    private float m_deathSizeX = 0.03f;
    private float m_deathSizeY = 0.005f;

    protected override void Start()
    {
        base.Start();
        // Mushroom özel statlar
        m_speed = 1.5f;
        m_health = 75f;
    }

    protected override void Patrol()
    {
        if (m_isIdle)
        {
            m_idleTimer -= Time.deltaTime;
            if (m_idleTimer <= 0f)
            {
                m_isIdle = false;
                m_animator.SetBool("Run", true);
                m_facingDirection *= -1;
                FlipSprite();
            }
            return;
        }

        if (m_patrolPath == null)
            return;

        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = m_facingDirection > 0 ? m_patrolPath.endPosition : m_patrolPath.startPosition;

        float step = m_speed * Time.deltaTime;
        float newX = Mathf.MoveTowards(currentPosition.x, targetPosition.x, step);
        transform.position = new Vector2(newX, currentPosition.y);

        if (Mathf.Abs(newX - targetPosition.x) < 0.1f)
        {
            m_isIdle = true;
            m_idleTimer = m_idleWaitTime;
            m_rb.velocity = Vector2.zero;
            m_animator.SetBool("Run", false);
        }
        else
        {
            m_animator.SetBool("Run", true);
        }
    }

    protected override void EngagePlayer()
    {
        float directionToPlayer = Mathf.Sign(m_playerTransform.position.x - transform.position.x);
        m_facingDirection = (int)directionToPlayer;
        FlipSprite();

        bool inMeleeRange = m_meleeRangeSensor.State();
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

    private void MeleeAttack()
    {
        if (m_isAttacking || Time.time - m_lastAttackTime < 1f)
            return;

        m_isAttacking = true;
        m_lastAttackTime = Time.time;

        string attackAnim = m_useAttack1 ? "Attack1" : "Attack2";
        m_useAttack1 = !m_useAttack1;

        m_animator.SetTrigger(attackAnim);

        m_currentAttackCoroutine = StartCoroutine(PerformMeleeDamage());
    }

    private IEnumerator PerformMeleeDamage()
    {
        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);

        foreach (var col in m_meleeRangeSensor.Colliders)
        {
            if (col.CompareTag("Player"))
            {
                var playerHealth = col.GetComponent<PlayerHealthManager>();
                playerHealth?.ReceiveDamage(m_damage, transform);
            }
        }

        m_isAttacking = false;
    }

    protected override IEnumerator Die()
    {
        m_isDead = true;
        m_animator.SetTrigger("Death");

        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);

        m_rb.velocity = Vector2.zero;
        transform.Find("MeleeRangeSensor").gameObject.SetActive(false);

        // Collider ayarlarý
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.offset = new Vector2(m_deathOffsetX, m_deathOffsetY);
            col.size = new Vector2(m_deathSizeX, m_deathSizeY);
        }

        yield return new WaitForSeconds(stateInfo.length);

        this.enabled = false;
    }
}
