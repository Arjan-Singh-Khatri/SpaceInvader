using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListSingleUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    private Unity.Services.Lobbies.Models.Lobby lobby;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            Lobby.instance.JoinWithID(lobby.Id);    
        });
    }

    public void SetLobby(Unity.Services.Lobbies.Models.Lobby lobby)
    {
        this.lobby = lobby;
        lobbyNameText.text = lobby.Name;
    }
    
}
