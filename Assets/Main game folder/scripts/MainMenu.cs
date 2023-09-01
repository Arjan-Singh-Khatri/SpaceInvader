using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using Unity.Netcode;
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
    [SerializeField] GameObject playerReadyPanel;
    [SerializeField] Button ready;
    [SerializeField] TextMeshProUGUI waitingForPlayer;
    [SerializeField] GameObject multiPlayerPanel;
    [SerializeField] Button client;
    [SerializeField] Button host;
    [SerializeField] GameObject gamePausedBySomePlayerPanel;

    [Header("Disconnect && Multiplayer Player Death")]
    [SerializeField] GameObject playerDeadUiMultiplayer;
    [SerializeField] Button keepWatching;
    [SerializeField] Button mainMenuPlayerDeath;
    [SerializeField] GameObject allPlayerDeadPanel;
    [SerializeField] Button allPlayerDeadMenu;

    [SerializeField] GameObject hostDisconnectUI;
    [SerializeField] Button hostDisconnectMenu;
    

    // Start is called before the first frame update
    void Start()
    {
        Events.playerPanelToggleOn += PlayerPanelToggleOn;
        Events.playerReadyPanelToggleOff += PlayerReadyPanelToggleOff;
        Events.gamePausedBySomePlayerMulti += LocalGamePausedBySomePlayer;
        Events.gameUnpausedBySomePlayerMulti += LocalGameUnPaused;
        Events.gameOver += GameOverUI;
        Events.ammoCount += AmmoCount;
        Events.healthCount += HealthSlider;
        Events.allPlayerDeadUI += AllPlayerDeadUi;
        Events.playerDeathUI += PlayerDeadUi;
        Events.hostDisconnect += HostDisconnectUi;
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
        ready.onClick.AddListener(() =>
        {
            Ready();
        });

        #region Netcode UI


        host.onClick.AddListener(() =>
        {
            GameStateManager.Instance.currentGameState = GameState.waitingForPlayers;
            NetworkManager.StartHost();
            MultiPlayerPanelToggle();
            PlayerReadyPanelToggle();

        });

        client.onClick.AddListener(() =>
        {
            GameStateManager.Instance.currentGameState = GameState.waitingForPlayers;
            NetworkManager.StartClient();
            MultiPlayerPanelToggle();
            PlayerReadyPanelToggle();

        });
        #endregion
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            if (LocalGamePaused)
            {
                Resume();
            }

            else
            {
                Pause();
            }

        }
        if (GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
            multiPlayerPanel.SetActive(false);

        if (!(GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)) return;
        if (!IsOwner) return;


    }

    #region Player Death and Disconnect 

    void HostDisconnectUi()
    {
        playerDeadUiMultiplayer.SetActive(false);
        allPlayerDeadPanel.SetActive(false);
        hostDisconnectUI.SetActive(true);
    }

    void PlayerDeadUi()
    {
        if(GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer && IsOwner)
            playerDeadUiMultiplayer.SetActive(true);

    }
    void KeepWatching()
    {
        if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer && IsOwner)
            playerDeadUiMultiplayer.SetActive(false);
    }

    void AllPlayerDeadUi()
    {
        playerDeadUiMultiplayer.SetActive(false);
        allPlayerDeadPanel.SetActive(true);
    }

    #endregion

    #region MultiplayerGameStarted
    void Ready()
    {
        //if (!IsOwner) return;
        ready.gameObject.SetActive(false);
        Events.playerReady();
    }

    void PlayerReadyPanelToggle()
    {
        playerReadyPanel.SetActive(true);   
    }
    void PlayerReadyPanelToggleOff()
    {
        playerReadyPanel.SetActive(false);
    }

    void LocalGamePausedBySomePlayer()
    {
        gamePausedBySomePlayerPanel.SetActive(true);

    }

    void LocalGameUnPaused()
    {
        gamePausedBySomePlayerPanel.SetActive(false);

    }
    #endregion

    #region Menu UI
    private void MultiPlayerPanelToggle()
    {
        multiPlayerPanel.SetActive(false);
    }
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
        SceneManager.LoadScene("MainMenu");
    }

    private void Resume()
    {
        pauseMenuPanel.SetActive(false);
        LocalGamePaused = false;
        Time.timeScale = 1f;
        Events.CallToUnPauseGameMulti();
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void Pause()
    {
        pauseMenuPanel.SetActive(true);
        LocalGamePaused = true;
        Events.CallToPauseGameMulti();
        Time.timeScale = 0f;

    }



    private void GameOver()
    {
        if(GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
        {
            gameOverPanel.SetActive(true);
            LocalGamePaused = true;
            Time.timeScale = 0f;
        }

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
            if (IsOwner) 
            {
                gameOverPanel.SetActive(true);
            }
            
        }
        else
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }

    }

    void HealthSlider(int health)
    {
        healthSlider.value = health;
        Debug.Log(healthSlider.value);
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
        waveText.gameObject.SetActive(false);
    }
    #endregion


    private void OnDisable()
    {
        Events.gameOver -= GameOverUI;
        Events.waveDelegate -= WaveTextMove;
        Events.ammoCount -= AmmoCount;
        Events.healthCount -= HealthSlider;
    }

}
