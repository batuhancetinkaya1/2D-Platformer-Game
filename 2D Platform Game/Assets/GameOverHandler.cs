using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    private void OnEnable()
    {
        // GameState de�i�ti�inde tetiklenecek
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
            // Oyun bitti�inde yap�lacak i�lemler
            Debug.Log("Game Over! Show Game Over UI or something else...");
        }
    }
}
