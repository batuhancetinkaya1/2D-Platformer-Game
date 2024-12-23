using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameStates m_currentState;
    public GameStates CurrentState => m_currentState;

    public delegate void GameStateChangeHandler(GameStates newState);
    public static event GameStateChangeHandler OnGameStateChange;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // �kinci bir GameManager varsa yok et (Singleton).

        // Sahne ge�i�lerinde yok olmas�n istiyorsan�z:
        // DontDestroyOnLoad(this.gameObject);
    }

    public void ChangeState(GameStates newState)
    {
        if (m_currentState == newState)
            return;

        m_currentState = newState;
        Debug.Log($"Game State Changed: {newState}");
        OnGameStateChange?.Invoke(newState);
    }
}
