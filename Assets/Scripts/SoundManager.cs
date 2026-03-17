using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] AudioSource bgm;
    [SerializeField] AudioSource sfx;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void PlayBGM(AudioClip clip)
    {
        bgm.clip = clip;
        bgm.ignoreListenerPause = true;
        Debug.Log($"BGM 클립 이름 : {bgm.clip.name}");
        bgm.Play();

    }

    public void StopBGM(AudioClip clip)
    {
        if (bgm.isPlaying)
        {
            bgm.Stop();
        }
    }

    public void StopCurBGM()
    {
        if (bgm.isPlaying)
        {
            bgm.Stop();
        }
    }

    public void PauseBGM()
    {
        if (bgm.isPlaying == false)
            return;
        bgm.Pause();
    }

    public void LoopBGM(bool loop)
    {
        bgm.loop = loop;
    }

    public void SetBGM(float volume, float pitch = 1f)
    {
        bgm.volume = volume;
        bgm.pitch = pitch;
    }

    public void PlaySFX(AudioClip clip)
    {
        sfx.PlayOneShot(clip);
    }

    public void SetSFX(float volume, float pitch = 1f)
    {
        sfx.volume = volume;
        sfx.pitch = pitch;
    }
}
