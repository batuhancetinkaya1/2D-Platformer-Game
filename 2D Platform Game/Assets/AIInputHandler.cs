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
    [SerializeField] private float m_decisionInterval = 0.25f;

    [Tooltip("Maximum distance at which the AI can see/detect targets.")]
    [SerializeField] private float m_detectionRange = 8f;

    [Tooltip("Desired range for attacking the player (melee).")]
    [SerializeField] private float m_idealAttackRange = 1.5f;

    [Tooltip("AI's reaction delay for blocking or reacting to attacks.")]
    [SerializeField] private float m_blockReactionTime = 0.2f;

    [Tooltip("Chance (0-1) that the AI might block if the target is attacking.")]
    [Range(0f, 1f)]
    [SerializeField] private float m_blockChance = 0.5f;

    [Tooltip("Chance (0-1) that the AI uses a roll instead of a jump to dodge.")]
    [Range(0f, 1f)]
    [SerializeField] private float m_rollInsteadOfJumpChance = 0.3f;

    [Header("General Behavior Tweaks")]
    [Tooltip("Random factor for deciding if the AI tries to do combos, chase, etc.")]
    [Range(0f, 1f)]
    [SerializeField] private float m_randomAggression = 0.5f;

    // Internal states
    private AIState m_currentState = AIState.Idle;
    private float m_decisionTimer = 0f;
    private bool m_isBlocking = false;

    // Target tracking
    private Transform m_target;
    private PlayerCore m_targetCore;  // If your target is also a PlayerCore

    // A small cooldown for repeated actions
    private float m_actionCooldownTimer = 0f;
    private const float ACTION_COOLDOWN = 0.2f;

    private void Awake()
    {
        m_core = GetComponent<PlayerCore>();
        m_movement = m_core.MovementController;
        m_combat = m_core.CombatController;
        m_health = m_core.HealthManager;
    }

    private void Start()
    {
        // Identify a valid target—e.g. another PlayerCore with different PlayerType
        FindTargetPlayer();

        // Set initial state, if you want
        TransitionTo(AIState.Idle);
    }

    private void Update()
    {
        // Only run AI if the game is active (avoid messing with states during pause/cutscene)
        if (GameManager.Instance.CurrentState == GameStates.GameOn ||
            GameManager.Instance.CurrentState == GameStates.FinalFight)
        {
            // Update blocking state if any
            UpdateBlockLogic();

            // Update decision timer
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
            // In other states (Respawn, GameOver, etc.), do nothing or reset
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

        // Evaluate states
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
        // Possibly chase if the target is within detection range
        float distance = DistanceToTarget();

        if (distance < m_detectionRange)
        {
            // 50% chance to chase right away; otherwise wait
            if (UnityEngine.Random.value < 0.5f)
                TransitionTo(AIState.Chase);
        }
    }

    private void ThinkChase()
    {
        // Move horizontally to get close to the target
        float distance = DistanceToTarget();

        if (distance > m_detectionRange)
        {
            // Lost the target => go idle
            TransitionTo(AIState.Idle);
            return;
        }

        // If we are close enough => Attack
        if (distance <= m_idealAttackRange)
        {
            TransitionTo(AIState.Attack);
            return;
        }

        // Else chase horizontally
        float dir = (m_target.position.x > transform.position.x) ? 1f : -1f;
        m_movement.HandleHorizontalMovement(dir);

        // Possibly jump or roll if there's an obstacle or random chance
        if (ShouldDodgeOrJump())
        {
            AttemptJumpOrRoll();
        }
    }

    private void ThinkAttack()
    {
        // If we are far from target, go back to chase
        float distance = DistanceToTarget();
        if (distance > m_idealAttackRange + 0.5f)
        {
            TransitionTo(AIState.Chase);
            return;
        }

        // Attack if not on cooldown
        if (m_actionCooldownTimer <= 0f)
        {
            m_combat.HandleAttack();
            m_actionCooldownTimer = ACTION_COOLDOWN;

            // Chance to remain in Attack state for combos
            if (UnityEngine.Random.value < m_randomAggression)
            {
                // stay in Attack, do another slash next time
            }
            else
            {
                // maybe block or move away
                float r = UnityEngine.Random.value;
                if (r < 0.2f) TransitionTo(AIState.Block);
                else if (r < 0.4f) TransitionTo(AIState.Retreat);
                else TransitionTo(AIState.Chase);
            }
        }
    }

    private void ThinkBlock()
    {
        // Typically we only block for a short time or if we see the target attacking
        // For simplicity, we do a short block and return to chase or idle
        StartCoroutine(BlockBriefly());
    }

    private IEnumerator BlockBriefly()
    {
        // Enter block if we’re not already
        if (!m_isBlocking)
        {
            m_isBlocking = true;
            m_combat.HandleBlock(true);  // Raise shield
        }

        // Block for random 0.5-1.0 seconds
        float blockTime = UnityEngine.Random.Range(0.5f, 1.0f);
        yield return new WaitForSeconds(blockTime);

        m_isBlocking = false;
        m_combat.HandleBlock(false); // Lower shield

        // After blocking, maybe chase again
        TransitionTo(AIState.Chase);
    }

    private void ThinkRetreat()
    {
        // Move away from the target for a short duration
        float distance = DistanceToTarget();
        float dir = (m_target.position.x > transform.position.x) ? -1f : 1f;
        m_movement.HandleHorizontalMovement(dir);

        // Possibly jump or roll away
        if (ShouldDodgeOrJump())
        {
            AttemptJumpOrRoll();
        }

        // If we have retreated enough, or random chance, move on
        if (distance > m_detectionRange * 0.5f || UnityEngine.Random.value > 0.8f)
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
        // Debugging / logging
        // Debug.Log("AI " + m_core.PlayerType + " => " + m_currentState);
    }

    private float DistanceToTarget()
    {
        if (!m_target) return float.MaxValue;
        return Vector2.Distance(transform.position, m_target.position);
    }

    private bool ShouldDodgeOrJump()
    {
        // Example logic: 20% chance to jump while chasing/retreating
        return (UnityEngine.Random.value < 0.2f);
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
    /// Checks if we should block, and sets block state if so.
    /// You might expand this with sensors for the player's Attack animation.
    /// </summary>
    private void UpdateBlockLogic()
    {
        // If we are already blocking or in a block state, skip
        if (m_currentState == AIState.Block || m_isBlocking) return;

        // If target is in an attacking state (not shown here, but you could check e.g. their anim),
        // there's a chance we block. We do a small reaction delay with a coroutine.
        // (This is a placeholder, you'd need to detect if the player is attacking)
        bool targetIsAttacking = false;
        if (m_targetCore && m_targetCore.CombatController)
        {
            // Hypothetical check: if they're mid-attack
            // targetIsAttacking = m_targetCore.CombatController.IsAttacking;
            // For now, let's just randomly say they might be attacking
            targetIsAttacking = (UnityEngine.Random.value < 0.05f);
        }

        // Decide to block
        if (targetIsAttacking && UnityEngine.Random.value < m_blockChance)
        {
            StartCoroutine(BlockWithDelay(m_blockReactionTime));
        }
    }

    private IEnumerator BlockWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Double-check we’re still in a valid situation
        if (!m_isBlocking && m_currentState != AIState.Block)
        {
            TransitionTo(AIState.Block);
        }
    }

    /// <summary>
    /// Finds another player in the scene with a different PlayerType.
    /// Adjust to your needs (PVP, multiple AI, etc.).
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

