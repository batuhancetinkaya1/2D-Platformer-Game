using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    [SerializeField] private Transform[] m_spawnPoints; // Sahnedeki spawn noktalarý
    [SerializeField] private GameObject m_playerPrefab;
    [SerializeField] private GameObject m_player2Prefab;
    [SerializeField] private GameObject m_botPrefab;
    [SerializeField] private GameObject m_bot2Prefab;

    private void Start()
    {
        string mode = PlayerPrefs.GetString("ArenaMode", "PVP");
        SetupArena(mode);
    }

    private void SetupArena(string mode)
    {
        switch (mode)
        {
            case "PVP":
                // Örneðin 2 player instantiate et
                Instantiate(m_playerPrefab, m_spawnPoints[0].position, Quaternion.identity);
                Instantiate(m_player2Prefab, m_spawnPoints[1].position, Quaternion.identity);
                break;

            case "PVB":
                // Bir player + bir bot
                Instantiate(m_playerPrefab, m_spawnPoints[0].position, Quaternion.identity);
                Instantiate(m_botPrefab, m_spawnPoints[1].position, Quaternion.identity);
                break;

            case "AIvsAI":
                // Ýki bot
                Instantiate(m_bot2Prefab, m_spawnPoints[0].position, Quaternion.identity);
                Instantiate(m_botPrefab, m_spawnPoints[1].position, Quaternion.identity);
                break;

            default:
                Instantiate(m_playerPrefab, m_spawnPoints[0].position, Quaternion.identity);
                Instantiate(m_botPrefab, m_spawnPoints[1].position, Quaternion.identity);
                break;
        }
    }
}
