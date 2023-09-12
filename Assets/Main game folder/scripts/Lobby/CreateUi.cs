using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateUi : MonoBehaviour
{
    [SerializeField] Button privateLobby;
    [SerializeField] Button publicLobby;
    [SerializeField] Button close;
    [SerializeField] TMP_InputField lobbyName;
    // Start is called before the first frame update
    void Start()
    {
        privateLobby.onClick.AddListener(() =>
        {
            Lobby.instance.CreateLobby(lobbyName.text, true);
        });
        publicLobby.onClick.AddListener(() =>
        {
            Lobby.instance.CreateLobby(lobbyName.text, false);
        });
        close.onClick.AddListener(() =>
        {
            Hide();
        });
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void show()
    {
        gameObject.SetActive(true);
    }
}
