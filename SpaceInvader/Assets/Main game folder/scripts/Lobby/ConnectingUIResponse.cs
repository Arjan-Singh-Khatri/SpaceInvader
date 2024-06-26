using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectingUIResponse : MonoBehaviour
{
    [SerializeField] Button close;
    [SerializeField] TextMeshProUGUI response;

    private void Start()
    {
        MultiplayerManager.instance.onFailedToJoinGame += HostAndClient_onFailedToJoinGame;
        close.onClick.AddListener(() =>
        {
            Hide();
        });
        Hide();
    }

    private void HostAndClient_onFailedToJoinGame(object sender, System.EventArgs e)
    {
        Show();
    }

    void Show()
    {
        gameObject.SetActive(true);
        response.text = NetworkManager.Singleton.DisconnectReason;

        if (response.text == "")
            response.text = "Failed To Connect ";
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }


}
