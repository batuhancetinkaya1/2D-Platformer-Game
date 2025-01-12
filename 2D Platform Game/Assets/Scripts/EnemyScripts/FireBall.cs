using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Fireball : MonoBehaviour
{
    public float m_speed = 5f;
    public float m_damage = 10f;
    public int m_attackerfaceDirection = 1;

    private Animator m_animator;
    private bool m_isDestroyed = false;
    private Collider2D m_collider2D;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_collider2D = GetComponent<Collider2D>();

        // Eðer patlayýnca collider devredýþý kalacaksa isTrigger = true kullanýlabilir.
        // Aksi halde OnCollisionEnter2D vb. kullanýlabilir. Þimdilik OnTriggerEnter2D üzerinden ilerliyoruz.
        m_collider2D.isTrigger = true;
    }

    private void Update()
    {
        if (!m_isDestroyed)
        {
            float direction = (m_attackerfaceDirection >= 0) ? 1f : -1f;
            transform.Translate(Vector2.right * direction * m_speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_isDestroyed) return;

        // Duvar veya grid
        if (collision.CompareTag("Grid"))
        {
            StartCoroutine(DestroyFireball());
        }
        else if (collision.CompareTag("Player"))
        {
            PlayerHealthManager playerHealth = collision.GetComponent<PlayerHealthManager>();
            if (playerHealth != null)
            {
                // Hasar uygula
                playerHealth.ReceiveDamage(m_damage, this.transform);
            }
            StartCoroutine(DestroyFireball());
        }
    }

    private IEnumerator DestroyFireball()
    {
        m_isDestroyed = true;
        m_speed = 0f;

        // Patlama animasyonu tetikle
        if (m_animator != null)
            m_animator.SetTrigger("Collided");

        // Collider devre dýþý
        m_collider2D.enabled = false;

        // Animasyonun bitmesini bekle (Örn. 0.5-1sn)
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }
}
