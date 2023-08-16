using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource explode;
    [SerializeField] AudioClip explodeClip;
    [SerializeField]bool True = false;
    private void Start()
    {
        explode = GetComponent<AudioSource>();
        Events.playExplodeSound += PlayExplodeSound;
    }
    private void Update()
    {
        if (True)
        {
            PlayExplodeSound();
            True = false;
        }
            
    }
    void PlayExplodeSound()
    {
        explode.clip = explodeClip;
        explode.PlayOneShot(explode.clip, .1f);
    }    
}

