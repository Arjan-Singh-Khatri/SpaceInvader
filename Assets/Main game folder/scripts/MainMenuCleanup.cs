using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanup : MonoBehaviour
{

    private void Awake()
    {
        if(NetworkManager.Singleton!= null)
        {
            Destroy(NetworkManager.Singleton);
        }
        if(MultiplayerManager.instance != null)
        {
            Destroy(MultiplayerManager.instance);
        }
        if(Lobby.instance != null)
        {
            Destroy(Lobby.instance);
        }
    }

}
