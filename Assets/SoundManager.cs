using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource, altAudioSource, audioTestSource;  

    [Header("Audios with")]
    public AudioClip conv1_br, conv2_br, conv3_br, win_br, term1_br, term2_br, term3_br;
    [Header("Audios without")]
    public AudioClip conv1_no, conv2_no, conv3_no, win_no, term1_no, term2_no, term3_no;

    private AudioClip conv1, conv2, conv3, win, term1, term2, term3;
    [Header("UI")]
    public AudioClip lostlife, shoot, enemydestroyed, terminate_ai;
    public AudioClip whitenoise;

    [Header("Other")]
    public AudioClip audioTest;

    float whitenoise_volume = 0.2f;
    float ui_sounds_volume = 0.5f;
    float audio_test_volume = 1f;
    float conv_volume = 1.6f;

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

    void Update()
    {
        if(! altAudioSource.isPlaying)
            altAudioSource.PlayOneShot(whitenoise, whitenoise_volume);
    }

    public void SetCondition()
    {
        if (PlayerPrefs.HasKey("Condition"))
        {
            switch (PlayerPrefs.GetString("Condition"))
            {
                case "With":
                    conv1 = conv1_br;
                    conv2 = conv2_br;
                    conv3 = conv3_br;
                    win = win_br;
                    term1 = term1_br;
                    term2 = term2_br;
                    term3 = term3_br;
                    break;

                case "Without":
                    conv1 = conv1_no;
                    conv2 = conv2_no;
                    conv3 = conv3_no;
                    win = win_no;
                    term1 = term1_no;
                    term2 = term2_no;
                    term3 = term3_no;
                    break;
            }
        }
    }

    public void PlayAudioTest()
    {
        audioTestSource.PlayOneShot(audioTest, audio_test_volume);
    }
    public void StopAudioTest()
    {
        audioTestSource.Pause();
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


