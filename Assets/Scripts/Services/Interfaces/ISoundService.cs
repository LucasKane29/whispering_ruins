using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISoundService : IService
{
    void PlayOneShot(AudioClip clip, Vector3 position, float volume = 1f);
    void Play2D(AudioClip clip, float volume = 1f);
    void PlayMusic(AudioClip clip, float fadeDuration = 0f);
    void StopMusic(float fadeDuration = 0f);
}
