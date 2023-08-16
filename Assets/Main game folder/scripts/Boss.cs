using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    bool isShooting= true;
    [SerializeField] ParticleSystem forceField;
    private ParticleSystem instantiatedParticleSystem;
    [SerializeField]private GameObject bulletPrefab;
    [SerializeField] Transform shootingPoint;

    private float shootingTimer = 5f;
    private float forceFieldOnTimer = 10f;
    private int bulletCount = 0;

    private List<GameObject> list = new();

    // Start is called before the first frame update
    void Start()
    {
        instantiatedParticleSystem = Instantiate(forceField, transform.localPosition, Quaternion.identity);
        instantiatedParticleSystem.Stop();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (isShooting)
        {
            Debug.Log("shooting");
            shootingTimer -= Time.fixedDeltaTime;
            if (shootingTimer <= 0)
            {
                ShootPlayer();
                shootingTimer = 5f;
                bulletCount++;
                if (bulletCount == 3)
                {
                    isShooting = false;
                    ForceFieldOn();
                    bulletCount = 0;
                    list.Clear();
                }
            }

        } else if (!isShooting)
        {
            Debug.Log("forcefield");
            forceFieldOnTimer -=Time.fixedDeltaTime;
            if(forceFieldOnTimer<=0) 
            {
                forceFieldOnTimer = 10f;
                ForceFieldOff();
                isShooting = true;
            }
            
        }
    }

    void ShootPlayer()
    {
        GameObject ins = Instantiate(bulletPrefab, shootingPoint.position, Quaternion.identity);
        list.Add(ins);
        Debug.Log("bullet instantiated !");
    }

    void ForceFieldOff()
    {
        instantiatedParticleSystem.Stop();
    }

    void ForceFieldOn()
    {
        instantiatedParticleSystem.Play();
    }

    void MovementOfEnemyShip()
    {
           
    }
}
