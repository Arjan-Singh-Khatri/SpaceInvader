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
    [SerializeField]private float bulletLifeTime = 5f;
    // Start is called before the first frame update
    void Start()
    {
        if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
            CallRpcForPlayerTrack();
        else
            playerToTrack = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MovementFunction();
        bulletLifeTime -= Time.fixedDeltaTime;
        if (bulletLifeTime <= 0)
            Destroy(gameObject);
        if (GameStateManager.Instance.currentGameMode != GameMode.MultiPlayer) return;

    }

    void MovementFunction()
    {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Donot Destroy if enemy bullet hit enemy and player bullet hit player
        if (collision.gameObject.CompareTag("Player") && GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
            DespawnServerRpc();
        else if (collision.gameObject.CompareTag("Player") && GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
            Destroy(gameObject);

    }
}