using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class SpaceShipMovementMenu : MonoBehaviour
{
    [SerializeField] private Transform[] wayPointsForPath;
    private Vector3[] vectorOfWayPoints;
    [SerializeField] private float duration = 3f;
    [SerializeField] PathType pathType;
    [SerializeField] PathMode pathMode;
    [SerializeField] Ease easeType;
    [SerializeField] Color color = Color.white;
    Vector2 previousPosition;
    Quaternion rotation;

    // Start is called before the first frame update
    void Start()
    {
        previousPosition = transform.position;
        vectorOfWayPoints = new Vector3[wayPointsForPath.Length];
        for(int i = 0; i< wayPointsForPath.Length; i++)
        {
            vectorOfWayPoints[i] = wayPointsForPath[i].position;
        }
        transform.DOPath(vectorOfWayPoints, duration, pathType, pathMode,10,color).SetLoops(-1,LoopType.Restart).SetEase(easeType);
        
    }

    private void Update()
    {
        Vector2 currentPosition = transform.position;
        Vector2 direction = (currentPosition - previousPosition).normalized;
        previousPosition = currentPosition;
        rotation = Quaternion.LookRotation(Vector3.forward,direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation,rotation,1000*Time.deltaTime);
   
    }

}
