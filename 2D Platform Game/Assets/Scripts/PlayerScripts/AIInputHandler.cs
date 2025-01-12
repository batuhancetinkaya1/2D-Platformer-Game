using System.Collections;
using UnityEngine;
using System;

[RequireComponent(typeof(PlayerCore))]
public class AIInputHandler : MonoBehaviour
{
    private PlayerCore m_core;
    private PlayerMovementController m_movement;
    private PlayerCombatController m_combat;
    private PlayerHealthManager m_health;

    [Header("AI Settings")]
    [Tooltip("How often (in seconds) the AI decides on the next action.")]
    [SerializeField] private float m_decisionInterval = 0.15f; // Biraz daha s�k karar als�n

    [Tooltip("Maximum distance at which the AI can see/detect targets.")]
    [SerializeField] private float m_detectionRange = 500f; // Biraz art�rd�k

    [Tooltip("Desired range for attacking the player (melee).")]
    [SerializeField] private float m_idealAttackRange = 1.5f;

    [Tooltip("AI's reaction delay for blocking or reacting to attacks.")]
    [SerializeField] private float m_blockReactionTime = 0.15f; // Biraz h�zland�rd�k

    [Tooltip("Chance (0-1) that the AI might block if the target is attacking.")]
    [Range(0f, 1f)]
    [SerializeField] private float m_blockChance = 0.5f;

    [Tooltip("Chance (0-1) that the AI uses a roll instead of a jump to dodge.")]
    [Range(0f, 1f)]
    [SerializeField] private float m_rollInsteadOfJumpChance = 0.3f;

    [Header("General Behavior Tweaks")]
    [Tooltip("Random factor for deciding if the AI tries to do combos, chase, etc.")]
    [Range(0f, 1f)]
    [SerializeField] private float m_randomAggression = 0.6f;  // Biraz art�rd�k

    // Internal states
    private AIState m_currentState = AIState.Idle;
    private float m_decisionTimer = 0f;
    private bool m_isBlocking = false;

    // Target tracking
    private Transform m_target;
    private PlayerCore m_targetCore;  // If your target is also a PlayerCore

    // A small cooldown for repeated actions
    private float m_actionCooldownTimer = 0f;
    private const float ACTION_COOLDOWN = 0.15f; // Daha s�k sald�rabilsin

    private void Awake()
    {
        m_core = GetComponent<PlayerCore>();
        m_movement = m_core.MovementController;
        m_combat = m_core.CombatController;
        m_health = m_core.HealthManager;
    }

    private void Start()
    {
        // Identify a valid target�e.g. another PlayerCore with different PlayerType
        FindTargetPlayer();

        // Set initial state, if you want
        TransitionTo(AIState.Idle);
    }

    private void Update()
    {
        // Only run AI if the game is active
        if (GameManager.Instance.CurrentState == GameStates.GameOn ||
            GameManager.Instance.CurrentState == GameStates.FinalFight)
        {
            // Update blocking state if any
            UpdateBlockLogic();

            // Update timers
            m_decisionTimer += Time.deltaTime;
            m_actionCooldownTimer -= Time.deltaTime;

            if (m_decisionTimer >= m_decisionInterval)
            {
                m_decisionTimer = 0f;
                DecideNextAction();
            }
        }
        else
        {
            // In other states (Respawn, GameOver, etc.), do nothing
        }
    }

    private void DecideNextAction()
    {
        // If no target found, just Idle
        if (m_target == null)
        {
            TransitionTo(AIState.Idle);
            return;
        }

        switch (m_currentState)
        {
            case AIState.Idle:
                ThinkIdle();
                break;
            case AIState.Chase:
                ThinkChase();
                break;
            case AIState.Attack:
                ThinkAttack();
                break;
            case AIState.Block:
                ThinkBlock();
                break;
            case AIState.Retreat:
                ThinkRetreat();
                break;
        }
    }

    //==================================================
    // State Machine: Internal Logic
    //==================================================

    private void ThinkIdle()
    {
        float distance = DistanceToTarget();

        // Hedef, detectionRange i�inde mi?
        if (distance < m_detectionRange)
        {
            // AI, bo� durmaktansa hedefe do�ru yakla�s�n
            // Daha agresif olsun diye random de�erini biraz y�kselttik
            if (UnityEngine.Random.value < 0.8f)
            {
                TransitionTo(AIState.Chase);
            }
        }
        else
        {
            // Hedef �ok uzakta de�ilse bile yine kovalama �ans�n� biraz y�kseltebiliriz
            // �rne�in 0.3 gibi bir ihtimal daha ekleyebilirsiniz.
        }
    }

    private void ThinkChase()
    {
        float distance = DistanceToTarget();

        // Hedef fazla uzakla��rsa Idle'a d�n
        // (Ya da isterseniz burada da "hedef uzakta ama yine de kovalayal�m" yapabilirsiniz.)
        if (distance > m_detectionRange + 2f)
        {
            TransitionTo(AIState.Idle);
            return;
        }

        // E�er ideal sald�r� aral���ndaysak sald�r
        if (distance <= m_idealAttackRange + 0.1f)
        {
            TransitionTo(AIState.Attack);
            return;
        }

        // Hedefe do�ru y�nelim
        float dir = (m_target.position.x > transform.position.x) ? 1f : -1f;
        m_movement.HandleHorizontalMovement(dir);

        // Belli bir ihtimalle z�pla veya yuvarlan
        if (ShouldDodgeOrJump())
        {
            AttemptJumpOrRoll();
        }
    }

