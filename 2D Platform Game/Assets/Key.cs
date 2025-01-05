using DG.Tweening;
using UnityEngine;

public class Key : MonoBehaviour, ICollectable
{
    [SerializeField] private float m_bounceHeight = 0.2f;
    [SerializeField] private float m_bounceDuration = 1f;
    [SerializeField] private float m_rotationSpeed = 20f;

    private Vector3 m_startPosition;

    private void Start()
    {
        m_startPosition = transform.position;

        transform.DOMoveY(m_startPosition.y + m_bounceHeight, m_bounceDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        if (m_rotationSpeed > 0)
        {
            transform.DORotate(new Vector3(0f, 0f, 10f), 1f / m_rotationSpeed, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    public void OnCollect(PlayerCore collector)
    {
        // UI'da key'i göster
        UIManager.Instance.ShowKey();

        // Spawn point'i deðiþtir
        if (collector.RespawnController != null)
        {
            collector.RespawnController.SetSpawnPointToTwo();
        }

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
                AudioManager.Instance.PlaySFXWithNewSource("Key", transform.position);
                OnCollect(playerCore);
            }
        }
    }

    public void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}