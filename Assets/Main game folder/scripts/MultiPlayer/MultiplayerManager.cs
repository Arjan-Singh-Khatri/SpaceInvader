using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerManager : NetworkBehaviour
{
    Dictionary<ulong, bool> playerReadyDictionary = new();
    Dictionary<ulong, bool> playerPauseDictionary = new();
    // Start is called before the first frame update
    void Start()
    {
        Events.playerReady += callPlayerReadyRpc;
        Events.CallToPauseGameMulti += CallRpcToPauseGame;
        Events.CallToUnPauseGameMulti += CallRpcToUnPauseGame;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void callPlayerReadyRpc()
    {
        LocalPlayerReadyServerRpc();
        Debug.Log("CAlled");
    }
    [ServerRpc(RequireOwnership = false)]
    void LocalPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        Debug.Log(NetworkManager.Singleton.ConnectedClientsList);
        bool allClientsReady = true;
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if(!playerReadyDictionary.ContainsKey(clientId) && !playerReadyDictionary[clientId])
            {
                allClientsReady = false;
                break;
            }
        }
        if(allClientsReady) 
        {
            Debug.Log("All Clients Ready");
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
        List<ulong> ClientIds = new List<ulong>();
        playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = true;
        foreach(var client in NetworkManager.Singleton.ConnectedClientsIds)
        {
            //even one paused then game paused else not paused 
            if (playerPauseDictionary[client] && playerPauseDictionary.ContainsKey(client))
            {
                break;
            }
        }

        foreach(var client in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if(client == serverRpcParams.Receive.SenderClientId)
            {
                continue;
            }else
            {
                ClientIds.Add(client);  
            }
        }
        CallToPauseAllClientsCLientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = ClientIds } });

    }
    [ServerRpc(RequireOwnership =false)]
    private void CallToUnpauseTheGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        List<ulong> ClientIds = new List<ulong>();
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
        foreach (var client in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (client == serverRpcParams.Receive.SenderClientId)
            {
                continue;
            }
            else
            {
                ClientIds.Add(client);
            }
        }
        if (allUnpaused)
        {
            CallToUnPauseAllClientsClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = ClientIds } });
        }
    }
    [ClientRpc]
    private void CallToPauseAllClientsCLientRpc(ClientRpcParams clientRpcParams)
    {
        Events.gamePausedBySomePlayerMulti();
    }


    [ClientRpc]
    private void CallToUnPauseAllClientsClientRpc(ClientRpcParams clientRpcParams)
    {
        Events.gameUnpausedBySomePlayerMulti();
    }
}
