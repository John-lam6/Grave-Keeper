using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu UI")]
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button mainMenuButton;
    public Button quitButton;
    public TMP_Text pauseTitle;
    
    public bool isPaused = false;

    public PlayerShoot playershoot;
    public PlayerMovements playermovement;
    public ZombieManager zombieManager;
    
    public void Init()
    {
        // setup button listeners
        if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
        
        // hides pause menu at start
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }
    
    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
    
    public void Pause()
    {
        
        
        // find GameManager and call ShowMainMenu
        GameManager gameManager = FindAnyObjectByType<GameManager>();
    
        zombieManager.setCanSummon(false);
        ZombieController[] controllers = FindObjectsOfType<ZombieController>();
        ZombieAttack[] attacks = FindObjectsOfType<ZombieAttack>();
        
        foreach (var z in controllers)
            z.setCanMove(false);

        foreach (var z in attacks)
            z.setCanMove(false);
        
        if (gameManager != null)
        {
            // pauses timer
            Timer timer = gameManager.getTimer();
            timer.timerPause();
        }
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
        
        Time.timeScale = 0f; 
        isPaused = true;
    }
    
    public void Resume()
    {
        zombieManager.setCanSummon(true);
        ZombieController[] controllers = FindObjectsOfType<ZombieController>();
        ZombieAttack[] attacks = FindObjectsOfType<ZombieAttack>();
        
        foreach (var z in controllers)
            z.setCanMove(true);

        foreach (var z in attacks)
            z.setCanMove(true);
        
        if (pauseMenuPanel != null)
        {
            // find GameManager and call ShowMainMenu
            GameManager gameManager = FindAnyObjectByType<GameManager>();
            if (gameManager != null)
            {
                // resumes timer
                Timer timer = gameManager.getTimer();
                timer.timerResume();
            }
            pauseMenuPanel.SetActive(false);
        }
        
        Time.timeScale = 1f;
        isPaused = false;
    }
    
    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        
        // find GameManager and call ShowMainMenu
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null)
        {
            // resets the timer
            Timer timer = gameManager.getTimer();
            timer.timerReset();
            gameManager.ShowMainMenu();
        }
        
        // hide pause menu
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }
    
    private void QuitGame()
    {
        Time.timeScale = 1f;
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    void OnDestroy()
    {
        Time.timeScale = 1f;
        
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
        }
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
        }
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
        }
    }
}