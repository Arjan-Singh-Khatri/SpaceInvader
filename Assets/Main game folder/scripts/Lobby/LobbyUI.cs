using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{

    [SerializeField] Button createLobby;
    [SerializeField] Button menu;
    [SerializeField] Button quickJoinLobby;
    [SerializeField] Button joinWithCode;
    [SerializeField] TMP_InputField lobbyCode;
    [SerializeField] CreateUi createUi;
    // Start is called before the first frame update
    void Awake()
    {
        createLobby.onClick.AddListener(() =>
        {
            createUi.show();
        });

        quickJoinLobby.onClick.AddListener(() =>
        {
            Lobby.instance.QuickJoinLobby();
        });

        menu.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MainMenu");
        });

        joinWithCode.onClick.AddListener(() =>
        {
            Lobby.instance.JoinWithCode(lobbyCode.text);
        });

    }

}
