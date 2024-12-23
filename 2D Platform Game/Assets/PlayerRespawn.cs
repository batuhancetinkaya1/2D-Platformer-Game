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
            // Tüm PlayerCore nesnelerini bul
            var players = FindObjectsOfType<PlayerCore>();
            foreach (var player in players)
            {
                // Player'ý respawn noktasýna götür
                if (m_respawnPoint != null)
                {
                    player.transform.position = m_respawnPoint.position;
                }

                // Caný yenile
                player.HealthManager.Heal(player.HealthManager.MaxHealth);
                player.AnimControl.SetTriggerRespawn();
            }

            // Ardýndan GameOn'a dön
            GameManager.Instance.ChangeState(GameStates.GameOn);
        }
    }
}
