using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : NetworkBehaviour
{
    [Header("RandomVariables")]
    private bool LocalGamePaused = false;

    [Header("Menu UI")]
    [SerializeField] Button pause;
    [SerializeField] Button mainMenu;
    [SerializeField] Button mainMenuGameOver;
    [SerializeField] Button resume;
    [SerializeField] Button quit;
    [SerializeField] GameObject pauseMenuPanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] Button playAgain;

    [Header("Player UI")]
    [SerializeField] TextMeshProUGUI bulletLeftText;
    [SerializeField] TextMeshProUGUI missileLeftText;
    [SerializeField] Slider healthSlider;
    [SerializeField] GameObject playerUiPanel;

    [Header("Wave Text")]
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] Ease ease;
    Vector3 waveTextOgPosition = new Vector3(0,273f, 0);

    [Header("Multiplayer  && Game state State")]

    [SerializeField] GameObject gamePausedBySomePlayerPanel;

    [Header("Disconnect && Multiplayer Player Death")]
    [SerializeField] GameObject playerDeadUiMultiplayer;
    [SerializeField] Button keepWatching;
    [SerializeField] Button mainMenuPlayerDeath;
    [SerializeField] GameObject allPlayerDeadPanel;
    [SerializeField] Button allPlayerDeadMenu;

    [SerializeField] GameObject hostDisconnectUI;
    [SerializeField] Button hostDisconnectMenu;

    [Header("GameOver Boss Dead")]
    [SerializeField] private GameObject gameWonPanel;
    

    // Start is called before the first frame update
    void Start()
    {


        Events.instance.playerPanelToggleOn += PlayerPanelToggleOn;
        //Events.instance.playerReadyPanelToggleOff += PlayerReadyPanelToggleOff;

        Events.instance.GamePausedMultiplayer += GamePausedWaiting;
        Events.instance.GameUnpausedMultiplayer += GameUnPausedMultiplayer;

        Events.instance.GameWonUI += GameWon;

        Events.instance.gameOver += GameOverUI;
        Events.instance.ammoCount += AmmoCount;
        Events.instance.healthCount += HealthSlider;
        Events.instance.allPlayerDeadUI += AllPlayerDeadUi;
        Events.instance.playerDeathUI += PlayerDeadUi;
        Events.instance.hostDisconnect += HostDisconnectUi;

        Events.instance.waveTextDelegate += WaveTextMove;

        #region AddingListeners 
        hostDisconnectMenu.onClick.AddListener(GoToMainMenu);
        allPlayerDeadMenu.onClick.AddListener(GoToMainMenu);
        pause.onClick.AddListener(Pause);
        resume.onClick.AddListener(Resume);
        mainMenu.onClick.AddListener(GoToMainMenu);
        quit.onClick.AddListener(QuitGame);
        playAgain.onClick.AddListener(PlayAgain);
        mainMenuGameOver.onClick.AddListener(GoToMainMenu);
        bulletLeftText.text = "X" + 25.ToString();
        missileLeftText.text ="X" + 6.ToString();
        mainMenuPlayerDeath.onClick.AddListener(GoToMainMenu);

        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        if (LocalGamePaused)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Resume();
            }
            else if (Input.GetKeyUp(KeyCode.M))
            {
                GoToMainMenu();

            }
            else if (Input.GetKeyUp(KeyCode.Q))
            {
                QuitGame();
            }
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Pause();
            }
        }

        if (Input.GetKeyUp(KeyCode.K))
        {
            KeepWatching();
        }

    }

    #region Game Won
    void GameWon()
    {
        gameWonPanel.SetActive(true);
    }

    #endregion

    #region Player Death and Disconnect 

    void HostDisconnectUi()
    {
        playerDeadUiMultiplayer.SetActive(false);
        allPlayerDeadPanel.SetActive(false);
        hostDisconnectUI.SetActive(true);
        LocalGamePaused = true;
    }

    void PlayerDeadUi()
    {
        playerDeadUiMultiplayer.SetActive(true);
        LocalGamePaused = true;

    }
    void KeepWatching()
    {
        //Removed is owner
        playerDeadUiMultiplayer.SetActive(false);
        LocalGamePaused = false;
    }

    void AllPlayerDeadUi()
    {
        playerDeadUiMultiplayer.SetActive(false);
        GameOverUI();
        LocalGamePaused = true;
    }

    #endregion

    

    #region Menu UI
    //private void MultiPlayerPanelToggle()
    //{
    //    multiPlayerPanel.SetActive(false);
    //}
    private void PlayAgain()
    {
        SceneManager.LoadScene("SampleScene");
    }

    private void GoToMainMenu()
    {
        if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer && IsOwner)
        {
            NetworkManager.Singleton.Shutdown();
        }
        Time.timeScale = 1f;

        #region CleanUp
        if (Events.instance != null)
        {
            Destroy(Events.instance.gameObject);
        }
        if(GameStateManager.Instance != null)
        {
            Destroy(GameStateManager.Instance.gameObject);
        }
        if(GameStateManager.Instance!= null)
        {
            Destroy(BgMusic.instance.gameObject);
        }
        #endregion

        SceneManager.LoadScene("MainMenu");
    }

    private void Resume()
    {
        pauseMenuPanel.SetActive(false);
        LocalGamePaused = false;

        if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
        {
            Events.instance.CallToUnPauseGameMulti();
        }
        Time.timeScale = 1f;
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void Pause()
    {
        pauseMenuPanel.SetActive(true);
        LocalGamePaused = true;

        if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
        {
            Events.instance.CallToPauseGameMulti();
        }

        if(GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
        {
            Time.timeScale = 0; 
        }
    }


    private void GamePausedWaiting()
    {
        GameStateManager.Instance.currentGameState = GameState.gamePaused;
        gamePausedBySomePlayerPanel.SetActive(true);
        Time.timeScale = 0f;
        LocalGamePaused = true;
    }

    private void GameUnPausedMultiplayer()
    {
        Time.timeScale = 1f;
        GameStateManager.Instance.currentGameState = GameState.allPlayersReady;
        gamePausedBySomePlayerPanel.SetActive(false);
        LocalGamePaused = false;
        
    }



    #endregion

    #region Player UI
    void AmmoCount(int bullet, int missile)
    {
        bulletLeftText.text = "X" + bullet.ToString();
        missileLeftText.text = "X" + missile.ToString();

    }

    void GameOverUI()
    {
        if(GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
        {
            gameOverPanel.SetActive(true);
            LocalGamePaused = true;
            
        }
        else
        {
            gameOverPanel.SetActive(true);
            LocalGamePaused = true;
        }

    }

    void HealthSlider(int health)
    {
        healthSlider.value = health;

    }

    void PlayerPanelToggleOn()
    {
        playerUiPanel.SetActive(true);
    }
    #endregion


    #region Wave Text UI
    void WaveTextMove(int waveNum)
    {
        waveText.gameObject.SetActive(true);
        waveText.text = "Wave " + waveNum.ToString();
        waveText.rectTransform.DOAnchorPos(new Vector3(0, -434, 0), 3f).SetEase(ease);
        Invoke(nameof(WaveTextPositionReturn), 3f);
        
    }
    void WaveTextPositionReturn()
    {
        waveText.rectTransform.DOAnchorPos(waveTextOgPosition, 3f).SetEase(ease);
        
    }
    #endregion



    private new void OnDestroy() 
    {
        Events.instance.playerPanelToggleOn -= PlayerPanelToggleOn;
        //Events.instance.playerReadyPanelToggleOff += PlayerReadyPanelToggleOff;

        Events.instance.GamePausedMultiplayer -= GamePausedWaiting;
        Events.instance.GameUnpausedMultiplayer -= GameUnPausedMultiplayer;
        Events.instance.gameOver -= GameOverUI;
        Events.instance.ammoCount -= AmmoCount;
        Events.instance.healthCount -= HealthSlider;
        Events.instance.allPlayerDeadUI -= AllPlayerDeadUi;
        Events.instance.playerDeathUI -= PlayerDeadUi;
        Events.instance.hostDisconnect -= HostDisconnectUi;
    }

}
