using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyLogic : NetworkBehaviour
{
    public static ReadyLogic instance;
    public event EventHandler onPlayerReadyChange;

    Dictionary<ulong, bool> playerReadyDictionary;
    private void Awake()
    {
        instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
        
    }

    public void CallPlayerReadyRpc()
    {
        LocalPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void LocalPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allClientsReady = false;
                break;
            }
        }
        if (allClientsReady)
        {
            Lobby.instance.DeleteLobby();
            GameStateManager.Instance.currentGameState = GameState.allPlayersReady;
            NetworkManager.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
        
    }

    [ClientRpc]
    void GameStateToggleClientRpc(ClientRpcParams clientRpcParams)
    {
        GameStateManager.Instance.currentGameState = GameState.allPlayersReady;
    }

    [ClientRpc]
    void SetPlayerReadyClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = true;
        onPlayerReadyChange?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientID)
    {
        return playerReadyDictionary[clientID] && playerReadyDictionary.ContainsKey(clientID) ;
    }
}
