using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gamePanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public GameObject creditsPanel;
    public GameObject controlsPanel;

    [Header("Game UI Elements")]
    public Text starText;
    public Image keyImage; // Örn. bir key sprite
    // vs. ek UI

    private int m_totalstars;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Sahne deðiþse bile yok olmasýn istiyorsanýz:
        // DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        ShowMainMenu();
    }

    #region Panel Control
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        gamePanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        creditsPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        gamePanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        creditsPanel.SetActive(false);
        controlsPanel.SetActive(false);

        // Gerekirse scene yüklemek isterseniz:
        // SceneManager.LoadScene("MainScene");
    }

    public void ShowPauseMenu()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // Oyunu durdur
    }

    public void HidePauseMenu()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void ShowCredits()
    {
        creditsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void HideCredits()
    {
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void ShowControls()
    {
        controlsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void HideControls()
    {
        controlsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    #endregion

    #region star & Key
    public void AddStars(int amount)
    {
        m_totalstars += amount;
        if (starText != null)
            starText.text = "Stars: " + m_totalstars.ToString();
    }

    public void ResetStars()
    {
        m_totalstars = 0;
        if (starText != null)
            starText.text = "Stars: 0";
    }

    // Key toplama vs.
    public void ShowKey()
    {
        if (keyImage) keyImage.enabled = true;
    }
    public void HideKey()
    {
        if (keyImage) keyImage.enabled = false;
    }
    #endregion

    #region Buttons
    public void OnPlayButton()
    {
        // StartGame butonu
        StartGame();
    }

    public void OnCreditsButton()
    {
        ShowCredits();
    }

    public void OnReturnMenuButton()
    {
        ShowMainMenu();
    }

    public void OnPauseButton()
    {
        ShowPauseMenu();
    }

    public void OnContinueButton()
    {
        HidePauseMenu();
    }

    public void OnGameOverReturnMenu()
    {
        // GameOver panelindeki ReturnMenu
        ShowMainMenu();
        // Time.timeScale = 1f;
        // Gerekirse sahneyi sýfýrla:
        // SceneManager.LoadScene("MainMenu");
    }

    public void OnShowControlsButton()
    {
        ShowControls();
    }

    public void OnHideControlsButton()
    {
        HideControls();
    }
    #endregion
}
