using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float m_speed = 5f;
    public float m_damage = 10f;
    public int m_attackerfaceDirection;

    private Animator m_animator;
    private bool m_isDestroyed = false;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!m_isDestroyed)
        {
            float direction = m_attackerfaceDirection == 1 ? 1 : -1;
            transform.Translate(Vector2.right * direction * m_speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerActionHandler player = collision.GetComponent<PlayerActionHandler>();
            if (player != null)
            {
                player.GetDamage(m_damage, this.transform);
            }
            TriggerDestruction();
        }
        else if (collision.CompareTag("Grid"))
        {
            TriggerDestruction();
        }
    }

    private void TriggerDestruction()
    {
        if (m_isDestroyed) return;

        m_isDestroyed = true;
        m_speed = 0;
        m_animator.SetTrigger("Collided");
        Destroy(gameObject, m_animator.GetCurrentAnimatorStateInfo(0).length);
    }
}
