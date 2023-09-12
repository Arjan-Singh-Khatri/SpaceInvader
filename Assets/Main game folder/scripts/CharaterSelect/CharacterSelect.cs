using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{

    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameobject;
    private void Start()
    {
        MultiplayerManager.instance.onPlayerDataListChange += MultiPlayerManager_onPlayerDataListChange;
        ReadyLogic.instance.onPlayerReadyChange += ReadyLogic_onPlayerReadyChange;
        UpdatePlayer();
    }

    private void ReadyLogic_onPlayerReadyChange(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void MultiPlayerManager_onPlayerDataListChange(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    void UpdatePlayer()
    {
        if(MultiplayerManager.instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
            PlayerData playerData = MultiplayerManager.instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyGameobject.SetActive(ReadyLogic.instance.IsPlayerReady(playerData.clientId));
        }else
        {
            Hide();
        }
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
