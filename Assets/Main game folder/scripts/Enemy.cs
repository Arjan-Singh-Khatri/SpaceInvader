using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{


    protected void Movement(ref GameObject player , float speed)
    {
        transform.position = Vector2.MoveTowards(transform.position,player.transform.position, Time.deltaTime * speed) ;
        //var angle = Mathf.Atan2(-player.transform.position.y, -player.transform.position.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector3 directionToPlayer = player.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle - 180);

        // Smoothly rotate the enemy towards the player
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 90 * Time.deltaTime);
    }

    protected void Shooting(ref Transform shootingPoint, ref GameObject bulletPrefab, ref GameObject player)
    {
        //var angle = Mathf.Atan2(player.transform.position.y, player.transform.position.x) * Mathf.Rad2Deg;
        GameObject instantiatedGameobject = Instantiate(bulletPrefab, shootingPoint.position, Quaternion.identity,shootingPoint.transform);
        //instantiatedGameobject.transform.position += -shootingPoint.right;
    }


    protected void TakeDamage(int Damage, ref int health)
    {
        health -= Damage;
    }


}
