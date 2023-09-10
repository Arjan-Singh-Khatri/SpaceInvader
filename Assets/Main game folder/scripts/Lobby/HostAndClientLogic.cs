using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostAndClientLogic : NetworkBehaviour
{
    public int MAX_PLAYER_COUNT = 4;
    public event EventHandler onTryingToJoinGame;
    public event EventHandler onFailedToJoinGame;

    public static HostAndClientLogic instance;

    // Start is called before the first frame update
    void Start()
    {



    }

    #region Late join

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManagr_ConnectionApprovalCallBack;
        NetworkManager.StartHost();
    }

    private void NetworkManagr_ConnectionApprovalCallBack(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != "CharacterSelect")
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game Already Started ";
            return;
        }
        if(NetworkManager.ConnectedClientsList.Count >= MAX_PLAYER_COUNT)
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

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.StartClient();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientID)
    {
        onFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }
    #endregion



}
