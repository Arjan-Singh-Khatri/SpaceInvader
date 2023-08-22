using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Projectiles : NetworkBehaviour
{
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float missileSpeed = 4f;
    //private Quaternion quaternionForRotation;
    private float destroyTimer = 3f;

    private void Start()
    {
        //quaternionForRotation = this.transform.parent.rotation;
        //this.transform.parent = null;
        //transform.rotation = quaternionForRotation;
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
            transform.position += Time.deltaTime * missileSpeed * -transform.right;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Donot Destroy if enemy bullet hit enemy and player bullet hit player
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy") && GameStateManager.Instance.currentGameMode==GameMode.MultiPlayer)
            DespawnServerRpc();

    }

    [ServerRpc(RequireOwnership =false)]
    void DespawnServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn(true);
    }

}
