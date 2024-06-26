using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgMusic : MonoBehaviour
{
    public static BgMusic instance;
    private AudioSource bgMusicSource;
    [SerializeField] AudioClip bgMusicClip;

    private void Start()
    {
        instance = this;
        bgMusicSource = GetComponent<AudioSource>();      
        PlayMusic();
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        PlayMusic();
    }

    void PlayMusic()
    {
        if(GameStateManager.Instance.currentGamePhase == GamePhase.boss)
        {
            bgMusicSource.volume = Mathf.Lerp(bgMusicSource.volume, 0,2);
            //bgMusicSource.volume = 0f;
        }else
        {
            //bgMusicSource.volume = 0.3f;
            bgMusicSource.volume = Mathf.Lerp(0, 0.3f,2);
        }
    }
}
