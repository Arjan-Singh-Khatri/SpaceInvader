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
    [SerializeField] SpaceShipManager spaceShipManager;
    void Start()
    {
        screenBounds = MainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, MainCamera.transform.position.z));
        objectWidth = transform.GetComponent<SpriteRenderer>().bounds.extents.x; //extents = size of width / 2
        objectHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y; //extents = size of height / 2
    }

    public Camera MainCamera;
    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;
    Quaternion previousRotation;
    // Update is called once per frame
    void Update()
    {
        if(spaceShipManager.playerHealth<=0) return;    
        if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
        {
            if (!IsOwner) return;
            Movement();
            PlayerRotation();
            
        }
        if (GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
        {
            #region Movement

            Movement();
            PlayerRotation();
            //Boundary Check
            
            #endregion

        }
    }
    void LateUpdate()
    {
        Vector3 viewPos = transform.position;
        viewPos.x = Mathf.Clamp(viewPos.x, screenBounds.x * -1 + objectWidth, screenBounds.x - objectWidth);
        viewPos.y = Mathf.Clamp(viewPos.y, screenBounds.y * -1 + objectHeight, screenBounds.y - objectHeight);
        transform.position = viewPos;
    }
    void Movement()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        movementVector.x = horizontal;
        movementVector.y = vertical;
        transform.position += Time.deltaTime * speed * movementVector;

    }

    private void PlayerRotation()
    {
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
