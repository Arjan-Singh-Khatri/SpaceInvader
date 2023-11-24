using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : NetworkBehaviour
{
    [SerializeField] List<EnemyToGenerate> enemiesList = new();
    [SerializeField] List<Transform> spawnPoints = new();
    [SerializeField] private float instantiateTimer = 0f;
    [SerializeField] private float waveIntervalTimer = 5f;
    [SerializeField] private GameObject bossPrefab;
    private Vector3 bossLocation = new Vector3(13.1400003f, -0.00999999978f, 1.50239027f);

    private NetworkObject spawnedObject;
    private List<GameObject> EnemiesListTospwan = new();
    private int waveValue = 0;
    private int waveNumber = 0;
    private bool enemiesLeftToSpawn;
    private bool bossPhase = false;

    private GameObject bossRef;
    private bool gameWon;

    

    // Start is called before the first frame update
    void Start()
    {
        enemiesLeftToSpawn = CheckIfEnemyleft();
        Events.instance.GameWonToggleEvent += ToggleGameWon;

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer)
        {
            if (!IsServer) return;
        }
        if (gameWon)
        {
            if(GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer )
            {
                GameWon();
                return;
            }else if(GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
            {
                GameWon();
                return;
            }
        }
        
        if (waveNumber == 3)
        {
            if(GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer )
            {
                SpawnBoss();
            }
            else if (GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
            {
               
                InstantiateBoss();
                bossPhase = true;
            }
                
            waveNumber++;
        }
        else
        {
            if (enemiesLeftToSpawn)
            {
                SpawnEnemies();
            }
            else
            {
                WaitForEnemyToSpawn();
            }
        }

    }


    #region Required Functions 

    void ToggleGameWon()
    {
        gameWon = true;
    }

    void GameWon()
    {
        
        if(!CheckIfEnemyleft())
        {
            GameWonServerRpc();
        }
    }
    #region ServerRpcCalls
    [ServerRpc(RequireOwnership = false)]
    void GameWonServerRpc()
    {
        GameWonUIClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds } });
    }

    [ClientRpc]
    void GameWonUIClientRpc(ClientRpcParams clientRpcParams)
    {
        Events.instance.GameWonUI();
    }
    #endregion

    #region Wave Txt Rpcs

    void WaveText(int waveNum)
    {
        if(GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
        {
            Events.instance.waveTextDelegate(waveNum);
        }else
        {
            WaveTextServerRpc(waveNum);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void WaveTextServerRpc(int waveNumber)
    {
        WaveTextClientRpc(waveNumber,new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds } });
    }

    [ClientRpc]
    void WaveTextClientRpc(int waveNumber ,ClientRpcParams clientRpcParams)
    {
        Events.instance.waveTextDelegate(waveNumber);
    }
    
    #endregion

    void WaitForEnemyToSpawn()
    {
        waveIntervalTimer -= Time.fixedDeltaTime;
        if (waveIntervalTimer <= 0)
        {
            if (bossPhase && !bossRef.GetComponent<Boss>().forceFieldOn)
            {
                ++waveNumber;
                waveValue = waveNumber * 2;
                waveIntervalTimer = 5f;
                GenerateEnemies();
                enemiesLeftToSpawn = true;
            }
            else if(!bossPhase)
            {
                ++waveNumber;
                if(waveNumber<=3)WaveText(waveNumber);
                waveValue = waveNumber * 10 +5;
                waveIntervalTimer = 5f;
                GenerateEnemies();
                enemiesLeftToSpawn = true;
            }
            
        }
    }

    void SpawnEnemies()
    {

        instantiateTimer -= Time.fixedDeltaTime;
        if (instantiateTimer <= 0)
        {
            instantiateTimer = 4f;
            if (EnemiesListTospwan.Count > 0)
            {
                InstantiateEnemies();
            }
            else
                if (!CheckIfEnemyleft()) enemiesLeftToSpawn = false;
        }
    }

    void InstantiateEnemies()
    {
        if (GameStateManager.Instance.currentGameMode == GameMode.MultiPlayer && IsServer)
        {
            GameObject enemy = Instantiate(EnemiesListTospwan[0], spawnPoints[Random.Range(0, spawnPoints.Count)].position, Quaternion.identity);
            enemy.GetComponent<NetworkObject>().Spawn(true);
            EnemiesListTospwan.RemoveAt(0);
        }
        else if(GameStateManager.Instance.currentGameMode == GameMode.singlePlayer)
        {
            GameObject enemy = Instantiate(EnemiesListTospwan[0], spawnPoints[Random.Range(0, spawnPoints.Count)].position, Quaternion.identity);
            EnemiesListTospwan.RemoveAt(0);
        }
    }

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

    [ServerRpc(RequireOwnership =false)]
    void BossPhaseTriggerServerRpc()
    {
        BossPhaseTriggerClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds } });
    }

    [ClientRpc]
    void BossPhaseTriggerClientRpc(ClientRpcParams clientRpcParams)
    {
        GameStateManager.Instance.currentGamePhase = GamePhase.boss;
    }

    private void InstantiateBoss()
    {
        GameObject instantiatedBoss = Instantiate(bossPrefab, bossLocation, Quaternion.identity);
        bossRef = instantiatedBoss;
        GameStateManager.Instance.currentGamePhase = GamePhase.boss;
        
    }

    private void SpawnBoss()
    {
        GameObject instantiatesBoss = Instantiate(bossPrefab, bossLocation, Quaternion.identity);
        bossRef = instantiatesBoss;
        instantiatesBoss.GetComponent<NetworkObject>().Spawn(true);
        BossPhaseTriggerServerRpc();
    }

    #endregion

    private void OnDestroy()
    {
        Events.instance.GameWonToggleEvent -= ToggleGameWon;
    }
}

[System.Serializable]
public class EnemyToGenerate
{
    public GameObject enemy;
    public int weight;
}

