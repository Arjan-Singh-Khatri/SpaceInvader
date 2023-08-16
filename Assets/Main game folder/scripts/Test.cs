using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject boss;
    float shootingTimer = 4f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        shootingTimer -= Time.fixedDeltaTime;
        if (shootingTimer < 0)
        {
            Instantiate(boss, transform.position, Quaternion.identity);
            shootingTimer = 1000000f;
        }

    }
}
