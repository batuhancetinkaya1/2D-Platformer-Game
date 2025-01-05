using UnityEngine;

public class CatController : MonoBehaviour
{
    private Animator animator;
    private Collider2D catCollider;

    // Ko�u animasyonu vb i�in kulland���n�z de�i�kenler
    private bool isRunning;

    // Inspector�dan ayarlayabiliriz
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float idleWaitTime = 5f;

    // �Her animasyonun 5 kat� kadar oynas�n� dedi�iniz factor
    [SerializeField] private float animLoopFactor = 7.5f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        catCollider = GetComponent<Collider2D>();

        // S�rekli animasyon d�ng�s�n� burada ba�lat�yoruz
        StartCoroutine(AnimationLoopCoroutine());
    }

    private void Update()
    {
        // Ko�ma durumu vs i�in yine Update�te bakabiliriz
        if (isRunning)
        {
            MoveLeft();
        }
    }

    private System.Collections.IEnumerator AnimationLoopCoroutine()
    {
        while (true)
        {
            // 1) Rastgele bir anim state se� (1�4 aras�)
            int randomState = Random.Range(1, 5);
            animator.SetInteger("AnimState", randomState);

            // 2) O animasyonun s�resini al
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            float animDuration = info.length;

            // Animasyon �5 kat�� kadar d�necek
            float playTime = animDuration * animLoopFactor;

            // 3) Bu s�re kadar bekle
            yield return new WaitForSeconds(playTime);

            // 4) Sonra animState = -1 yap (idle�a ge�sin)
            animator.SetInteger("AnimState", -1);

            // 5) Idle animasyonunu oynamas� i�in belli s�re bekle (idleWaitTime)
            yield return new WaitForSeconds(idleWaitTime);

            // Sonra d�ng� ba�a d�necek ve tekrar rastgele state se�ecek
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
