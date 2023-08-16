using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyB : Enemy
{
    [Header("General")]
    private GameObject player;
    private readonly float speed = 2f;
    private int healthShip = 40;

    private readonly int damageFromMissile = 25;
    private readonly int damageFromBullet = 10;
    private readonly int damageFromCollision = 5;
    private float shootTimer = 2f;

    [Header("Shooting")]
    [SerializeField] private Transform shootingPoint;
    [SerializeField] GameObject bulletPrefab;

    [Header("Animation")]
    private Animator animator;
    private float destroyAnimationTIme;
    private AnimationClip[] animationClips;
    private bool isDestroyed = false;


    [Header("Drop")]
    [SerializeField] GameObject[] listOfDropItems;
    
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
    }
    // Update is called once per frame
    void Update()
    {
        if (isDestroyed) return;
        shootTimer -= Time.deltaTime;
        Movement(ref player, speed);
        if (shootTimer <= 0)
        {
            Shooting(ref shootingPoint, ref bulletPrefab, ref player);
            shootTimer = 2f;
        }

        ShipDestroy();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerMissile") && !isDestroyed)
        {
            Destroy(collision.gameObject);
            TakeDamage(damageFromMissile, ref healthShip);
        }
            
        if (collision.gameObject.CompareTag("PlayerBullet") && !isDestroyed)
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
        int randomnumberforitemdrop = Random.Range(0, 9);
        if (randomnumberforitemdrop > -1)
        {
            Instantiate(listOfDropItems[Random.Range(0, listOfDropItems.Length)], transform.position, Quaternion.identity);
        }
        Debug.Log("Dropped");

    }
    protected void DestroyShip()
    {
        Destroy(gameObject);
    }


}
