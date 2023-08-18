using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Enemy : NetworkBehaviour
{


    protected void Movement(GameObject player , float speed)
    {
        transform.position = Vector2.MoveTowards(transform.position,player.transform.position, Time.deltaTime * speed) ;

        Vector3 directionToPlayer = player.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle - 180);

        // Smoothly rotate the enemy towards the player
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 90 * Time.deltaTime);
    }

    protected void Shooting(Transform shootingPoint, GameObject bulletPrefab)
    {
        //Vector3 directionToPlayer = player.transform.position - transform.position;
        //float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        //Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle - 180);
        if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer && IsServer)
        {
            GameObject instantiatedGameobject = Instantiate(bulletPrefab, shootingPoint.position, transform.rotation);
            instantiatedGameobject.GetComponent<NetworkObject>().Spawn(true);
        }
        else
        {
            GameObject instantiatedGameobject = Instantiate(bulletPrefab, shootingPoint.position, transform.rotation);
        }

    }

    protected void TakeDamage(int Damage, ref int health)
    {
        health -= Damage;
    }


}
