using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] List<EnemyToGenerate> enemiesList = new();
    [SerializeField] List<Transform> spawnPoints = new();
    [SerializeField] private float instantiateTimer = 0f;
    [SerializeField] private float waveIntervalTimer = 5f;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform bossLocation;

    private List<GameObject> EnemiesListTospwan = new();
    private List<GameObject> SpwanedEnemies = new();
    private int waveValue = 0;
    private int waveNumber = 0;
    private bool enemiesLeft;
    // Start is called before the first frame update
    void Start()
    {
        enemiesLeft = CheckIfEnemyleft();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (waveNumber==3)
        {
            InstantiateBoss();   
            gameObject.SetActive(false);
        }else
        {
            if (enemiesLeft)
            {
                instantiateTimer -= Time.fixedDeltaTime;
                if(instantiateTimer <= 0)
                {
                    instantiateTimer = 4f;
                    if (EnemiesListTospwan.Count > 0)
                    {
                        GameObject enemy = Instantiate(EnemiesListTospwan[0], spawnPoints[Random.Range(0, spawnPoints.Count)].position, Quaternion.identity);
                        EnemiesListTospwan.RemoveAt(0);
                        SpwanedEnemies.Add(enemy);
                    }
                    else
                        if (!CheckIfEnemyleft()) enemiesLeft = false;            
                }
            } 
            else
            {
                SpwanedEnemies.Clear();
                waveIntervalTimer -= Time.fixedDeltaTime;
                if (waveIntervalTimer <= 0)
                {
                    ++waveNumber;
                    Events.waveDelegate(waveNumber);
                    waveValue = waveNumber * 10 + 5;
                    waveIntervalTimer = 5f;
                    GenerateEnemies();
                    enemiesLeft = true;
                }
            } 
        }
         
    }

    #region New
    bool CheckIfEnemyleft()
    {
        if (GameObject.FindWithTag("Enemy") == null)
            return false;
        else
            return true;
    }

    void GenerateEnemies()
    {
        List<GameObject> generatedEnemiesList = new();
        while(waveValue >0)
        {
            int randomNumber = Random.Range(0, enemiesList.Count);
            if(waveValue >= enemiesList[randomNumber].weight)
            {
                waveValue -= enemiesList[randomNumber].weight;
                generatedEnemiesList.Add(enemiesList[randomNumber].enemy);
                
            }else if(waveValue <= 0)
            {
                break;
            }
            
        }
        EnemiesListTospwan.Clear();
        EnemiesListTospwan = generatedEnemiesList;
    }

    #endregion

    #region BossPhase
    private void InstantiateBoss()
    {
        Instantiate(bossPrefab,bossLocation.position,Quaternion.identity);
    }

    #endregion
}

[System.Serializable]
public class EnemyToGenerate
{
    public GameObject enemy;
    public int weight;
}

