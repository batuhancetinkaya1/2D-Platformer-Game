using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
public class Key : MonoBehaviour, ICollectable
{ 
    [SerializeField] private float m_bounceHeight = 0.2f; // Yukarý-aþaðý mesafe
    [SerializeField] private float m_bounceDuration = 1f; // Yukarý-aþaðý süresi
    [SerializeField] private float m_rotationSpeed = 20f; // Dönüþ hýzý (isteðe baðlý)

    private Vector3 m_startPosition;

    private void Start()
    {
        m_startPosition = transform.position;

        // Yukarý-aþaðý hareket (DoTween Yoyo)
        transform.DOMoveY(m_startPosition.y + m_bounceHeight, m_bounceDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        // Hafif bir dönme animasyonu (isteðe baðlý)
        if (m_rotationSpeed > 0)
        {
            transform.DORotate(new Vector3(0f, 0f, 10f), 1f / m_rotationSpeed, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    public void OnCollect(PlayerCore collector)
    {
        // UIManager üzerinden coin ekleme
        UIManager.Instance.ShowKey();

        //key alýndýðýna dair bilgiyi vermeliyiz bu bilgi oyunu bitirirken lazým olacak
        //spawnpoint'i 2 yap ve spawnpoint 2 deki kapýyý aktive et 
        
        // Key yok et
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
