using UnityEngine;

public class CatController : MonoBehaviour
{
    private Animator animator;
    private Collider2D catCollider;

    // Koþu animasyonu vb için kullandýðýnýz deðiþkenler
    private bool isRunning;

    // Inspector’dan ayarlayabiliriz
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float idleWaitTime = 5f;

    // “Her animasyonun 5 katý kadar oynasýn” dediðiniz factor
    [SerializeField] private float animLoopFactor = 7.5f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        catCollider = GetComponent<Collider2D>();

        // Sürekli animasyon döngüsünü burada baþlatýyoruz
        StartCoroutine(AnimationLoopCoroutine());
    }

    private void Update()
    {
        // Koþma durumu vs için yine Update’te bakabiliriz
        if (isRunning)
        {
            MoveLeft();
        }
    }

    private System.Collections.IEnumerator AnimationLoopCoroutine()
    {
        while (true)
        {
            // 1) Rastgele bir anim state seç (1–4 arasý)
            int randomState = Random.Range(1, 5);
            animator.SetInteger("AnimState", randomState);

            // 2) O animasyonun süresini al
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            float animDuration = info.length;

            // Animasyon “5 katý” kadar dönecek
            float playTime = animDuration * animLoopFactor;

            // 3) Bu süre kadar bekle
            yield return new WaitForSeconds(playTime);

            // 4) Sonra animState = -1 yap (idle’a geçsin)
            animator.SetInteger("AnimState", -1);

            // 5) Idle animasyonunu oynamasý için belli süre bekle (idleWaitTime)
            yield return new WaitForSeconds(idleWaitTime);

            // Sonra döngü baþa dönecek ve tekrar rastgele state seçecek
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && UIManager.Instance.StarCount() >= 0)
        {
            AudioManager.Instance.PlaySFXWithNewSource("Meow", transform.position);
            UIManager.Instance.RemoveStars(5);
            TriggerRunState();
        }
    }

    private void TriggerRunState()
    {
        catCollider.enabled = false;
        isRunning = true;
        animator.SetTrigger("Run");
    }

    private void MoveLeft()
    {
        transform.position += Vector3.left * runSpeed * Time.deltaTime;
    }
}