    private void ThinkAttack()
    {
        float distance = DistanceToTarget();

        // Mesafe a��ld�ysa tekrar kovalamaya d�n
        if (distance > m_idealAttackRange + 0.5f)
        {
            TransitionTo(AIState.Chase);
            return;
        }

        // Sald�r� yap (cooldown bitti mi?)
        if (m_actionCooldownTimer <= 0f)
        {
            m_combat.HandleAttack();
            m_actionCooldownTimer = ACTION_COOLDOWN;

            // Sald�r� sonras� k�sa s�re sonra yeni sald�r� yapma �ans�
            // m_randomAggression de�erini art�rd�k, combo yapma olas�l��� arts�n
            float r = UnityEngine.Random.value;
            if (r < m_randomAggression)
            {
                // Ayn� Attack state'te kal�p bir sonraki kararda yine sald�rabilir
                // (Bu sayede AI pe� pe�e sald�r�lar yapabilir)
            }
            else
            {
                // Blok veya geri �ekilme veya tekrar Chase durumu
                float r2 = UnityEngine.Random.value;
                if (r2 < 0.3f)
                {
                    TransitionTo(AIState.Block);
                }
                else if (r2 < 0.5f)
                {
                    TransitionTo(AIState.Retreat);
                }
                else
                {
                    TransitionTo(AIState.Chase);
                }
            }
        }
    }

    private void ThinkBlock()
    {
        // Blok durumunu y�netiyoruz
        // Bu i�i bir coroutine ile k�sa s�reli�ine yap�p tekrar durumu de�i�tirece�iz
        StartCoroutine(BlockBriefly());
    }

    private IEnumerator BlockBriefly()
    {
        // Blokta de�ilsek gir
        if (!m_isBlocking)
        {
            m_isBlocking = true;
            m_combat.HandleBlock(true);  // Raise shield
        }

        // 0.4-0.8 saniye blok tut
        float blockTime = UnityEngine.Random.Range(0.4f, 0.8f);
        yield return new WaitForSeconds(blockTime);

        // Bloktan ��k
        m_isBlocking = false;
        m_combat.HandleBlock(false); // Lower shield

        // Bloktan sonra rastgele bir state'e ge�ebilirsiniz.
        // �rne�in tekrar sald�r� ya da chase
        if (DistanceToTarget() <= m_idealAttackRange)
        {
            TransitionTo(AIState.Attack);
        }
        else
        {
            TransitionTo(AIState.Chase);
        }
    }

    private void ThinkRetreat()
    {
        float distance = DistanceToTarget();
        float dir = (m_target.position.x > transform.position.x) ? -1f : 1f;
        m_movement.HandleHorizontalMovement(dir);

        // Geri �ekilirken de arada z�pla/roll yapabilir
        if (ShouldDodgeOrJump())
        {
            AttemptJumpOrRoll();
        }

        // E�er belli bir mesafe a�t�ysak veya random bir ihtimal olu�tuysa Idle'a ya da Chase'e ge�elim
        if (distance > m_detectionRange * 0.6f || UnityEngine.Random.value > 0.7f)
        {
            TransitionTo(AIState.Idle);
        }
    }

    //==================================================
    // Helpers & Utility
    //==================================================

    private void TransitionTo(AIState newState)
    {
        m_currentState = newState;
        // Debug.Log("AI " + m_core.PlayerType + " => " + m_currentState);
    }

    private float DistanceToTarget()
    {
        if (!m_target) return float.MaxValue;
        return Vector2.Distance(transform.position, m_target.position);
    }

    private bool ShouldDodgeOrJump()
    {
        // �rnek: %25 ihtimalle z�pla/roll denesin
        return (UnityEngine.Random.value < 0.25f);
    }

    private void AttemptJumpOrRoll()
    {
        if (UnityEngine.Random.value < m_rollInsteadOfJumpChance)
        {
            // Perform a roll
            m_movement.HandleRoll();
        }
        else
        {
            // Perform a jump
            m_movement.HandleJump();
        }
    }

    /// <summary>
    /// Yak�n d�v�� sald�r�s� alg�lama vb. i�in block mant���
    /// </summary>
    private void UpdateBlockLogic()
    {
        // Halihaz�rda blok state'inde ya da blok yap�yorsak ��k
        if (m_currentState == AIState.Block || m_isBlocking) return;

        // Hedef �ok uzaktaysa blok yapma ihtimalini k�sabilirsiniz. 
        // �rne�in:
        float distance = DistanceToTarget();
        if (distance > m_idealAttackRange * 1.5f) return;

        bool targetIsAttacking = false;
        if (m_targetCore && m_targetCore.CombatController)
        {
            // Burada ger�ekten hedefin sald�r� animasyonuna bakabilirsiniz (�r. targetCore.CombatController.IsAttacking)
            // Biz basit bir random senaryo yap�yoruz
            targetIsAttacking = (UnityEngine.Random.value < 0.08f);
        }

        if (targetIsAttacking && UnityEngine.Random.value < m_blockChance)
        {
            StartCoroutine(BlockWithDelay(m_blockReactionTime));
        }
    }

    private IEnumerator BlockWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!m_isBlocking && m_currentState != AIState.Block)
        {
            TransitionTo(AIState.Block);
        }
    }

    /// <summary>
    /// Bulabildi�imiz ilk farkl� PlayerType'a sahip PlayerCore'u hedef al.
    /// </summary>
    private void FindTargetPlayer()
    {
        var allPlayers = FindObjectsOfType<PlayerCore>();
        foreach (var pc in allPlayers)
        {
            if (pc != m_core && pc.PlayerType != m_core.PlayerType)
            {
                m_target = pc.transform;
                m_targetCore = pc;
                break;
            }
        }
    }
}

//==================================================
// AI States
//==================================================
public enum AIState
{
    Idle,
    Chase,
    Attack,
    Block,
    Retreat
}
