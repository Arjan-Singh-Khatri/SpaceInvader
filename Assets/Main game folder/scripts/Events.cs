using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour
{
    public static Action playExplodeSound;
    public delegate void WaveDelegate(int waveNumber);
    public static WaveDelegate waveDelegate;
    public delegate void AmmoCount(int bullet, int missile);
    public static AmmoCount ammoCount;
    public delegate void HealthCount(int health);
    public static HealthCount healthCount;
    public static Action gameOver;
    public static Action playerReady;
    public static Action gamePausedBySomePlayerMulti;
    public static Action gameUnpausedBySomePlayerMulti;
    public static Action CallToPauseGameMulti;
    public static Action CallToUnPauseGameMulti;

    public static Action playerReadyPanelToggleOff;
    public static Action playerPanelToggleOn;
    public static Action playerShipDestroy;

}
