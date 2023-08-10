using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    [SerializeField] List<GameObject> prefabList = new List<GameObject>();
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();
    [SerializeField]private float instantiateTImer = 1.7f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        instantiateTImer += Time.deltaTime;
        if(instantiateTImer >= 1.7f)
        {
            Instantiate();
        }  
    }

    void Instantiate()
    {

        GameObject instantiatedGameObject =  Instantiate(prefabList[Random.Range(0,prefabList.Count-1)], 
                                             spawnPoints[Random.Range(0, spawnPoints.Count - 1)].position,Quaternion.identity);
        instantiateTImer = 0f;
    }


}
