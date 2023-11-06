using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

public class SpaceShipMovement : NetworkBehaviour
{
    
    


    private readonly float speed = 5f;
    float angleForRotation;
    private float objectWidth;
    private float objectHeight;
    public Camera MainCamera;
    private Vector2 screenBounds;
    private Vector3 movementVector = new(1, 1, 0);

    private Vector3 _viewPos;


    //readonly float rightXBoundary = 9.75f;
    //readonly float leftXBoundary = -9.75f;
    //readonly float upYBoundary= 4.18f;
    //readonly float downYBoundary = -4.18f;

    Quaternion previousRotation;
    [SerializeField] SpaceShipManager spaceShipManager;
    void Start()
    {
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();    
        screenBounds = MainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, MainCamera.transform.position.z));
        objectWidth = transform.GetComponent<SpriteRenderer>().bounds.extents.x; //extents = size of width / 2
        objectHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y; //extents = size of height / 2
    }


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
            
            Movement();
            PlayerRotation();
            
        }
    }

    void Movement()
    {
        movementVector.x = Input.GetAxis("Horizontal");
        movementVector.y = Input.GetAxis("Vertical");
        _viewPos += Time.deltaTime * speed * movementVector;
        _viewPos.x = Mathf.Clamp(_viewPos.x, screenBounds.x * -1 + objectWidth, screenBounds.x - objectWidth);
        _viewPos.y = Mathf.Clamp(_viewPos.y, screenBounds.y * -1 + objectHeight, screenBounds.y - objectHeight);
        transform.position = _viewPos;

    }

    private void PlayerRotation()
    {
        angleForRotation = Mathf.Atan2(movementVector.y, movementVector.x) * Mathf.Rad2Deg;
        if (movementVector.magnitude < 0.1f)
            transform.rotation = previousRotation;
        else
            transform.rotation = Quaternion.AngleAxis(angleForRotation, Vector3.forward);
        previousRotation = transform.rotation;
    }


}
