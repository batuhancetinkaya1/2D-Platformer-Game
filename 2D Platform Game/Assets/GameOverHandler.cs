using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    private void OnEnable()
    {
        // GameState değiştiğinde tetiklenecek
        GameManager.OnGameStateChange += HandleGameStateChange;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChange -= HandleGameStateChange;
    }

    private void HandleGameStateChange(GameStates newState)
    {
        if (newState == GameStates.GameOver)
        {
            // Oyun bittiğinde yapılacak işlemler
            Debug.Log("Game Over! Show Game Over UI or something else...");
        }
    }
}
