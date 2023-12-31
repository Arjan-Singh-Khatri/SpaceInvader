using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;
    public GameMode currentGameMode = GameMode.singlePlayer;
    public GameState currentGameState = GameState.waitingForPlayers;
    public GamePhase currentGamePhase = GamePhase.normal;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

}

public enum GameMode{
    singlePlayer,
    MultiPlayer,

}

public enum GameState
{
    waitingForPlayers,
    allPlayersReady,
    gamePaused
}

public enum GamePhase 
{
    allPlayersDead,
    boss,
    normal
}

