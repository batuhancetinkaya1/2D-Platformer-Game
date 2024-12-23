using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // DoTween kütüphanesi gerekli

public class PlayerHealthManager : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float m_maxHealth = 500f;
    private float m_currentHealth;

    [Header("UI")]
    [SerializeField] private Slider m_healthSlider;
    [SerializeField] private Image m_fillImage;

    [SerializeField] private float m_healthChangeDuration = 0.5f; // Slider'ýn animasyon süresi

    public bool IsBlocking = false;

    private PlayerCore m_playerCore;

    public float CurrentHealth => m_currentHealth;
    public float MaxHealth => m_maxHealth;

    public void Initialize(PlayerCore core)
    {
        m_playerCore = core;
    }

    private void Awake()
    {
        m_currentHealth = m_maxHealth;

        // Saðlýk Slider'ýný otomatik bulma
        if (!m_healthSlider)
        {
            var sliderTrans = transform.Find("Canvas/HealthBar");
            if (sliderTrans != null)
                m_healthSlider = sliderTrans.GetComponent<Slider>();
        }
        if (!m_fillImage && m_healthSlider != null)
            m_fillImage = m_healthSlider.fillRect.GetComponent<Image>();

        if (m_healthSlider != null)
        {
            m_healthSlider.maxValue = m_maxHealth;
            m_healthSlider.value = m_currentHealth;
        }
    }

    public void ReceiveDamage(float damage, Transform attackerTransform = null)
    {
        if (m_playerCore.MovementController.IsRolling)
            return;

        bool isFacingAttack = true;
        int facingDirection = m_playerCore.MovementController.FacingDirection;

        if (attackerTransform != null)
        {
            float deltaX = attackerTransform.position.x - transform.position.x;
            isFacingAttack = (deltaX * facingDirection) > 0;
        }

        if (IsBlocking && isFacingAttack)
        {
            m_currentHealth -= damage / 5f;
            m_playerCore.AnimControl.SetTriggerBlocked();
        }
        else
        {
            m_currentHealth -= damage;
            m_playerCore.AnimControl.HandleHurt();
        }

        m_currentHealth = Mathf.Clamp(m_currentHealth, 0, m_maxHealth);
        UpdateHealthUI();

        if (m_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        m_currentHealth += amount;
        m_currentHealth = Mathf.Clamp(m_currentHealth, 0, m_maxHealth);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (m_healthSlider != null)
        {
            // DoTween ile slider'ý yumuþak bir þekilde güncelle
            m_healthSlider.DOValue(m_currentHealth, m_healthChangeDuration).SetEase(Ease.OutQuad);
        }
    }

    public void Die()
    {
        m_playerCore.AnimControl.HandleDeath();
        m_playerCore.OnPlayerDeath();
    }
}
