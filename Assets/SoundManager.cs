using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource, altAudioSource;  
    public AudioClip conv1, conv2, conv3, win, term1, term2, term3;
    public AudioClip lostlife, shoot, enemydestroyed, terminate_ai;
    public AudioClip whitenoise;

    float whitenoise_volume = 0.3f;
    float ui_sounds_volume = 0.5f;
    float conv_volume = 1.5f;

    bool convo_is_paused;

    void Start()
    {
        convo_is_paused = false;

        if(!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }
        altAudioSource.PlayOneShot(whitenoise, whitenoise_volume);

    }

    public void PlayLostLife()
    {
        altAudioSource.PlayOneShot(lostlife, ui_sounds_volume);
    }

    public void PlayShoot()
    {
        altAudioSource.PlayOneShot(shoot, ui_sounds_volume);
    }

    public void PlayTerminateAI()
    {
        audioSource.Pause();
        altAudioSource.PlayOneShot(terminate_ai, ui_sounds_volume);
    }

    public void PlayEnemyDestroyed()
    {
        altAudioSource.PlayOneShot(enemydestroyed, ui_sounds_volume);
    }

    public void PauseConv()
    {
        audioSource.Pause();
        convo_is_paused = true;
    }

    public void ResumeConv()
    {
        audioSource.UnPause();
        convo_is_paused = false;
    }

    public void PlayConv(int step)
    {
        float volume = conv_volume;

        if(audioSource.isPlaying || convo_is_paused)
        {
            return;
        }

        switch(step)
        {
            case 1:
                audioSource.PlayOneShot(conv1, volume);
                break;
            case 2:
                audioSource.PlayOneShot(conv2, volume);
                break;
            case 3:
                audioSource.PlayOneShot(conv3, volume);
                break;
            case 4:
                audioSource.PlayOneShot(win, volume);
                break;
            case 5:
                audioSource.PlayOneShot(term1, volume);
                break;
            case 6:
                audioSource.PlayOneShot(term2, volume);
                break;
            case 7:
                audioSource.PlayOneShot(term3);
                break;
            default:
                Debug.LogWarning("Invalid step provided: " + step);
                break;
        }
    }
}


