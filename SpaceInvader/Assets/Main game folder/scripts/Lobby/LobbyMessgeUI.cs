using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessgeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private Button close;
    // Start is called before the first frame update
    void Start()
    {
        Lobby.instance.OnCreateLobbyStarted += Multiplayer_OnCreateLobbyStarted ;
        Lobby.instance.OnCreateLobbyFailed += Multiplayer_OnCreateLobbyFailed ;
        Lobby.instance.OnJoinStarted += Multiplayer_OnJoinStarted;
        Lobby.instance.OnJoinFailed += Multiplayer_OnJoinFailed;
        Lobby.instance.OnQuickJoinFailed += Multiplayer_OnQuickJoinFailed;

        close.onClick.AddListener(() =>
        {
            Hide();
        });
    
    }

    private void Multiplayer_OnQuickJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Quick Join Failed");
    }

    private void Multiplayer_OnJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Couldn't Join Lobby ..");
    }

    private void Multiplayer_OnJoinStarted(object sender, EventArgs e)
    {
        ShowMessage("Joining Lobby ....");
    }

    private void Multiplayer_OnCreateLobbyFailed(object sender, EventArgs e)
    {
        if (NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("Failed To Connect To Lobby");
        }else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }

    private void Multiplayer_OnCreateLobbyStarted(object sender, EventArgs e)
    {
        ShowMessage("Creating Lobby.....");
    }

    private void ShowMessage(String reason)
    {
        Show();
        message.text = reason;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }


}
