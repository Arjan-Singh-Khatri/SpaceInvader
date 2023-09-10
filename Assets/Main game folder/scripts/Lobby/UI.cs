using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : NetworkBehaviour
{
    [SerializeField] Button startGame;
    [SerializeField] Button joinGame;

    private void Start()
    {
        startGame.onClick.AddListener(() =>
        {
            HostAndClientLogic.instance.StartHost();
            NetworkManager.SceneManager.LoadScene("CharacterSelect", LoadSceneMode.Single);
        });

        joinGame.onClick.AddListener(() =>
        {
            HostAndClientLogic.instance.StartClient();
        });
    }
}
