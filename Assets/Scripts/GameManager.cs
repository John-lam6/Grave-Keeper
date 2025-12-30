using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu,
        Leaderboard,
        WeaponSelection,
        Game,
        GameEnd
    }

    public Transform playerTransform;
    public CameraControl m_CameraControl; // Reference to the CameraControl script for control during different phases.
    private GameState m_CurrentState;
    
    [Header("Tanks Prefabs")]
    public GameObject gunPrefab1;            // The Prefab used by the gun in Slot 1 of the Menu
    public GameObject gunPrefab2;            // The Prefab used by the gun in Slot 2 of the Menu
    public GameObject gunPrefab3;            // The Prefab used by the gun in Slot 3 of the Menu
    
    [Header("Screens")]
    public GameObject m_MainMenuScreen;
    public GameObject m_WeaponSelectionScreen;
    public GameObject m_GameScreen;
    public GameObject m_GameOverScreen;
    public GameObject m_LeaderboardScreen;

    [Header("Main Menu UI")]
    public Button m_StartButton;
    public Button m_LeaderboardButton;
    public Button m_QuitButton;
    
    [Header("Game UI")]
    public TMP_Text timer_display;
    public TMP_Text score_display;
    public OnScreenButton m_PauseMenuButton;
    
    [Header("Game Over UI")]
    public TMP_Text finalScore_display;
    public Button m_RetryButton;
    public Button m_MenuButton;
    
    [Header("Leaderboard UI")]
    public TMP_Text leaderboard_display;
    public Button m_BackButton;
    
    [SerializeField] private Timer timer;
    
    private int score = 0;
    
    private PauseMenu m_PauseMenu;
    private InputAction m_PauseAction;
    private List<Entry> leaderboard = new List<Entry>();

    private bool[] gunSelected = { false, false, false }; // assault rifle, shotgun, sniper
    private GameUIHandler m_GameUIHandler;

    private PlayerShoot playershoot;
    private PlayerMovements playerMovement;

    public Canvas game_canvas;
    public ZombieManager zombiemanager;
    
    private LanguageManager languageManager;
    public class Entry {
        public string gun;
        public int score;

        public Entry(string gun, int score) {
            this.gun = gun;
            this.score = score;
        }
    }
    private void Start() {
        languageManager = FindObjectOfType<LanguageManager>();
        playershoot = playerTransform.GetComponent<PlayerShoot>();
        playerMovement = playerTransform.GetComponent<PlayerMovements>();
        
        m_CurrentState = GameState.MainMenu;
        m_GameUIHandler = FindAnyObjectByType<GameUIHandler>();
        
        // add functionality to buttons
        if (m_StartButton != null)
            m_StartButton.onClick.AddListener(ShowWeaponSelection);
        if (m_LeaderboardButton != null)
            m_LeaderboardButton.onClick.AddListener(ShowLeaderboard);
        if (m_QuitButton != null)
            m_QuitButton.onClick.AddListener(QuitGame);
        if (m_RetryButton != null)
            m_RetryButton.onClick.AddListener(ShowWeaponSelection);
        if (m_MenuButton != null)
            m_MenuButton.onClick.AddListener(ShowMainMenu);
        if (m_BackButton != null)
            m_BackButton.onClick.AddListener(ShowMainMenu);
        
        m_PauseMenu = FindAnyObjectByType<PauseMenu>(FindObjectsInactive.Include);
        
        if (m_PauseMenu != null) {
            m_PauseMenu.Init();

            if (m_PauseMenuButton != null) {
                var rectTransform = m_PauseMenuButton.GetComponent<RectTransform>();
                rectTransform.SetAsLastSibling();
            }
        }
        m_CurrentState = GameState.MainMenu;
        
        LoadLeaderboard();
        ShowMainMenu();
    }
    
    public void ShowMainMenu() {
        zombiemanager.Reset();
        playershoot.setCanMove(false);
        playerMovement.setCanMove(false);
        ChangeGameState(GameState.MainMenu);
        //disableMovement(); // disables movement for zombies and player
        game_canvas.gameObject.SetActive(false);
        if (m_MainMenuScreen != null) m_MainMenuScreen.SetActive(true);
        if (m_WeaponSelectionScreen != null) m_WeaponSelectionScreen.SetActive(false);
        if (m_GameScreen != null) m_GameScreen.SetActive(false);
        if (m_GameOverScreen != null) m_GameOverScreen.SetActive(false);
        if (m_LeaderboardScreen != null) m_LeaderboardScreen.SetActive(false);
        if (m_PauseMenuButton != null) m_PauseMenuButton.gameObject.SetActive(false);
    }

    public void ShowWeaponSelection() {
        zombiemanager.Reset();
        m_GameUIHandler.m_StartButton.gameObject.SetActive(true);
        m_GameUIHandler.m_StartButtonText.text = "Select a gun first";
        playershoot.setCanMove(false);
        playerMovement.setCanMove(false);
        ChangeGameState(GameState.WeaponSelection);
        game_canvas.gameObject.SetActive(true);
        if (m_MainMenuScreen != null) m_MainMenuScreen.SetActive(false);
        if (m_WeaponSelectionScreen != null) m_WeaponSelectionScreen.SetActive(true);
        if (m_GameScreen != null) m_GameScreen.SetActive(false);
        if (m_GameOverScreen != null) m_GameOverScreen.SetActive(false);
        if (m_LeaderboardScreen != null) m_LeaderboardScreen.SetActive(false);
        if (m_PauseMenuButton != null) m_PauseMenuButton.gameObject.SetActive(false);
        
        if (m_GameUIHandler != null) m_GameUIHandler.ShowWeaponSelection();
    }
    
    void ShowLeaderboard()
    {
        zombiemanager.Reset();
        playershoot.setCanMove(false);
        playerMovement.setCanMove(false);
        ChangeGameState(GameState.Leaderboard);
        //disableMovement(); // disables movement for zombies and player
        game_canvas.gameObject.SetActive(false);
        if (m_MainMenuScreen != null) m_MainMenuScreen.SetActive(false);
        if (m_GameScreen != null) m_GameScreen.SetActive(false);
        if (m_WeaponSelectionScreen != null) m_WeaponSelectionScreen.SetActive(false);
        if (m_GameOverScreen != null) m_GameOverScreen.SetActive(false);
        if (m_LeaderboardScreen != null) m_LeaderboardScreen.SetActive(true);
        if (m_PauseMenuButton != null) m_PauseMenuButton.gameObject.SetActive(false);
        
        UpdateLeaderboardDisplay();
    }
    
    void ShowGameOver()
    {
        zombiemanager.Reset();
        playershoot.setCanMove(false);
        playerMovement.setCanMove(false);
        ChangeGameState(GameState.GameEnd);
        //disableMovement(); // disables movement for both computer and player
        game_canvas.gameObject.SetActive(false);
        if (m_MainMenuScreen != null) m_MainMenuScreen.SetActive(false);
        if (m_GameScreen != null) m_GameScreen.SetActive(false);
        if (m_WeaponSelectionScreen != null) m_WeaponSelectionScreen.SetActive(false);
        if (m_GameOverScreen != null) m_GameOverScreen.SetActive(true);
        if (m_LeaderboardScreen != null) m_LeaderboardScreen.SetActive(false);
        if (m_PauseMenuButton != null) m_PauseMenuButton.gameObject.SetActive(false);
        SaveScore();
        
        // displays final score
        if (finalScore_display != null)
            finalScore_display.text = languageManager.currentLanguage.GetValue("Total_Score") + ": " + score;
    }
    
    void OnStartButtonClicked() {
        // reset the variables
        timer.timerReset();;
        score = 0;
        
        // reset the player positions
        if (playerTransform != null)
        {
            playerTransform.position = new Vector3 (-0.6f, 8.2f, -97.4f);
        }
        
        if (m_MainMenuScreen != null) m_MainMenuScreen.SetActive(false);
        if (m_GameScreen != null) m_GameScreen.SetActive(true);
        if (m_GameOverScreen != null) m_GameOverScreen.SetActive(false);
        if (m_LeaderboardScreen != null) m_LeaderboardScreen.SetActive(false);
        
        // enable pause menu
        if (m_PauseMenu != null && m_PauseMenuButton != null) m_PauseMenuButton.gameObject.SetActive(true);
        
        ChangeGameState(GameState.Game);
    }
    
    void ChangeGameState(GameState newState)
    {
        m_CurrentState = newState;

        switch (m_CurrentState)
        {
            case GameState.Game:
                StartCoroutine(GameLoop());
                break;
        }
    }
    
    private IEnumerator GameLoop() {
        //yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine (RoundPlaying());
        //yield return StartCoroutine (RoundEnding());
        
        ShowGameOver();
    }
    
    private IEnumerator RoundStarting () {
        // disable movement until start delay has ended
        //disableMovement(); 
        m_CameraControl.SetStartPositionAndSize ();
        //yield return m_StartWait;
        yield return 1f;
        //enableMovement();
    }
    
    private IEnumerator RoundPlaying () {
        zombiemanager.Reset();
        zombiemanager.setCanSummon(true);
        playershoot.setCanMove(true);
        playerMovement.setCanMove(true);
        PlayerHealth health = playerTransform.GetComponent<PlayerHealth>();
        health.Reset();
        zombiemanager.StartSummon();
        while (health.currentHealth > 0) {
            // Add 1000 points every second
            int currentSecond = Mathf.FloorToInt(timer.getTime());
            int previousSecond = Mathf.FloorToInt(timer.getTime() - Time.deltaTime);
            
            if (currentSecond > previousSecond) {
                score += 1000;
            }

            // displays the timer and score
            if (score_display != null) score_display.text = languageManager.currentLanguage.GetValue("Score") + ": " + score;
            if (timer_display != null) timer_display.text = languageManager.currentLanguage.GetValue("Time") +": " + Mathf.CeilToInt(timer.getTime());
            yield return null;
        }
    }
    
    private IEnumerator RoundEnding ()
    {
        // disableMovement();
        // destroy the it indicator 
        //if (itIndicator != null) Destroy(itIndicator);
        //yield return m_EndWait;
        yield return 1f;
    }
    
    private void SaveScore() {
        string gun = "";
        
        if (gunSelected[0]) gun = "Assault Rifle";
        if (gunSelected[1]) gun = "Shotgun";
        if (gunSelected[2]) gun = "Sniper Rifle";
        
        
        
        leaderboard.Add(new Entry(gun, score));
        leaderboard.Sort((a, b) => b.score.CompareTo(a.score)); // Sort descending
        
        // keep only top 5
        if (leaderboard.Count > 5)
        {
            leaderboard.RemoveRange(5, leaderboard.Count - 5);
        }
        
        SaveLeaderboard();
    }
    
    private void SaveLeaderboard() {
        for (int i = 0; i < leaderboard.Count; i++)
        {
            PlayerPrefs.SetString($"LeaderboardGun{i}", leaderboard[i].gun);
            PlayerPrefs.SetInt($"LeaderboardScore{i}", leaderboard[i].score);
        }
        PlayerPrefs.SetInt("LeaderboardCount", leaderboard.Count);
        PlayerPrefs.Save();
    }

    public void Update() {
        // if ESC is pressed during game phase, pause / resume the game
        if (Input.GetKeyDown(KeyCode.Escape) && m_CurrentState == GameState.Game) {
            if (m_PauseMenu != null) {
                if (m_PauseMenu.isPaused) {
                    m_PauseMenu.Resume();
                }
                else {
                    m_PauseMenu.Pause();
                }
            }
        }
        else if (m_CurrentState != GameState.Game) {
            m_PauseMenu.Resume();
        }
    }

    public void addScore(int score) {
        this.score += score;
    }

    public Timer getTimer() {
        return timer;
    }
    
    private void LoadLeaderboard()
    {
        leaderboard.Clear();
        int count = PlayerPrefs.GetInt("LeaderboardCount", 0);
        
        for (int i = 0; i < count; i++)
        {
            string name = PlayerPrefs.GetString($"LeaderboardGun{i}", "Gun");
            int score = PlayerPrefs.GetInt($"LeaderboardScore{i}", 0);
            leaderboard.Add(new Entry(name, score));
        }
    }

    public void StartGameWithGun(GameObject selectedGunPrefab) {
        timer.timerReset();
        timer.timerResume();
        score = 0;

        if (playerTransform != null) {
            Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            Animator anim = playerTransform.GetComponentInChildren<Animator>();
            bool oldRootMotion = anim.applyRootMotion;
            anim.applyRootMotion = false;
            rb.position = new Vector3 (-0.6f, 8.2f, -97.4f);
            anim.applyRootMotion = oldRootMotion;
            
            //playerTransform.position = new Vector3 (-0.6f, 8.2f, -97.4f);
            string gunName = selectedGunPrefab.name;  // get prefab's name
            
            for (int i = 0; i < playerTransform.childCount; i++) {
                Transform gun = playerTransform.GetChild(i);
                if (gun.name.EndsWith("Gun"))
                    gun.gameObject.SetActive(false);
            }
            
            // Turn on matching gun, turn off others
            for (int i = 0; i < playerTransform.childCount; i++)
            {
                Transform gun = playerTransform.GetChild(i);
                
                if (gun.name.StartsWith(gunName)) {
                    PlayerShoot shootscript = playerTransform.GetComponent<PlayerShoot>();
                    gun.gameObject.SetActive(true);
                    if (gun.name.StartsWith("Rifle")) {
                        gunSelected[0] = true;
                        gunSelected[1] = false;
                        gunSelected[2] = false;
                        shootscript.updateGun(gun.GetComponent<Gun>());
                    }
                    else if (gun.name.StartsWith("Shotgun")) {
                        gunSelected[0] = false;
                        gunSelected[1] = true;
                        gunSelected[2] = false;
                        shootscript.updateGun(gun.GetComponent<Gun>());
                    }
                    else if (gun.name.StartsWith("Sniper")) {
                        gunSelected[0] = false;
                        gunSelected[1] = false;
                        gunSelected[2] = true;
                        shootscript.updateGun(gun.GetComponent<Gun>());
                    }

                    break;
                }
            }
        }
        
        if (m_MainMenuScreen != null) m_MainMenuScreen.SetActive(false);
        if (m_GameScreen != null) m_GameScreen.SetActive(true);
        if (m_WeaponSelectionScreen != null) m_WeaponSelectionScreen.SetActive(false);
        if (m_GameOverScreen != null) m_GameOverScreen.SetActive(false);
        if (m_LeaderboardScreen != null) m_LeaderboardScreen.SetActive(false);
    
        // Enable pause menu
        if (m_PauseMenu != null && m_PauseMenuButton != null) 
            m_PauseMenuButton.gameObject.SetActive(true);
        
        ChangeGameState(GameState.Game);
    }
    
    private void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
    
    private void UpdateLeaderboardDisplay() {
        if (leaderboard_display == null) return;

        string text = "";

        for (int i = 0; i < 5; i++) {
            if (i < leaderboard.Count) {
                string localizedGun = languageManager.currentLanguage.GetValue(leaderboard[i].gun);
                text += "[" + (i + 1) + "]\t" + localizedGun + "\t\t\t" + leaderboard[i].score + "\n";
            }
            // if empty slots in leaderboard, fill it in with placeholder values
            else {
                text += "[" + (i + 1) + "]\t" + "-----" + "\t\t\t" + "-----" + "\n";
            }
        }
        leaderboard_display.text = text;
    }
}
