
using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanup : MonoBehaviour
{

    private void Awake()
    {
        if(NetworkManager.Singleton!= null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
        if(MultiplayerManager.instance != null)
        {
            Destroy(MultiplayerManager.instance.gameObject);
        }
        if(Lobby.instance != null)
        {
            Destroy(Lobby.instance.gameObject);
        }
        
    }

}
