using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetcodeUI : NetworkBehaviour
{
    [SerializeField] Button server;
    [SerializeField] Button client;
    [SerializeField] Button host;
    // Start is called before the first frame update
    void Start()
    {
        server.onClick.AddListener(() =>
        {
            NetworkManager.StartServer();
        });

        host.onClick.AddListener(() =>
        {
            NetworkManager.StartHost();
        });

        client.onClick.AddListener(() =>
        {
            NetworkManager.StartClient();
        });
    }

}
