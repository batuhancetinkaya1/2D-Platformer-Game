using UnityEngine;

public class UnifiedEnemy : EnemyBase
{
    public EnemyType m_enemyType;

    public float m_meleeRange = 1.5f;
    public SensorPlayer m_meleeRangeSensor;

    protected override void Start()
    {
        base.Start();

        if (m_enemyType == EnemyType.FlyingEye)
        {
            m_speed = 2f;
            m_health = 45f; // Flying Eye specific health
        }
        else if (m_enemyType == EnemyType.Mushroom)
        {
            m_speed = 1.5f;
            m_health = 75f; // Mushroom specific health
        }
    }

    protected override void EngagePlayer()
    {
        float distanceToPlayer = Mathf.Abs(transform.position.x - m_player.position.x);
        Vector2 directionToPlayer = (m_player.position - transform.position).normalized;

        GetComponent<SpriteRenderer>().flipX = directionToPlayer.x < 0;

        if (m_meleeRangeSensor.State())
        {
            MeleeAttack();
        }
        else if (distanceToPlayer <= m_rangedRange && !m_meleeRangeSensor.State())
        {
            if (m_enemyType == EnemyType.FlyingEye)
            {
                RangedAttack();
            }
            else
            {
                MoveTowardsPlayer(directionToPlayer.x);
            }
        }
        else
        {
            MoveTowardsPlayer(directionToPlayer.x);
        }
    }

    private void MeleeAttack()
    {
        m_animator.SetTrigger("Attack1");
        m_rb.velocity = Vector2.zero;

        if (m_meleeRangeSensor.LastCollider != null &&
            m_meleeRangeSensor.LastCollider.CompareTag("Player"))
        {
            PlayerActionHandler targetPlayer =
                m_meleeRangeSensor.LastCollider.GetComponent<PlayerActionHandler>();

            if (targetPlayer != null)
            {
                targetPlayer.GetDamage(10, this.transform);
            }
        }
    }

    private void RangedAttack()
    {
        m_animator.SetTrigger("Attack2");

        if (m_fireballPrefab && m_fireballSpawnPoint)
        {
            GameObject fireball = Instantiate(m_fireballPrefab, m_fireballSpawnPoint.position, Quaternion.identity);
            fireball.GetComponent<Fireball>().m_attackerfaceDirection = m_facingDirection;
        }
    }

    protected override void MoveTowardsPlayer(float direction)
    {
        m_rb.velocity = new Vector2(m_speed * direction, 0);
        m_animator.SetBool("IsMoving", true);
    }
}
