using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Boss : NetworkBehaviour
{

    [SerializeField] GameObject forceFieldColliderObject;
    [SerializeField] ParticleSystem forceField;
    private ParticleSystem instantiatedParticleSystem;
    [SerializeField]private GameObject bulletPrefab;
    [SerializeField] Transform shootingPoint;


    private float Health = 150f;
    private float damageTaken = 0;
    private bool forceFieldOn = false;
    private float forceFieldTimer = 10f;
    private float shootingTimer = 4f;



    // Start is called before the first frame update
    void Start()
    {
        SpaceShipEntryMovement();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        
        if(Health >=0 ) { BossOver(); }
        MovementOfEnemyShip();
        if(damageTaken >= 40)
        {
            ShootBullet();
            damageTaken = 0f;
            ForceFieldOn();
        }
        if (!forceFieldOn) return;
        forceFieldTimer -= Time.fixedDeltaTime;
        if (forceFieldTimer <= 0)
        {
            forceFieldOn = false;
            forceFieldTimer = 10f;
            ForceFieldOff();
        }
    }

    #region Multiplayer Stuff

    [ServerRpc(RequireOwnership =false)]
    void Death()
    {
        gameObject.GetComponent<NetworkObject>().Despawn(true);
    }

    #endregion


    #region Functions For Boss 
    void MovementOfEnemyShip()
    {

    }

    void SpaceShipEntryMovement()
    {

    }

    void ShootBullet()
    {
        shootingTimer -= Time.fixedDeltaTime;
        if(shootingTimer <= 0) 
        {
            SpawnBullet();
        }
    }

    void SpawnBullet()
    {
        if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
        {
            shootingTimer = 2f;
            GameObject instantiated = Instantiate(bulletPrefab, shootingPoint.position, Quaternion.identity);
            instantiated.GetComponent<NetworkObject>().Spawn(true);
        }
        else
        {
            shootingTimer = 4f;
            GameObject instantiated = Instantiate(bulletPrefab, shootingPoint.position, Quaternion.identity);
        }
    }


    void ForceFieldOff()
    {
        instantiatedParticleSystem.Stop();
        forceFieldColliderObject.SetActive(false);
    }

    void ForceFieldOn()
    {
        instantiatedParticleSystem.Play();
        forceFieldColliderObject.SetActive(true);
    }

    void BossOver()
    {
        if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
            Death();
        else if(GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
        {
            Destroy(gameObject);
        }

    }

    #endregion


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet"))
        {
            damageTaken += 10f;
            Health -= 10f;
        }
        if (collision.CompareTag("PlayerMissile"))
        {
            damageTaken += 20f;
            Health -= 20f;
        }
    }

   

}
