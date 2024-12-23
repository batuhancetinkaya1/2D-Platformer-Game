using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Transform m_respawnPoint;

    private void OnEnable()
    {
        GameManager.OnGameStateChange += OnGameStateChange;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
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
                if (m_respawnPoint != null)
                {
                    player.transform.position = m_respawnPoint.position;
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
