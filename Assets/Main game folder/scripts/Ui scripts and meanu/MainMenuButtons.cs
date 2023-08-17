using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] Button play;
    [SerializeField] Button quit;
    [SerializeField] Button Multiplayer;
    [SerializeField] GameObject panelUI;
    [SerializeField] Transform panelPos;
    [SerializeField] Ease ease;
    
    // Start is called before the first frame update
    void Start()
    {
        play.onClick.AddListener(Play);
        quit.onClick.AddListener(Quit);
        //PanelTransition();
    }
    
    void Play()
    {
        SceneManager.LoadScene(0);
    }
    void Quit()
    {
        Application.Quit();
    }
    
    void PanelTransition()
    {
        panelUI.gameObject.transform.DOMove(panelPos.position, 3f).SetEase(ease);
    }
}
