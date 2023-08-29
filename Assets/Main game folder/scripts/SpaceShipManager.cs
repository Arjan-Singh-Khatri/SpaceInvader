using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class SpaceShipManager : NetworkBehaviour
{
    [Header("Basic Components")]
    private int playerHealth = 5;
    private float horizontal;
    private float vertical;

    [Header("Shooting Components")]
    [SerializeField] private GameObject missile;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform fireingPoint;
    int bulletCount = 25;
    int missleCount = 6;

    int damageTakenFromMissile = 20;
    int damageTakenFromBullet = 9;
    int damageTakeFromCollision = 5;
    float angleForRotation;

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        angleForRotation = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;
        ShootingManagerFunction();
 

    }
  
    void HealthManagerFunction()
    {
        playerHealth += 20;
        if (playerHealth > 100)
        {
            playerHealth = 100;
        }
    }
    void TakeDamage(int Damage)
    {
        playerHealth -= Damage;
        if(playerHealth < 0)
        {
            playerHealth = 0;
            if(IsOwner && GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
            {
                CallDespwan();
                Events.gameOver();
            }

            else if(GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
            {
                Events.gameOver();
                Destroy(gameObject);
            }

        }
        Events.healthCount(playerHealth);
    }
    void CallDespwan()
    {
        DesapwnServerRpc((uint)OwnerClientId);
    }
    [ServerRpc(RequireOwnership =false)]
    void DesapwnServerRpc(uint clientId)
    {
        if(clientId == (uint)NetworkManager.ServerClientId)
        {
            NetworkManager.Singleton.Shutdown();
            //UI - Game Over UI GAME OVER ! event


        }
        gameObject.GetComponent<NetworkObject>().Despawn();
    }

    void ShootingManagerFunction()
    {

        if (Input.GetKeyDown(KeyCode.Z) && bulletCount > 0)
        {
            if(IsOwner)
                BulletInstantiate();
        }
            
        else if (Input.GetKeyDown(KeyCode.X) && missleCount > 0)
        {
            if (IsOwner)
                MissileInstantiate(); 
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
            Events.ammoCount(bulletCount, missleCount);
        }else
        {

            BulletSpawnServerRpc();
            bulletCount -= 1;
            if (bulletCount < 0)
                bulletCount = 0;
            Events.ammoCount(bulletCount, missleCount);
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
            Events.ammoCount(bulletCount, missleCount);
        }
        else
        {
            missleCount -= 1;
            if (missleCount < 0)
                missleCount = 0;
            MissileSpawnServerRpc();
            Events.ammoCount(bulletCount, missleCount);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // MAKE ANOTHER TAG AND OTHER PREFAB FOR COLLECTABLE
        if (collision.gameObject.CompareTag("DropBullet"))
        {
            bulletCount += 10;
            Events.ammoCount(bulletCount, missleCount);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("DropMissile"))
        {
            missleCount += 5;
            Events.ammoCount(bulletCount, missleCount);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Health"))
        {
            HealthManagerFunction();
            Destroy(collision.gameObject);
            
        }
        if (collision.gameObject.CompareTag("GunEnemy"))
        {
            TakeDamage(damageTakenFromBullet);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("MissileEnemy"))
        {
            TakeDamage(damageTakenFromMissile);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(damageTakeFromCollision);
             
        }

        if (collision.gameObject.CompareTag("BossBullet"))
        {
            TakeDamage(40);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Boss"))
        {
            TakeDamage(10);
        }

        
    }
}
