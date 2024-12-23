using UnityEngine;

[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerCombatController))]
[RequireComponent(typeof(PlayerHealthManager))]
[RequireComponent(typeof(PlayerAnimControl))]
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerDetectionControl))]
public class PlayerCore : MonoBehaviour
{
    [Header("Player Type")]
    [SerializeField] private PlayerType m_playerType;
    public PlayerType PlayerType => m_playerType;

    // Alt bile�enler
    public PlayerMovementController MovementController { get; private set; }
    public PlayerCombatController CombatController { get; private set; }
    public PlayerHealthManager HealthManager { get; private set; }
    public PlayerAnimControl AnimControl { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public PlayerDetectionControl DetectionControl { get; private set; }

    private GameManager m_gameManager;

    private void Awake()
    {
        MovementController = GetComponent<PlayerMovementController>();
        CombatController = GetComponent<PlayerCombatController>();
        HealthManager = GetComponent<PlayerHealthManager>();
        AnimControl = GetComponent<PlayerAnimControl>();
        InputHandler = GetComponent<PlayerInputHandler>();
        DetectionControl = GetComponent<PlayerDetectionControl>();

        //m_gameManager = GameManager.Instance;

        // Initialize
        MovementController.Initialize(this);
        CombatController.Initialize(this);
        HealthManager.Initialize(this);
    }

    private void Start()
    {
        m_gameManager = GameManager.Instance;
    }

    public void OnPlayerDeath()
    {
        // GameManager var m� kontrol
        if (m_gameManager == null)
        {
            Debug.LogWarning("GameManager yok, OnPlayerDeath() => Respawn veya GameOver �a�r�lamad�.");
            return;
        }

        // FinalFight durumundaysa => GameOver
        if (m_gameManager.CurrentState == GameStates.FinalFight)
            m_gameManager.ChangeState(GameStates.GameOver);
        else
            m_gameManager.ChangeState(GameStates.Respawn);
    }
}
