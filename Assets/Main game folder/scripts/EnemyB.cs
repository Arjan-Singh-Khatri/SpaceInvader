using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyB : Enemy
{
    [Header("General")]
    private GameObject player;
    private readonly float speed = 2f;
    private int healthShip = 40;

    private readonly int damageFromMissile = 25;
    private readonly int damageFromBullet = 10;
    private float shootTimer = 2f;

    [Header("Shooting")]
    [SerializeField]Transform shootingPoint;
    [SerializeField] GameObject bulletPrefab;

    [Header("Animation")]
    private Animator animator;
    private float destroyAnimationTIme;
    private AnimationClip[] animationClips;
    private bool isDestroyed = false;


    [Header("Drop")]
    [SerializeField]private GameObject[] listOfDropItems;

    [Header("Multiplayer")]
    private GameObject playerForTracking;
    private bool isFirst = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animationClips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in animationClips)
        {
            if (clip.name == "Explosion")
            {
                destroyAnimationTIme = clip.length;
            }
        }
        if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
            CallServerToGetClientListServerRpc();
        else
            playerForTracking = GameObject.FindGameObjectWithTag("Player");

    }
    // Update is called once per frame
    void Update()
    {
        if (isDestroyed) return;
        if (GameStateManager.Instance.currentGameState == GameState.gamePaused) return;
        if (GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
        {
            shootTimer -= Time.deltaTime;
            Movement(playerForTracking, speed);
            if (shootTimer <= 0)
            {
                Shooting(ref shootingPoint, bulletPrefab);
                shootTimer = 2f;
            }
        }
        else
        {
            
            if (!IsOwner) return;
            shootTimer -= Time.deltaTime;
            if (playerForTracking == null )
            {
                
                CallServerToGetClientListServerRpc();
            }

            Movement(playerForTracking, speed);
            if (shootTimer <= 0)
            {

                CallToShootServerRpc();
                shootTimer = 2f;
            }
        }
        
        ShipDestroy();
    }
    [ServerRpc(RequireOwnership =false)]
    void CallServerToGetClientListServerRpc()
    {
        GetClientListClientRpc();
    }

    [ClientRpc]
    void GetClientListClientRpc()
    {
        AssignMultiplayerPlayerObject();
    }

    void AssignMultiplayerPlayerObject()
    {
        var randomClientID = Random.Range(0,NetworkManager.Singleton.ConnectedClientsList.Count);
        if (isFirst)
        {
            playerForTracking = NetworkManager.Singleton.ConnectedClients[(ulong)randomClientID].PlayerObject.gameObject;
            isFirst = false;
        }

        else
        {
            //while (NetworkManager.Singleton.ConnectedClients[(ulong)randomClientID].PlayerObject == null)
            //{
            //    randomClientID =Random.Range(0, NetworkManager.Singleton.ConnectedClientsList.Count);
            //}
            foreach(var client in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (NetworkManager.Singleton.ConnectedClients[client].PlayerObject == null)
                    continue;
                else
                    playerForTracking = NetworkManager.Singleton.ConnectedClients[(ulong)randomClientID].PlayerObject.gameObject;
            }

            
        }

    }

    [ServerRpc]
    void CallToShootServerRpc()
    {
        if (IsServer)
        {
            Shooting(ref shootingPoint, bulletPrefab);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerMissile") && !isDestroyed)
        {

            TakeDamage(damageFromMissile, ref healthShip);
        }
            
        if (collision.gameObject.CompareTag("PlayerBullet") && !isDestroyed)
        {

            TakeDamage(damageFromBullet, ref healthShip);
        }


    }

    private void ShipDestroy()
    {
        if (!(healthShip <= 0)) return;
        Invoke(nameof(DestroyShip), destroyAnimationTIme);
        animator.SetBool("destroy", true);
        healthShip = 50;
        Events.instance.playExplodeSound();
        Drop();
        isDestroyed = true;
    }
    private void Drop()
    {
        int randomnumberforitemdrop = Random.Range(0, 9);
        if (randomnumberforitemdrop > -1)
        {
            if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
            {
                GameObject drop = Instantiate(listOfDropItems[Random.Range(0, listOfDropItems.Length)], transform.position, Quaternion.identity);
                drop.GetComponent<NetworkObject>().Spawn();
            }
            else
            {
                GameObject drop = Instantiate(listOfDropItems[Random.Range(0, listOfDropItems.Length)], transform.position, Quaternion.identity);
            }
        }

    }
    [ServerRpc]
    void DespawnServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }

    protected void DestroyShip()
    {
        animator.SetBool("destroy", false);
        if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
            DespawnServerRpc();
        Destroy(gameObject);
    }


}
