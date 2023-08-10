using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyS : Enemy
{
    private GameObject player;
    private float speed = 2f;
    private int healthShip = 30;

    private int damageFromMissile = 25;
    private int damageFromBullet = 10;
    private int damageFromCollision = 5;

    [SerializeField]private Transform shootingPoint;

    [SerializeField]GameObject bulletprefab;
    [SerializeField] private float shootTimer = 3f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer -= Time.deltaTime;
        Movement(ref player , speed);
        if(shootTimer <=0 )
        {
            Shooting(ref shootingPoint,ref bulletprefab,ref player);
            shootTimer = 3f;
        }
            
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerMissile"))
            TakeDamage(damageFromMissile, ref healthShip);
        if (collision.gameObject.CompareTag("PlayerBullet"))
            TakeDamage(damageFromBullet, ref healthShip);
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            TakeDamage(damageFromCollision,ref healthShip);
    }
}
