using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("RandomVariables")]
    private bool gamePaused = false;

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

    [Header("Wave Text")]
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] Ease ease;
    Vector3 waveTextOgPosition = new Vector3(0,273f, 0);
    
    

    // Start is called before the first frame update
    void Start()
    {
        Events.gameOver += GameOverUI;
        Events.waveDelegate += WaveTextMove;
        Events.ammoCount += AmmoCount;
        Events.healthCount += HealthSlider;
        pause.onClick.AddListener(Pause);
        resume.onClick.AddListener(Resume);
        mainMenu.onClick.AddListener(GoToMainMenu);
        quit.onClick.AddListener(QuitGame);
        playAgain.onClick.AddListener(PlayAgain);
        mainMenuGameOver.onClick.AddListener(GoToMainMenu);
        bulletLeftText.text = "X" + 25.ToString();
        missileLeftText.text ="X" + 6.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            if (gamePaused)
                Resume();
            else
                Pause();
        }   
    }

    #region Menu UI
    private void PlayAgain()
    {
        SceneManager.LoadScene(0);
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }

    private void Resume()
    {
        pauseMenuPanel.SetActive(false);
        gamePaused = false;
        Time.timeScale = 1f;
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void Pause()
    {
        pauseMenuPanel.SetActive(true);
        gamePaused = true;
        Time.timeScale = 0f;
    }

    private void GameOver()
    {
        gameOverPanel.SetActive(true);
        gamePaused = true;
        Time.timeScale = 0f;

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
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void HealthSlider(int health)
    {
        healthSlider.value = health;
        Debug.Log(healthSlider.value);
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
