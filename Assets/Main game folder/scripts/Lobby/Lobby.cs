using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Lobby : NetworkBehaviour
{

    public event EventHandler<OnLobbyListChangedEventsArgs> onLobbyListChanged;   
    public class OnLobbyListChangedEventsArgs : EventArgs { 
        
        public List<Unity.Services.Lobbies.Models.Lobby> lobbyList;
    }

    public static Lobby instance;
    private Unity.Services.Lobbies.Models.Lobby joinedLobby;
    private float hearthBeatTimer;
    private float listLobbyTimer;
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
        HandleListLobbyCheck();
    }

    private void HandleListLobbyCheck()
    {
        listLobbyTimer -= Time.deltaTime;
        if(listLobbyTimer <= 0f)
        {
            float maxListLobbyTimer = 3f;
            listLobbyTimer = maxListLobbyTimer;
            LIstLobbies();
        }
    }

    private void HandleLobbyHearthBeat()
    {
        if (!AuthenticationService.Instance.IsSignedIn) return;
        if (joinedLobby != null) return;
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
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 1000).ToString());
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


    public async void JoinWithID(string lobbyID)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID);
            MultiplayerManager.instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void DeleteLobby()
    {
        if(joinedLobby !=null )
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }

        }
    }

    public async void LeaveLobby()
    {
        if(joinedLobby!= null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;
            }catch(LobbyServiceException e) 
            {
                Debug.Log(e);
            }
        }
    }

    public async void LIstLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
            {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            }
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            onLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventsArgs
            {
                lobbyList = queryResponse.Results
            }) ;
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);   
        }
    }
}
