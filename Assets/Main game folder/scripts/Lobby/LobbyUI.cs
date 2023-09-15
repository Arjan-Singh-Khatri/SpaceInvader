using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Core;
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
    [SerializeField] TMP_InputField playerName;

    [SerializeField] Transform lobbyListGameobject;
    [SerializeField] Transform template;
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
            Lobby.instance.LeaveLobby();
            SceneManager.LoadScene("MainMenu");
        });

        joinWithCode.onClick.AddListener(() =>
        {
            Lobby.instance.JoinWithCode(lobbyCode.text);
        });
        template.gameObject.SetActive(false);
    }

    private void Start()
    {
        playerName.text = MultiplayerManager.instance.GetPlayerName();

        playerName.onValueChanged.AddListener((string newText) =>
        {
            MultiplayerManager.instance.SetPlayerName(newText);
        });

        Lobby.instance.onLobbyListChanged += Lobby_onLobbyListChanged;
        UpdateLobbyList(new List<Unity.Services.Lobbies.Models.Lobby>());
    }

    private void Lobby_onLobbyListChanged(object sender, Lobby.OnLobbyListChangedEventsArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    public void UpdateLobbyList(List<Unity.Services.Lobbies.Models.Lobby> lobbies)
    {
        foreach(Transform child in lobbyListGameobject)
        {
            if(child == template) continue;
            Destroy(child.gameObject);
        }

        foreach(Unity.Services.Lobbies.Models.Lobby lobby in lobbies)
        {
            Transform lobbyTransform = Instantiate(template,lobbyListGameobject);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSingleUi>().SetLobby(lobby);
        }

    }

    private void OnDestroy()
    {
        Lobby.instance.onLobbyListChanged -= Lobby_onLobbyListChanged;
    }
}
