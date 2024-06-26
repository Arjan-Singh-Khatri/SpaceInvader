using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource explode;
    [SerializeField] AudioClip explodeClip;
    [SerializeField]bool enemyDestroyed = false;
    private void Start()
    {
        explode = GetComponent<AudioSource>();
        Events.instance.playExplodeSound += PlayExplodeSound;
    }
    private void Update()
    {
        if (enemyDestroyed)
        {
            PlayExplodeSound();
            enemyDestroyed = false;
        }
            
    }
    void PlayExplodeSound()
    {
        
        explode.clip = explodeClip;
        explode.PlayOneShot(explode.clip, .1f);
    }
   
}

