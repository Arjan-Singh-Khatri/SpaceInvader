using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyM : Enemy
{
    private GameObject player;
    private readonly float speed = .5f;
    private int healthShip = 60;

    private readonly int damageFromMissile = 25;
    private readonly int damageFromBullet = 10;
    private float shootTimer = 4f;

    [SerializeField]private Transform shootingPoint;
    [SerializeField] GameObject missilePrefab;
    private bool isDestroyed = false;

    private Animator animator;
    private float destroyAnimationTIme;
    private AnimationClip[] animationClips;
    [SerializeField] GameObject[] listOfDropItems;

    [Header("Multiplayer")]
    GameObject playerForMultiplayer;
    ulong randomClientID;
    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        animationClips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in animationClips)
        {
            if (clip.name == "Explosion")
            {
                destroyAnimationTIme = clip.length;
            }


        }
        CallServerToGetClientListServerRpc();
    }
    // Update is called once per frame
    void Update()
    {
        if (isDestroyed) return;
        if (GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
        {
            shootTimer -= Time.deltaTime;
            Movement(player, speed);
            if (shootTimer <= 0)
            {
                Shooting(ref shootingPoint, missilePrefab);
                shootTimer = 4f;
            }
        }
        else
        {
            if (!IsOwner) return;
            shootTimer -= Time.deltaTime;
            Movement(playerForMultiplayer, speed);
            if (shootTimer <= 0)
            {
                CallToShootServerRpc();
                shootTimer = 4f;
            }
        }

        ShipDestroy();
    }
    [ServerRpc(RequireOwnership =false)]
    void CallServerToGetClientListServerRpc()
    {
        GetClientListClientRpc((ulong)Random.Range(0, NetworkManager.Singleton.ConnectedClientsIds.Count));
    }

    [ClientRpc]
    void GetClientListClientRpc(ulong randomClientID)
    {
        AssignMultiplayerPlayerObject(randomClientID);
    }

    void AssignMultiplayerPlayerObject(ulong randomClientID)
    {
        randomClientID = (ulong)Random.Range(0, NetworkManager.Singleton.ConnectedClientsIds.Count);

        playerForMultiplayer = NetworkManager.Singleton.ConnectedClients[randomClientID].PlayerObject.gameObject;
    }

    [ServerRpc(RequireOwnership = false)]
    void CallToShootServerRpc()
    {
        if (IsServer)
            Shooting(ref shootingPoint, missilePrefab);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerMissile"))
        {
            Destroy(collision.gameObject);
            TakeDamage(damageFromMissile, ref healthShip);
        }
            
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            Destroy(collision.gameObject);
            TakeDamage(damageFromBullet, ref healthShip);
        }

    }
    private void ShipDestroy()
    {
        if (!(healthShip <= 0)) return;
        Invoke(nameof(DestroyShip), destroyAnimationTIme);
        animator.SetBool("destroy", true);
        healthShip = 50;
        Events.playExplodeSound();
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

