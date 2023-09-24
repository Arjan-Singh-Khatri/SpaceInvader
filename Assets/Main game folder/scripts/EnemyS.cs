using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyS : Enemy
{
    private GameObject player;
    private readonly float speed = 1f;
    private int healthShip = 30;

    private readonly int damageFromMissile = 25;
    private readonly int damageFromBullet = 10;
    private float shootTimer = 3f;
    private bool isDestroyed = false;
    [SerializeField]private Transform shootingPoint;
    [SerializeField]GameObject bulletPrefab;
    [SerializeField] GameObject[] listOfDropItems;
    private Animator animator;
    private float destroyAnimationTIme;
    private AnimationClip []animationClips;

    [Header("Multiplayer")]
    GameObject playerForTracking;
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
        if(GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
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
            shootTimer -=Time.deltaTime;
            Movement(playerForTracking, speed);
            if (shootTimer <= 0)
            {
                Shooting(ref shootingPoint, bulletPrefab);
                shootTimer = 3f;
            }
        }
        else
        {
            
            if (!IsOwner) return;
            shootTimer -= Time.deltaTime;
            if (playerForTracking == null)
            {
                Debug.Log("A");
                CallServerToGetClientListServerRpc();
            }
            Movement(playerForTracking, speed);
            if (shootTimer <= 0)
            {
                CallToShootServerRpc();
                shootTimer = 3f;
            }
        }

        ShipDestroy();
    }
    [ServerRpc(RequireOwnership = false)]
    void CallServerToGetClientListServerRpc()
    {

        var randomClientID = Random.Range(0, NetworkManager.Singleton.ConnectedClientsList.Count);
        if (NetworkManager.Singleton.ConnectedClients[(ulong)randomClientID].PlayerObject == null)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (NetworkManager.Singleton.ConnectedClients[client].PlayerObject != null)
                    GetClientListClientRpc(client);
            }
        }
        else
        {
            GetClientListClientRpc((ulong)randomClientID);
        }
    }

    [ClientRpc]
    void GetClientListClientRpc(ulong clienID)
    {
        AssignMultiplayerPlayerObject(clienID);
    }

    void AssignMultiplayerPlayerObject(ulong clienID)
    {
        playerForTracking = NetworkManager.Singleton.ConnectedClients[clienID].PlayerObject.gameObject;
    }

    [ServerRpc(RequireOwnership = false)]
    void CallToShootServerRpc()
    {
        if (IsServer)
            Shooting(ref shootingPoint, bulletPrefab);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerMissile"))
        {
            TakeDamage(damageFromMissile, ref healthShip);
        }
            
        if (collision.gameObject.CompareTag("PlayerBullet"))
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
        int randomNumberForItemDrop = Random.Range(0, 9);
        if (randomNumberForItemDrop > 5)
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
    [ServerRpc(RequireOwnership = false)]
    void DespawnServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn(true);
    }

    protected void DestroyShip()
    {
        animator.SetBool("destroy", false);
        if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
            DespawnServerRpc();
        Destroy(gameObject);
    }


}
