using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class SpaceShipManager : MonoBehaviour
{
    [Header("Basic Components")]
    private Animator anim;
    private int playerHealth = 50;
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
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
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
            Events.gameOver();
            Destroy(gameObject);
        }
        Events.healthCount(playerHealth);
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
        var angleForRotation = Mathf.Atan2(vertical,horizontal) * Mathf.Rad2Deg;
        Instantiate(bullet, fireingPoint.position, Quaternion.AngleAxis(angleForRotation,Vector3.forward),fireingPoint.transform);
        bulletCount -= 1;
        Events.ammoCount(bulletCount,missleCount);
    }

    void MissileInstantiate()
    {
        var angleForRotation = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;
        Instantiate(missile, fireingPoint.position, Quaternion.AngleAxis(angleForRotation, Vector3.forward), fireingPoint.transform);
        missleCount -= 1;
        Events.ammoCount(bulletCount, missleCount);
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
