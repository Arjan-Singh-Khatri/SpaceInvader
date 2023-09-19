using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Boss : NetworkBehaviour
{
    bool isShooting= true;
    [SerializeField] ParticleSystem forceField;
    private ParticleSystem instantiatedParticleSystem;
    [SerializeField]private GameObject bulletPrefab;
    [SerializeField] Transform shootingPoint;

    private float shootingTimer = 5f;
    private float forceFieldOnTimer = 10f;
    private int bulletCount = 0;


    // Start is called before the first frame update
    void Start()
    {
        // spawn 
        instantiatedParticleSystem = Instantiate(forceField, transform.localPosition, Quaternion.identity);
        instantiatedParticleSystem.Stop();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        // Bullet Spawning 
    }

    #region Multiplayer Stuff


    #endregion


    #region Functions For Boss 
    void MovementOfEnemyShip()
    {

    }

    void ShootPlayer()
    {
        // Spawn in Server
        GameObject instantiated = Instantiate(bulletPrefab, shootingPoint.position, Quaternion.identity);
        
    }

    void ForceFieldOff()
    {
        instantiatedParticleSystem.Stop();
    }

    void ForceFieldOn()
    {
        instantiatedParticleSystem.Play();
    }

    #endregion

   
}
