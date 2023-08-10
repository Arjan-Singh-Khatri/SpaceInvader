using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    private Vector3 directionVector = new(1, 0,0);
    // Start is called before the first frame update
    void Start()
    {
        // Diff speed for missile and bullet
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }


}
