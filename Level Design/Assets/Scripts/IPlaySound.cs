using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlaySound 
{
    public void PlaySound(AudioClip clip, bool loop);

    public void StopSound();
}
