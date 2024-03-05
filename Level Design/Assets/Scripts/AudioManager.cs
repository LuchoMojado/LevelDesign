using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip sweepSound;
    public AudioClip damageSound;
    public AudioClip deathSound;

    private AudioSource audioSource;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlaySweepSound()
    {
        if (sweepSound != null && !audioSource.isPlaying)
        {
            audioSource.clip = sweepSound;
            audioSource.Play();
        }
    }

    public void PlayDamageFloorSound()
    {
        if (damageSound != null && !audioSource.isPlaying)
        {
            audioSource.clip = damageSound;
            audioSource.Play();
        }
    }

    public void PlayWallRunSound()
    {
        if (deathSound != null && !audioSource.isPlaying)
        {
            audioSource.clip = deathSound;
            audioSource.Play();
        }
    }
}
