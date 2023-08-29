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
    GameObject playerForMultiplayer;
    private bool isFirst = true;

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
            shootTimer -=Time.deltaTime;
            Movement(player, speed);
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
            if (playerForMultiplayer == null)
            {
                Debug.Log("A");
                CallServerToGetClientListServerRpc();
            }
            Movement(playerForMultiplayer, speed);
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
        GetClientListClientRpc();
    }

    [ClientRpc]
    void GetClientListClientRpc()
    {
        AssignMultiplayerPlayerObject();
    }

    void AssignMultiplayerPlayerObject()
    {
        var randomClientID = Random.Range(0, NetworkManager.Singleton.ConnectedClientsList.Count);
        if (isFirst)
        {
            playerForMultiplayer = NetworkManager.Singleton.ConnectedClients[(ulong)randomClientID].PlayerObject.gameObject;
            isFirst = false;
        }

        else
        {
            while (NetworkManager.Singleton.ConnectedClients[(ulong)randomClientID].PlayerObject == null)
            {
                randomClientID = Random.Range(0, NetworkManager.Singleton.ConnectedClientsList.Count);
            }

            playerForMultiplayer = NetworkManager.Singleton.ConnectedClients[(ulong)randomClientID].PlayerObject.gameObject;
        }

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
