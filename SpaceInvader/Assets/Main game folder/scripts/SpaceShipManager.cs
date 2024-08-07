using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SpaceShipManager : NetworkBehaviour
{
    [Header("Basic Components")]
    public int playerHealth = 75;
    private float horizontal;
    private float vertical;

    [Header("Shooting Components")]
    [SerializeField] private GameObject missile;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform fireingPoint;
    private Animator animator;
    int bulletCount = 25;
    int missleCount = 6;

    int damageTakenFromMissile = 15;
    int damageTakenFromBullet = 9;
    int damageTakeFromCollision = 5;
    float angleForRotation;

    [SerializeField] PlayerVisual playerVisual;

    private void Start()
    {
        animator = GetComponent<Animator>();    
        
        if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
        {
            PlayerData playerData = MultiplayerManager.instance.GetPlayerDataFromClientId(OwnerClientId);
            playerVisual.SetPlayerColor(MultiplayerManager.instance.GetPlayerColorForPlayer(playerData.colorId));
        }

    }

    
    void Update()
    {
        if(playerHealth<=0) { return; }
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        angleForRotation = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;
        ShootingManagerFunction();
 
    }
  
    void HealthManagerFunction()
    {
        if(GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
        {
            if (!IsOwner) return;
            playerHealth += 20;
            if (playerHealth > 100)
            {
                playerHealth = 100;
            }

        }
        else
        {
            playerHealth += 20;
            if (playerHealth > 100)
            {
                playerHealth = 100;
            }
        }

    }
    void TakeDamage(int Damage)
    {
        playerHealth -= Damage;
        Events.instance.healthCount(playerHealth);
        if (playerHealth<=0)
        {
            animator.SetTrigger("Destroy");
        }
    }

    private void SpaceShipDestroy()
    {
        if(IsOwner && GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
        {
            Events.instance.playerDeathUI();
            Events.instance.playerDeathListAdd();
            CallDespwan();
        }
        else if(GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
        {
            Destroy(gameObject);
            Events.instance.gameOver();
        }
    }
 
    void CallDespwan()
    {
        DesapwnServerRpc((uint)OwnerClientId);
    }
    [ServerRpc(RequireOwnership =false)]
    void DesapwnServerRpc(uint clientId)
    {
        
        gameObject.GetComponent<NetworkObject>().Despawn(true);
    }

    void ShootingManagerFunction()
    {
        if(GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Z) && bulletCount > 0)
            {
                if (IsOwner)
                {
                    BulletInstantiate();
                }
            }

            else if (Input.GetKeyDown(KeyCode.X) && missleCount > 0)
            {
                if (IsOwner)
                    MissileInstantiate();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Z) && bulletCount > 0)
            {
                BulletInstantiate();
            }

            else if (Input.GetKeyDown(KeyCode.X) && missleCount > 0)
            {
                MissileInstantiate();
            }
        }



    }

    void BulletInstantiate()
    {

        if (GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
        {
            Instantiate(bullet, fireingPoint.position, Quaternion.AngleAxis(angleForRotation, Vector3.forward));
            bulletCount -= 1;
            if(bulletCount < 0)
            {
                bulletCount = 0;
            }
            Events.instance.ammoCount(bulletCount, missleCount);
        }else if(GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer && IsOwner)
        {

            BulletSpawnServerRpc();
            bulletCount -= 1;
            if (bulletCount < 0)
                bulletCount = 0;
            Events.instance.ammoCount(bulletCount, missleCount);
        }

    }
    [ServerRpc(RequireOwnership =false)]
    void BulletSpawnServerRpc()
    {
        GameObject instantiatedBullet = Instantiate(bullet, fireingPoint.position, Quaternion.AngleAxis(angleForRotation, Vector3.forward));
        instantiatedBullet.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc(RequireOwnership = false)]
    void MissileSpawnServerRpc()
    {
        GameObject instantiatedBullet = Instantiate(missile, fireingPoint.position, Quaternion.AngleAxis(angleForRotation, Vector3.forward));
        instantiatedBullet.GetComponent<NetworkObject>().Spawn();
    }


    void MissileInstantiate()
    {
        if (GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
        {
            Instantiate(missile, fireingPoint.position, Quaternion.AngleAxis(angleForRotation, Vector3.forward));
            missleCount -= 1;
            if(missleCount < 0)
                missleCount = 0;
            Events.instance.ammoCount(bulletCount, missleCount);
        }
        else
        {
            MissileSpawnServerRpc();
            missleCount -= 1;
            if (missleCount < 0)
                missleCount = 0;
            Events.instance.ammoCount(bulletCount, missleCount);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // MAKE ANOTHER TAG AND OTHER PREFAB FOR COLLECTABLE
        if (collision.gameObject.CompareTag("DropBullet"))
        {
            bulletCount += 10;
            Events.instance.ammoCount(bulletCount, missleCount);
            
        }

        if (collision.gameObject.CompareTag("DropMissile"))
        {
            missleCount += 6;
            Events.instance.ammoCount(bulletCount, missleCount);
        }

        if (collision.gameObject.CompareTag("Health"))
        {
            HealthManagerFunction();
            
        }
        if (collision.gameObject.CompareTag("GunEnemy"))
        {
            TakeDamage(damageTakenFromBullet);
        }

        if (collision.gameObject.CompareTag("MissileEnemy"))
        {
            TakeDamage(damageTakenFromMissile);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(damageTakeFromCollision);
        }

        if (collision.gameObject.CompareTag("BossBullet"))
        {
            TakeDamage(40);
        }

        if (collision.gameObject.CompareTag("Boss"))
        {
            TakeDamage(10);
        }

        if (collision.gameObject.CompareTag("Shield"))
        {
            TakeDamage(40);
        }
        
    }


}
