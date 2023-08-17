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
}
