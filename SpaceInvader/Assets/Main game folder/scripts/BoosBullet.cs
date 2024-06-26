using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class BoosBullet : NetworkBehaviour
{
    GameObject playerToTrack;
    private readonly float speed = 2.8f;
    [SerializeField]private float bulletLifeTime = 3f;
    // Start is called before the first frame update
    void Start()
    {
        if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
            CallRpcForPlayerTrack();
        else
            playerToTrack = GameObject.FindGameObjectWithTag("Player");

        Events.instance.GameWonToggleEvent += Destroy;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MovementFunction();
        bulletLifeTime -= Time.fixedDeltaTime;
        if (bulletLifeTime <= 0)
            DestroyBullet();

    }

    private void DestroyBullet()
    {
        if(GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
        {
            gameObject.GetComponent<NetworkObject>().Despawn(true);
        }else
        {
            Destroy(gameObject);
        }
    }

    void MovementFunction()
    {
        if (playerToTrack == null)
        {
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, playerToTrack.transform.position,Time.deltaTime * speed);
        Vector3 directionToPlayer = playerToTrack.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle - 180);

        // Smoothly rotate the enemy towards the player
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 90 * Time.deltaTime);
    }

    void AssignMultiplayerPlayerObject()
    {
        var randomClientID = Random.Range(0, NetworkManager.Singleton.ConnectedClientsList.Count);
        playerToTrack =NetworkManager.Singleton.ConnectedClients[(ulong)randomClientID].PlayerObject.gameObject;
    }

    [ServerRpc(RequireOwnership = false)]
    void CallServerToGetClientListServerRpc()
    {
        GetClientListClientRpc();
    }

    [ClientRpc]
    void GetClientListClientRpc()
    {
        AssignMultiplayerPlayerObject();
    }

    void CallRpcForPlayerTrack()
    {
        CallServerToGetClientListServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void DespawnServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn(true);
    }

    private void Destroy()
    {
        if(GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
        {
            DespawnServerRpc();
        }else if (GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Donot Destroy if enemy bullet hit enemy and player bullet hit player
        if (collision.gameObject.CompareTag("Player") )
            Destroy();
        else if (collision.gameObject.CompareTag("Player"))
            Destroy();

    }
}