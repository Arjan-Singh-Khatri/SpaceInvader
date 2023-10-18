using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : NetworkBehaviour
{
    public int MAX_PLAYER_COUNT = 2;
    private string PLAYER_PREF_PLAYER_NAME_MULTIPLAYER;
    public static MultiplayerManager instance;

    Dictionary<ulong, bool> playerPauseDictionary;
    Dictionary<ulong, bool> playerDeathDictionary;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);

    [SerializeField] private Transform playerPrefab;

    public event EventHandler onTryingToJoinGame;
    public event EventHandler onFailedToJoinGame;
    public event EventHandler onPlayerDataListChange;

    [SerializeField]private NetworkList<PlayerData> playerDataNetworkListSO;
    [SerializeField]private List<Color> playerColorList;

    private string playerName;
    private bool autoTestPause = false;

    private void Awake()
    {
        instance = this;
        
        playerPauseDictionary = new Dictionary<ulong, bool>();
        playerDeathDictionary = new Dictionary<ulong, bool>();

        playerDataNetworkListSO = new NetworkList<PlayerData>();
        playerDataNetworkListSO.OnListChanged += PlayerDatanNetworkListSO_OnListChanged;
        

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
        Debug.Log("Changed : ");
        onPlayerDataListChange?.Invoke(this, EventArgs.Empty);
        
    }

 

    // Start is called before the first frame update
    void Start()
    {
        Events.instance.playerDeath += PlayerDeathRpcCall;
        Events.instance.CallToPauseGameMulti += CallToPauseGame;
        Events.instance.CallToUnPauseGameMulti += CallToUnpauseGame;
        //Events.instance.waveDelegate += CallWaveUIRpc;

    }

    private void LateUpdate()
    {
        if (autoTestPause)
        {
            autoTestPause = false;
            TestPausedState();
        }
    }


    public override void OnNetworkSpawn()
    {

        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += On_Disconnect_Local_PlayerCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
            
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientID)
    {
        if(clientID == NetworkManager.ServerClientId) 
        {
            Events.instance.hostDisconnect();
        }
    }

    private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        
        if (isGamePaused.Value)
        {
            Debug.Log(isGamePaused.Value);
            Events.instance.GamePausedMultiplayer();
        }else
        {
            Events.instance.GameUnpausedMultiplayer();
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkListSO.Add(new PlayerData
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
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
        NetworkManager.StartHost();
    }

    private void NetworkManager_Server_OnClientConnectedCallback(ulong obj)
    {
        SetPlayerNameServerRpc(GetPlayerName());
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for(int i = 0; i < playerDataNetworkListSO.Count ; i++)
        {
            PlayerData playerData = playerDataNetworkListSO[i];
            if(clientId == playerData.clientId)
            {
                playerDataNetworkListSO.RemoveAt(i);
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
        SetPlayerNameServerRpc(GetPlayerName());
    }

    [ServerRpc(RequireOwnership = false)]
    void SetPlayerNameServerRpc(string playerName ,ServerRpcParams rpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(rpcParams.Receive.SenderClientId);
       
        PlayerData playerData = playerDataNetworkListSO[playerDataIndex];
        playerData.playerName = playerName;

        playerDataNetworkListSO[playerDataIndex] = playerData;
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientID)
    {
        onFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }
    #endregion


    #region Disconnect Handel and Player Death Online
    //Disconnect handling

    private void On_Disconnect_Local_PlayerCallback(ulong clientId)
    {
        autoTestPause = true;
    }


    //Player Death Handling
    void PlayerDeathRpcCall()
    {
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

    private void CallToPauseGame()
    {
        PauseGameServerRpc();
    }

    private void CallToUnpauseGame()
    {
        UnPauseGameServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = true;
        TestPausedState();
    }

    [ServerRpc(RequireOwnership =false)]
    private void UnPauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = false;
        TestPausedState();
    }

    private void TestPausedState()
    {
        foreach(var client in NetworkManager.Singleton.ConnectedClientsIds)
        {
            //even one paused then game paused else not paused 
            if (playerPauseDictionary.ContainsKey(client) && playerPauseDictionary[client] )
            {
                // this paused 
                isGamePaused.Value = true;
                return;
            }
        }
        // all unpaused
        isGamePaused.Value = false;
        

    }

    #endregion




    #region CharacterSelect
    
    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex <  playerDataNetworkListSO.Count;
    }
    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkListSO[playerIndex];   
    }

    public PlayerData GetPlayerDataFromClientId(ulong ClientID)
    {
        foreach(PlayerData data in playerDataNetworkListSO)
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
        for(int i =0; i < playerDataNetworkListSO.Count; i++)
        {
            if (playerDataNetworkListSO[i].clientId == ClientID)
            {
                return i;
            }
        }
        return -1;
    }
    #endregion

    private void OnDestroy()
    {

        Events.instance.playerDeath -= PlayerDeathRpcCall;
        //Events.instance.waveDelegate -= CallWaveUIRpc;
        Events.instance.CallToPauseGameMulti -= CallToPauseGame;
        Events.instance.CallToUnPauseGameMulti -= CallToUnpauseGame;
    }
}
