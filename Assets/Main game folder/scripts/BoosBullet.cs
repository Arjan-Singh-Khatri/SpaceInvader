using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosBullet : MonoBehaviour
{
    GameObject player;
    private readonly float speed = 2.8f;
    [SerializeField]private float bulletLifeTime = 5f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MovementFunction();
        bulletLifeTime -= Time.fixedDeltaTime;
        if (bulletLifeTime <= 0)
            Destroy(gameObject);
    }

    void MovementFunction()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position,Time.deltaTime * speed);
        var angle = Mathf.Atan2(-player.transform.position.y, -player.transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}