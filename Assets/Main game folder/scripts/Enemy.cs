using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Enemy : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void Movement(ref GameObject player , float speed)
    {
        transform.position = Vector2.MoveTowards(transform.position,player.transform.position, Time.deltaTime * speed) ;
        var angle = Mathf.Atan2(-player.transform.position.y, -player.transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    protected void Shooting(ref Transform shootingPoint, ref GameObject bulletPrefab, ref GameObject player)
    {
        var angle = Mathf.Atan2(player.transform.position.y, player.transform.position.x) * Mathf.Rad2Deg;
        GameObject instantiatedGameobject = Instantiate(bulletPrefab, shootingPoint.position, Quaternion.AngleAxis(angle, Vector3.forward));
        instantiatedGameobject.transform.position += -shootingPoint.right;
    }

    protected void FacingDirection()
    {

    }

    protected void TakeDamage(int Damage, ref int health)
    {
        Debug.Log("DamageTaken");
        health -= Damage;
        if (health <= 0)
            Destroy(gameObject);
    }

    protected void DropItem()
    {

    }
}
