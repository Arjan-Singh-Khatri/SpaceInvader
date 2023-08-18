using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

public class SpaceShipMovement : NetworkBehaviour
{
    
    
    private float horizontal;
    private float vertical;

    private Vector3 movementVector = new(1,1,0);
    private readonly float speed = 5f;
    readonly float rightXBoundary = 9.75f;
    readonly float leftXBoundary = -9.75f;
    readonly float upYBoundary= 4.18f;
    readonly float downYBoundary = -4.18f;
    float angleForRotation;

    Quaternion previousRotation;
    // Update is called once per frame
    void Update()
    {
        if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
        {
            if (!IsOwner) return;
            Movement();
            BoundaryChecks();
        }
        if (GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
        {
            #region Movement

            Movement();
            //Boundary Check
            BoundaryChecks();
            #endregion

        }
        Debug.Log(Time.timeScale);
    }
    void Movement()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        movementVector.x = horizontal;
        movementVector.y = vertical;
        Debug.Log(horizontal);
        transform.position += Time.deltaTime * speed * movementVector;
        angleForRotation = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;
        if (movementVector.magnitude < 0.1f)
            transform.rotation = previousRotation;
        else
            transform.rotation = Quaternion.AngleAxis(angleForRotation, Vector3.forward);
        previousRotation = transform.rotation;
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
