using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float missileSpeed = 4f;
    private Quaternion quaternionForRotation;
    private float destroyTimer = 3f;

    private void Start()
    {
        quaternionForRotation = this.transform.parent.rotation;
        this.transform.parent = null;
        transform.rotation = quaternionForRotation;
    }
    // Update is called once per frame
    void Update()
    {
        destroyTimer -= Time.deltaTime;
        if (destroyTimer <= 0) Destroy(gameObject);
        if (gameObject.CompareTag("GunEnemy"))
            transform.position += Time.deltaTime * bulletSpeed * -transform.right;
        else if (gameObject.CompareTag("PlayerBullet"))
            transform.position += Time.deltaTime * bulletSpeed * transform.right;
        else if (gameObject.CompareTag("PlayerMissile"))
            transform.position += Time.deltaTime * missileSpeed * transform.right;
            
        else if (gameObject.CompareTag("MissileEnemy"))
            transform.position += Time.deltaTime * missileSpeed *-transform.right;

    }

    

}
