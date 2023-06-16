using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private AudioClip musicAudio;
    void Start()
    {
        //audioSource.GetComponent<AudioSource>();
        StartMusic();
    }


    public void StartMusic()
    {
        audioSource.Play();
    }
    
    public void StopMusic()
    {
        audioSource.Stop();
    }
}
