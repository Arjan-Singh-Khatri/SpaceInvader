using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipManager : MonoBehaviour
{
    [Header("Basic Components")]
    private Animator anim;
    private int playerHealth = 50;

    [Header("Shooting Components")]
    [SerializeField] private GameObject missile;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform fireingPoint;
    float bulletCount = 20f;
    float missleCount = 6f;

    int damageTakenFromMissile = 25;
    int damageTakenFromBullet = 10;
    int damageTakeFromCollision = 5;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ShootingManagerFunction();
    }

    void ShipAnimation()
    {

    }
    
    void HealthManagerFunction()
    {
        playerHealth += 20;
        if (playerHealth > 50)
        {
            //Incere the number of lives 
            // if number of lives is max do nothing 
        }
    }
    void TakeDamage(int Damage)
    {
        playerHealth -= Damage;
        if(playerHealth < 0)
        {
            //Remove One life 
            // If life is finished then GameOver 
        }
    }

    void ShootingManagerFunction()
    {
        if (Input.GetKeyDown(KeyCode.Z) && bulletCount > 0)
            BulletInstantiate();
        else if (Input.GetKeyDown(KeyCode.X) && missleCount > 0)
            MissileInstantiate();
    }

    void BulletInstantiate()
    {
        GameObject instantiatedBullet = Instantiate(bullet, fireingPoint.position, Quaternion.identity);
        bulletCount -= 1;
    }

    void MissileInstantiate()
    {
        GameObject instantiateMissile = Instantiate(missile, fireingPoint.position, Quaternion.identity);
        missleCount -= 1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerMissile"))
            missleCount += 3;
        if(collision.gameObject.CompareTag("PlayerBullet"))
            bulletCount += 10;
        if (collision.gameObject.CompareTag("Health"))
        {
            HealthManagerFunction();
        }
        if (collision.gameObject.CompareTag("GunEnemy"))
            TakeDamage(damageTakenFromBullet);
        if (collision.gameObject.CompareTag("MissileEnemy"))
            TakeDamage(damageTakenFromMissile);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            TakeDamage(damageTakeFromCollision);
    }

}
