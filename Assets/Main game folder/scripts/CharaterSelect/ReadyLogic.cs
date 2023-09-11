using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyLogic : NetworkBehaviour
{
    public static ReadyLogic instance;
    

    Dictionary<ulong, bool> playerReadyDictionary;
    private void Start()
    {
        playerReadyDictionary = new Dictionary<ulong, bool>();
        instance = this;
    }

    public void CallPlayerReadyRpc()
    {
        LocalPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void LocalPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("Called");
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
            GameStateManager.Instance.currentGameState = GameState.allPlayersReady;
            //GameStateToggleClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = NetworkManager.ConnectedClientsIds } });
            NetworkManager.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
        
    }

    [ClientRpc]
    void GameStateToggleClientRpc(ClientRpcParams clientRpcParams)
    {
        GameStateManager.Instance.currentGameState = GameState.allPlayersReady;
    }


}
