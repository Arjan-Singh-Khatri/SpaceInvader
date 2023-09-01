using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerManager : NetworkBehaviour
{
    Dictionary<ulong, bool> playerReadyDictionary = new();
    Dictionary<ulong, bool> playerPauseDictionary = new();
    Dictionary<ulong, bool> playerDeathDictionary = new();
    private bool autoPauseUpdate = false;
    // Start is called before the first frame update
    void Start()
    {
        Events.playerReady += CallPlayerReadyRpc;
        Events.CallToPauseGameMulti += CallRpcToPauseGame;
        Events.CallToUnPauseGameMulti += CallRpcToUnPauseGame;
        Events.playerDeath += PlayerDeathRpcCall;
        Events.waveDelegate += CallWaveUIRpc;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += On_Disconnect_Local_Player;
        }
    }


    #region Wave

    void CallWaveUIRpc(int waveNumber)
    {
        WaveUIServerRpc(waveNumber);
    }

    [ServerRpc(RequireOwnership =false)]
    void WaveUIServerRpc(int waveNumber,ServerRpcParams serverRpcParams = default)
    {
        WaveUIClientRpc(waveNumber, new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds } });
    }

    [ClientRpc]
    void WaveUIClientRpc(int waveNumber,ClientRpcParams clientRpcParams)
    {
        Events.waveDelegate(waveNumber);
    }
    #endregion

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
        Events.hostDisconnect();
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
        Events.allPlayerDeadUI();
        GameStateManager.Instance.currentGamePhase = GamePhase.allPlayersDead;
        Time.timeScale = 0f;

    }
    [ClientRpc]
    void LocalPlayerDeadClientRpc()
    {
        // Event That Activates Continue Watching or Quit Ui 
        Events.playerDeathUI();
    }
    #endregion

    #region Ready and Pause
    void CallPlayerReadyRpc()
    {
        LocalPlayerReadyServerRpc();
        Debug.Log("CAlled");
    }

    [ServerRpc(RequireOwnership = false)]
    void LocalPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        bool allClientsReady = true;
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if(!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allClientsReady = false;
                break;
            }
        }
        if(allClientsReady) 
        {
           
            AllClientsReadyClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds}});
        }
    }

    [ClientRpc]
    void AllClientsReadyClientRpc(ClientRpcParams clientRpcParams)
    {
        GameStateToggle();
    }

    void GameStateToggle()
    {
        Events.playerReadyPanelToggleOff();
        Events.playerPanelToggleOn();
        GameStateManager.Instance.currentGameState = GameState.allPlayersReady;
    }

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
            CallToPauseAllClientsCLientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds } });
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
        Events.gamePausedBySomePlayerMulti();
        GameStateManager.Instance.currentGameState = GameState.gamePaused;
    }


    [ClientRpc]
    private void CallToUnPauseAllClientsClientRpc(ClientRpcParams clientRpcParams)
    {
        Events.gameUnpausedBySomePlayerMulti();
        GameStateManager.Instance.currentGameState = GameState.allPlayersReady;
    }
    #endregion
}
