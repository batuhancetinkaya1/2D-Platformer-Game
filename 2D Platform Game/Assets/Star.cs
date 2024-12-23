using UnityEngine;
using DG.Tweening; // DoTween k�t�phanesi gerekli

[RequireComponent(typeof(Collider2D))]
public class Star : MonoBehaviour, ICollectable
{
    [Header("Coin Settings")]
    [SerializeField] private int m_coinValue = 1;
    [SerializeField] private float m_bounceHeight = 0.2f; // Yukar�-a�a�� mesafe
    [SerializeField] private float m_bounceDuration = 1f; // Yukar�-a�a�� s�resi
    [SerializeField] private float m_rotationSpeed = 20f; // D�n�� h�z� (iste�e ba�l�)

    private Vector3 m_startPosition;

    private void Start()
    {
        m_startPosition = transform.position;

        // Yukar�-a�a�� hareket (DoTween Yoyo)
        transform.DOMoveY(m_startPosition.y + m_bounceHeight, m_bounceDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        // Hafif bir d�nme animasyonu (iste�e ba�l�)
        if (m_rotationSpeed > 0)
        {
            transform.DORotate(new Vector3(0f, 0f, 10f), 1f / m_rotationSpeed, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    public void OnCollect(PlayerCore collector)
    {
        // UIManager �zerinden coin ekleme
        UIManager.Instance.AddStars(m_coinValue);

        // Coin'i yok et
        DOTween.Kill(transform);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCore playerCore = collision.GetComponent<PlayerCore>();
            if (playerCore != null)
            {
                OnCollect(playerCore);
            }
        }
    }
}
