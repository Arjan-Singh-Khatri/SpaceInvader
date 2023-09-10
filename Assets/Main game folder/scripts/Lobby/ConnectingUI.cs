using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{


    private void Start()
    {
        HostAndClientLogic.instance.onTryingToJoinGame += HostAndClient_onTryingToJoinGame;
        HostAndClientLogic.instance.onFailedToJoinGame += HostAndClient_onFailedToJoinGame;

        Hide();
    }

    private void HostAndClient_onFailedToJoinGame(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void HostAndClient_onTryingToJoinGame(object sender, System.EventArgs e)
    {
        Show();
    }

    void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
    {
        gameObject.SetActive(false);    
    }
}
