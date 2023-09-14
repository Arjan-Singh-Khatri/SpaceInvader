using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : NetworkBehaviour
{
    public int MAX_PLAYER_COUNT = 2;
    private string PLAYER_PREF_PLAYER_NAME_MULTIPLAYER;
    public static MultiplayerManager instance;

    Dictionary<ulong, bool> playerPauseDictionary;
    Dictionary<ulong, bool> playerDeathDictionary;

    [SerializeField] private Transform playerPrefab;

    public event EventHandler onTryingToJoinGame;
    public event EventHandler onFailedToJoinGame;
    public event EventHandler onPlayerDataListChange;

    [SerializeField]private NetworkList<PlayerData> playerDatanNetworkListSO;
    [SerializeField]private List<Color> playerColorList;

    private string playerName;  

    private void Awake()
    {
        instance = this;
        
        playerPauseDictionary = new Dictionary<ulong, bool>();
        playerDeathDictionary = new Dictionary<ulong, bool>();

        playerDatanNetworkListSO = new NetworkList<PlayerData>();
        playerDatanNetworkListSO.OnListChanged += PlayerDatanNetworkListSO_OnListChanged;

        playerName = PlayerPrefs.GetString(PLAYER_PREF_PLAYER_NAME_MULTIPLAYER,"Player Name " +  UnityEngine.Random.Range(100, 1000));
        DontDestroyOnLoad(gameObject);
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
        PlayerPrefs.SetString(PLAYER_PREF_PLAYER_NAME_MULTIPLAYER, playerName);
    }

    private void PlayerDatanNetworkListSO_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        onPlayerDataListChange?.Invoke(this, EventArgs.Empty);
    }

    // Start is called before the first frame update
    void Start()
    {

        Events.instance.CallToPauseGameMulti += CallRpcToPauseGame;
        Events.instance.CallToUnPauseGameMulti += CallRpcToUnPauseGame;
        Events.instance.playerDeath += PlayerDeathRpcCall;
        //Events.instance.waveDelegate += CallWaveUIRpc;

    }


    public override void OnNetworkSpawn()
    {
        //
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += On_Disconnect_Local_Player;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDatanNetworkListSO.Add(new PlayerData
        {
            clientId = clientId,
        });
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName != "SampleScene") return;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform instantiatedObject = Instantiate(playerPrefab);
            instantiatedObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }

    }

    #region Late join

    public void StartTheHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManagr_ConnectionApprovalCallBack;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
        NetworkManager.StartHost();
    }

    private void NetworkManager_Server_OnClientConnectedCallback(ulong obj)
    {
        SetPlayerServerRpc(GetPlayerName());
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for(int i = 0; i < playerDatanNetworkListSO.Count ; i++)
        {
            PlayerData playerData = playerDatanNetworkListSO[i];
            if(clientId == playerData.clientId)
            {
                playerDatanNetworkListSO.RemoveAt(i);
            }
        }
    }

    private void NetworkManagr_ConnectionApprovalCallBack(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != "CharacterSelect")
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game Already Started ";
            return;
        }
        if (NetworkManager.ConnectedClientsList.Count >= MAX_PLAYER_COUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game Is Full";
            return;
        }

        connectionApprovalResponse.Approved = true;


    }

    public void StartClient()
    {
        onTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.StartClient();
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientID)
    {
        SetPlayerServerRpc(GetPlayerName());
    }

    [ServerRpc(RequireOwnership = false)]
    void SetPlayerServerRpc(string playerName ,ServerRpcParams rpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(rpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDatanNetworkListSO[playerDataIndex];
        playerData.playerName = playerName;

        playerDatanNetworkListSO[playerDataIndex] = playerData;
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientID)
    {
        onFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    //#region Wave

    //void CallWaveUIRpc(int waveNumber)
    //{
    //    WaveUIServerRpc(waveNumber);
    //}

    //[ServerRpc(RequireOwnership =false)]
    //void WaveUIServerRpc(int waveNumber,ServerRpcParams serverRpcParams = default)
    //{
    //    WaveUIClientRpc(waveNumber, new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds } });
    //}

    //[ClientRpc]
    //void WaveUIClientRpc(int waveNumber,ClientRpcParams clientRpcParams)
    //{
    //    Events.instance.waveDelegate(waveNumber);
    //}
    //#endregion

    #region Disconnect Handel and Player Death Online
    //Disconnect handling

    private void On_Disconnect_Local_Player(ulong clientId)
    {
        if(OwnerClientId == clientId)
        {
            CallRpcToUnPauseGame();
        }
        if(clientId == NetworkManager.ServerClientId)
        {
            HostDisconnectServerRpc();
        }
    }

    [ServerRpc(RequireOwnership =false)]
    void HostDisconnectServerRpc(ServerRpcParams serverRpcParams= default)
    {
        HostDisconnectClientRpc(new ClientRpcParams {Send= new ClientRpcSendParams { TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds } });
    }

    [ClientRpc]
    void HostDisconnectClientRpc(ClientRpcParams clientRpcParams)
    {
        Events.instance.hostDisconnect();
    }

    //Player Death Handling
    void PlayerDeathRpcCall()
    {
        if (!IsOwner) return;
        LocalPlayerDeathServerRpc();
    }
    [ServerRpc(RequireOwnership =false)]
    void LocalPlayerDeathServerRpc(ServerRpcParams serverRpcParams= default)
    {
        playerDeathDictionary[serverRpcParams.Receive.SenderClientId] = true;
        bool allClientDead = true;
        foreach(var clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerDeathDictionary.ContainsKey(clientID) || !playerDeathDictionary[clientID]) 
            {
                allClientDead = false;
                break;
            }
        }
        if (allClientDead)
        {
            AllPlayerDeathClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds } });
        }
        else
            LocalPlayerDeadClientRpc();
    }
    [ClientRpc]
    void AllPlayerDeathClientRpc(ClientRpcParams clientRpcParams)
    {
        // Event That activates All PlayerDead Ui - Host Stop - all TimeScale = 0 
        Events.instance.allPlayerDeadUI();
        GameStateManager.Instance.currentGamePhase = GamePhase.allPlayersDead;
        Time.timeScale = 0f;

    }
    [ClientRpc]
    void LocalPlayerDeadClientRpc()
    {
        // Event That Activates Continue Watching or Quit Ui 
        Events.instance.playerDeathUI();
    }
    #endregion

    #region Ready and Pause


    //void GameStateToggle()
    //{
    //    Events.instance.playerReadyPanelToggleOff();
    //    Events.instance.playerPanelToggleOn();
    //    GameStateManager.Instance.currentGameState = GameState.allPlayersReady;
    //}

    // Functions to call ServerRpc via Events
    private void CallRpcToPauseGame()
    {
        CallToPauseTheGameServerRpc();
    }

    private void CallRpcToUnPauseGame()
    {
        CallToUnpauseTheGameServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void CallToPauseTheGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        bool onePause = false;
        playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = true;
        foreach(var client in NetworkManager.Singleton.ConnectedClientsIds)
        {
            //even one paused then game paused else not paused 
            if (playerPauseDictionary[client] && playerPauseDictionary.ContainsKey(client))
            {
                onePause = true;
                break;
            }
        }
        if(onePause)
        {
            List<ulong> clientIds = new List<ulong>();
            foreach(var client in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if(client != OwnerClientId)
                {
                    clientIds.Add(client);  
                }
            }
            CallToPauseAllClientsCLientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = clientIds } });
        }


    }
    [ServerRpc(RequireOwnership =false)]
    private void CallToUnpauseTheGameServerRpc(ServerRpcParams serverRpcParams = default)
    {

        bool allUnpaused = true;
        playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = false;
        foreach (var client in NetworkManager.Singleton.ConnectedClientsIds)
        {
            //even one paused then game paused else not paused 
            if (!playerPauseDictionary[client] && !playerPauseDictionary.ContainsKey(client))
            {
                allUnpaused = false;
                break;
            }
        }

        if (!allUnpaused)
        {
            CallToUnPauseAllClientsClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds } });
        }
    }
    [ClientRpc]
    private void CallToPauseAllClientsCLientRpc(ClientRpcParams clientRpcParams)
    {
        Events.instance.gamePausedBySomePlayerMulti();
        GameStateManager.Instance.currentGameState = GameState.gamePaused;
    }


    [ClientRpc]
    private void CallToUnPauseAllClientsClientRpc(ClientRpcParams clientRpcParams)
    {
        Events.instance.gameUnpausedBySomePlayerMulti();
        GameStateManager.Instance.currentGameState = GameState.allPlayersReady;
    }
    #endregion


    private void OnDestroy()
    {
        Events.instance.CallToPauseGameMulti -= CallRpcToPauseGame;
        Events.instance.CallToUnPauseGameMulti -= CallRpcToUnPauseGame;
        Events.instance.playerDeath -= PlayerDeathRpcCall;
        //Events.instance.waveDelegate -= CallWaveUIRpc;
    }

    #region CharacterSelect
    
    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex <  playerDatanNetworkListSO.Count;
    }
    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDatanNetworkListSO[playerIndex];   
    }

    public PlayerData GetPlayerDataFromClientId(ulong ClientID)
    {
        foreach(PlayerData data in playerDatanNetworkListSO)
        {
            if(data.clientId == ClientID)
            {
                return data;
            }
        }
        return default;
    }

    public Color GetPlayerColorForPlayer(int colorID)
    {
        return playerColorList[colorID];
    }

    public int GetPlayerDataIndexFromClientId(ulong ClientID)
    {
        for(int i =0; i < playerDatanNetworkListSO.Count; i++)
        {
            if (playerDatanNetworkListSO[i].clientId == ClientID)
            {
                return i;
            }
        }
        return -1;
    }
    #endregion
}
