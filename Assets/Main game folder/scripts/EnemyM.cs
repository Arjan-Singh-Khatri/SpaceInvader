using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyM : Enemy
{
    private GameObject player;
    private readonly float speed = .5f;
    private int healthShip = 60;

    private readonly int damageFromMissile = 25;
    private readonly int damageFromBullet = 10;
    private readonly int damageFromCollision = 5;
    private float shootTimer = 4f;

    [SerializeField] private Transform shootingPoint;
    [SerializeField] GameObject missilePrefab;

    private Animator animator;
    private float destroyAnimationTIme;
    private AnimationClip[] animationClips;

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
        shootTimer -= Time.deltaTime;
        Movement(ref player, speed);
        if (shootTimer <= 0)
        {
            Shooting(ref shootingPoint, ref missilePrefab, ref player);
            shootTimer = 4f;
        }
        ShipDestroy();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerMissile"))
        {
            Destroy(collision.gameObject);
            TakeDamage(damageFromMissile, ref healthShip);
        }
            
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            Destroy(collision.gameObject);
            TakeDamage(damageFromBullet, ref healthShip);
        }

    }
    private void ShipDestroy()
    {
        if (!(healthShip <= 0)) return;
        animator.SetBool("destroy", true);
        healthShip = 50;
        Events.playExplodeSound();
        StartCoroutine(Drop());
        Invoke(nameof(DestroyShip), destroyAnimationTIme);

    }
    private IEnumerator Drop()
    {
        yield return new WaitForSeconds(destroyAnimationTIme);
        int randomNumberForItemDrop = Random.Range(0, 9);
        if (randomNumberForItemDrop > 5)
        {
            Instantiate(missilePrefab, transform.position, Quaternion.identity);
        }
    }
    protected void DestroyShip()
    {
        animator.SetBool("destroy", false);
        Destroy(gameObject);
    }


}
