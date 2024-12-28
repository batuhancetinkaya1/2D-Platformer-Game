using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Transform m_respawnPoint1;
    [SerializeField] private Transform m_respawnPoint2;
    [SerializeField] private GameObject m_spawnPoint2Visual;

    private Transform m_currentRespawnPoint;
    private bool m_hasKey = false;

    private void Awake()
    {
        m_currentRespawnPoint = m_respawnPoint1;
        if (m_spawnPoint2Visual != null)
            m_spawnPoint2Visual.SetActive(false);
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChange += OnGameStateChange;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    public void SetSpawnPointToTwo()
    {
        m_currentRespawnPoint = m_respawnPoint2;
        m_spawnPoint2Visual.SetActive(true);
        m_hasKey = true;
    }

    public bool HasKey()
    {
        return m_hasKey;
    }

    private void OnGameStateChange(GameStates newState)
    {
        if (newState == GameStates.Respawn)
        {
            // T�m PlayerCore nesnelerini bul
            var players = FindObjectsOfType<PlayerCore>();
            foreach (var player in players)
            {
                // Player'� respawn noktas�na g�t�r
                if (m_currentRespawnPoint != null)
                {
                    player.transform.position = m_currentRespawnPoint.position;
                }

                // Can� yenile
                player.HealthManager.Heal(player.HealthManager.MaxHealth);
                player.AnimControl.SetTriggerRespawn();
            }

            // Ard�ndan GameOn'a d�n
            GameManager.Instance.ChangeState(GameStates.GameOn);
        }
    }
}