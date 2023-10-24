using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : NetworkBehaviour
{

    [SerializeField] GameObject shield;
    [SerializeField] ParticleSystem forceField;
    
    [SerializeField]private GameObject bulletPrefab;
    [SerializeField] Transform shootingPoint;


    private float Health = 150f;
    private float damageTaken = 0;
    public bool forceFieldOn = false;
    private float forceFieldTimer = 10f;
    private float shootingTimer = 4f;



    // Start is called before the first frame update
    void Start()
    {  
        SpaceShipEntryMovement();
        ForceFieldOff();
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        
        if(Health <=0 ) { BossOver(); }
        MovementOfEnemyShip();
        if(damageTaken >= 40)
        {
            
            ForceFieldOn();
            
        }
        else
        {
            ShootBullet();
        }
        if (!forceFieldOn) return;
        forceFieldTimer -= Time.fixedDeltaTime;
        Debug.Log(forceFieldTimer);
        if (forceFieldTimer <= 0)
        {
            
            forceFieldTimer = 10f;
            ForceFieldOff();
            damageTaken = 0;
        }
        
    }

    #region Multiplayer Stuff

    [ServerRpc(RequireOwnership =false)]
    void DeathServerRpc()
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
        forceFieldOn = false;
        forceField.Stop();
        shield.SetActive(false);
        
    }

    void ForceFieldOn()
    {
        forceFieldOn = true;
        shield.SetActive(true);
        forceField.Play();
        
    }

    void BossOver()
    {
        Events.instance.GameWonToggleEvent();
        if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
        {
            DeathServerRpc();
            // Game Won Rpcs For All PLayers 
        }
        else if (GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
        {
            Destroy(gameObject);
            // Game Won Screen For Player 
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
