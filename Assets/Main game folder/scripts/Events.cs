using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour
{

    public static Events instance;
    private void Start()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public  Action playExplodeSound;
    public delegate void WaveDelegate(int waveNumber);
    public  WaveDelegate waveDelegate;
    public delegate void AmmoCount(int bullet, int missile);
    public  AmmoCount ammoCount;
    public delegate void HealthCount(int health);
    public  HealthCount healthCount;
    public  Action gameOver;
    public  Action playerReady;

    public Action GamePausedMultiplayer;
    public Action GameUnpausedMultiplayer;

    public Action CallToPauseGameMulti;
    public Action CallToUnPauseGameMulti;

    public Action playerReadyPanelToggleOff;
    public Action playerPanelToggleOn;
    public Action playerDeathListAdd;
    public Action playerDeathUI;
    public Action allPlayerDeadUI;
    public Action hostDisconnect;

    public Action GameWonToggleEvent;
    public Action GameWonUI;

    public Action Host;
    public Action client;

}
