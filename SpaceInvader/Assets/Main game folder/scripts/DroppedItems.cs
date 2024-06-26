using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DroppedItems : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
        {
            DespawnServerRpc();

        }else if(GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
        {
            Destroy(gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void DespawnServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn(true);
    }
}
