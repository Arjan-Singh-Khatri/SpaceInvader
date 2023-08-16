using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class SpaceShipMovement : MonoBehaviour
{

    private float horizontal;
    private float vertical;
    private Vector3 movementVector = new Vector3(1,1,0);
    private float speed = 5f;

    float rightXBoundary = 9.75f;
    float leftXBoundary = -9.75f;
    float upYBoundary= 4.18f;
    float downYBoundary = -4.18f;
    float angleForRotation;
    // Update is called once per frame
    void Update()
    {
        // Player Ship rotate According to Movement based on Input 

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        movementVector.x = horizontal;
        movementVector.y = vertical;
        transform.position += Time.deltaTime * speed * movementVector;

        angleForRotation = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angleForRotation, Vector3.forward);

        BoundaryChecks();
            
    }

    void BoundaryChecks()
    {
        if (transform.position.x < leftXBoundary)
            transform.position = new Vector2(leftXBoundary, transform.position.y);
        else if (transform.position.x > rightXBoundary)
            transform.position = new Vector2(rightXBoundary, transform.position.y);
        if (transform.position.y < downYBoundary)
            transform.position = new Vector2(transform.position.x, downYBoundary);
        else if (transform.position.y > upYBoundary)
            transform.position = new Vector2(transform.position.x, upYBoundary);
    }


}
