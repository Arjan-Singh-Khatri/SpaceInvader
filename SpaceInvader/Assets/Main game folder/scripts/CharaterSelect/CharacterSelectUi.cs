using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectUi : MonoBehaviour
{
    [SerializeField] Button ready;
    [SerializeField] Button menu;
    [SerializeField] TextMeshProUGUI lobbyName;
    [SerializeField] TextMeshProUGUI lobbyCode;

    private void Awake()
    {
        ready.onClick.AddListener(() =>
        {
            ReadyLogic.instance.CallPlayerReadyRpc();
        });

        menu.onClick.AddListener(() =>
        {
            Lobby.instance.LeaveLobby();
            SceneManager.LoadScene("MainMenu");
        });


    }

    private void Start()
    {
        Unity.Services.Lobbies.Models.Lobby lobby = Lobby.instance.GetLobby();
        lobbyName.text = "Lobby Name :" + lobby.Name;
        lobbyCode.text = "Lobby Code :" + lobby.LobbyCode;
    }
}
