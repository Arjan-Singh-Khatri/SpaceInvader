using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyLogic : NetworkBehaviour
{
    public static ReadyLogic instance;
    

    Dictionary<ulong, bool> playerReadyDictionary;
    private void Start()
    {
        instance = this;
    }

    public void CallPlayerReadyRpc()
    {
        LocalPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void LocalPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
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

            NetworkManager.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
    }


}
