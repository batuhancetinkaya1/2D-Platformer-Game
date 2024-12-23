using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    private void OnEnable()
    {
        // GameState deðiþtiðinde tetiklenecek
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
            // Oyun bittiðinde yapýlacak iþlemler
            Debug.Log("Game Over! Show Game Over UI or something else...");
        }
    }
}
