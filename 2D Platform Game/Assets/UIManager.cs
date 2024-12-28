using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Menu Panels")]
    public GameObject m_mainMenuPanel;
    public GameObject m_creditsPanel;
    public GameObject m_controlsPanel;
    [Header("Game Panels")]
    public GameObject m_pauseMenuPanel;
    public GameObject m_gamePanel;
    [Header("GameOver Panel")]
    public GameObject m_gameOverPanel;

    [Header("Game UI Elements")]
    public TMP_Text m_starText;
    public RawImage m_keyImage;

    private int m_totalStars;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        ResetStars();
    }

    #region Panel Control
    public void LoadGameScene()
    {
        HideAllPanels();
        GameManager.Instance.ChangeState(GameStates.GameOn);
        GameManager.Instance.LoadScene(SceneNames.GameScene);
        GameManager.Instance.ControlTime(false);
    }

    public void LoadMenuScene()
    {
        HideAllPanels();
        GameManager.Instance.ChangeState(GameStates.Menu);
        GameManager.Instance.LoadScene(SceneNames.MenuScene);
        GameManager.Instance.ControlTime(true);
    }

    public void LoadFightScene()
    {
        HideAllPanels();
        GameManager.Instance.ChangeState(GameStates.FinalFight);
        GameManager.Instance.LoadScene(SceneNames.MenuScene);
        GameManager.Instance.ControlTime(true);
    }

    public void ShowPauseMenu()
    {
        m_gamePanel.SetActive(false);
        m_pauseMenuPanel.SetActive(true);
        GameManager.Instance.ControlTime(true);
    }

    public void HidePauseMenu()
    {
        m_gamePanel.SetActive(true);
        m_pauseMenuPanel.SetActive(false);
        GameManager.Instance.ControlTime(false);
    }

    public void ShowCredits()
    {
        HideAllPanels();
        m_creditsPanel.SetActive(true);
    }

    public void HideCredits()
    {
        m_creditsPanel.SetActive(false);
        ShowMainMenu();
    }

    public void ShowControls()
    {
        HideAllPanels();
        m_controlsPanel.SetActive(true);
    }

    public void HideControls()
    {
        m_controlsPanel.SetActive(false);
        ShowMainMenu();
    }

    private void HideAllPanels()
    {
        if (m_mainMenuPanel != null) m_mainMenuPanel.SetActive(false);
        if (m_creditsPanel != null) m_creditsPanel.SetActive(false);
        if (m_controlsPanel != null) m_controlsPanel.SetActive(false);
        if (m_pauseMenuPanel != null) m_pauseMenuPanel.SetActive(false);
        if (m_gameOverPanel != null) m_gameOverPanel.SetActive(false);
        if (m_gamePanel != null) m_gamePanel.SetActive(false);
    }

    private void ShowMainMenu()
    {
        if (m_mainMenuPanel != null)
            m_mainMenuPanel.SetActive(true);
    }
    #endregion

    #region Star & Key
    public void AddStars(int amount)
    {
        m_totalStars += amount;
        UpdateStarText();
    }

    public void ResetStars()
    {
        m_totalStars = 0;
        UpdateStarText();
    }

    private void UpdateStarText()
    {
        if (m_starText != null)
            m_starText.text = $"x {m_totalStars}";
    }

    public void ShowKey()
    {
        if (m_keyImage != null) m_keyImage.enabled = true;
    }

    public void HideKey()
    {
        if (m_keyImage != null) m_keyImage.enabled = false;
    }
    #endregion

    #region Buttons MenuScene
    public void OnPlayButton() => LoadGameScene();
    public void OnCreditsButton() => ShowCredits();
    public void OnControlsButton() => ShowControls();
    public void OnHideCreditsButton() => HideCredits();
    public void OnHideControlsButton() => HideControls();
    #endregion

    #region Buttons GameScene
    public void OnReturnMenuButton() => LoadMenuScene();
    public void OnPauseButton() => ShowPauseMenu();
    public void OnContinueButton() => HidePauseMenu();
    public void OnGameOverReturnMenu() => LoadMenuScene();
    #endregion
}
