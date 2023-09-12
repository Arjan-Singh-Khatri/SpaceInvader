using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobby : NetworkBehaviour
{
    public static Lobby instance;
    private Unity.Services.Lobbies.Models.Lobby joinedLobby;
    private float hearthBeatTimer;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeUnityAuthentication();
    }

    private void Update()
    {
        HandleLobbyHearthBeat();
    }

    private void HandleLobbyHearthBeat()
    {
        if (IsLobbyHost())
        {
            hearthBeatTimer -= Time.deltaTime;
            if(hearthBeatTimer <= 0 ) 
            {
                float MAX_HEARTHBEAT_TIMER = 15f;
                hearthBeatTimer = MAX_HEARTHBEAT_TIMER;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;

    }

    private async void InitializeUnityAuthentication()
    {
        if(UnityServices.State!=ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Random.Range(0, 1000).ToString());
            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MultiplayerManager.instance.MAX_PLAYER_COUNT, new CreateLobbyOptions { IsPrivate = isPrivate });
            MultiplayerManager.instance.StartTheHost();
            NetworkManager.SceneManager.LoadScene("CharacterSelect", LoadSceneMode.Single);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }  
    }

    public async void QuickJoinLobby()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
            MultiplayerManager.instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public Unity.Services.Lobbies.Models.Lobby GetLobby()
    {
        return joinedLobby;
    }

    public async void JoinWithCode(string lobbyCode)
    {
        try
        {
            joinedLobby =  await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            MultiplayerManager.instance.StartClient();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
