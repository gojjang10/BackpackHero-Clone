using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] AudioSource bgm;
    [SerializeField] AudioSource sfx;

    // 페이드 연출을 위한 변수들
    private Coroutine fadeCoroutine;
    private float defaultBgmVolume = 1f; // 인스펙터에서 설정한 BGM 기본 볼륨을 여기에 적어주세요

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

    // 페이드 아웃 -> 클립 교체 -> 페이드 인 해주는 만능 함수
    public void PlayBGMWithFade(AudioClip newClip, float duration = 1.0f)
    {
        // 똑같은 노래를 틀라고 하면 무시
        if (bgm.clip == newClip) return;

        // 이미 페이드 중이라면 멈추고 새로 시작
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine(newClip, duration));
    }

    private IEnumerator FadeRoutine(AudioClip newClip, float duration)
    {
        float halfDuration = duration / 2f;
        float startVolume = bgm.volume;

        // 1. 페이드 아웃 (소리가 서서히 줄어듦)
        if (bgm.isPlaying)
        {
            float elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                bgm.volume = Mathf.Lerp(startVolume, 0f, elapsed / halfDuration);
                yield return null;
            }
        }

        // 2. 노래 교체 및 재생
        bgm.clip = newClip;
        bgm.Play();

        // 3. 페이드 인 (소리가 서서히 커짐)
        float elapsedIn = 0f;
        while (elapsedIn < halfDuration)
        {
            elapsedIn += Time.deltaTime;
            bgm.volume = Mathf.Lerp(0f, defaultBgmVolume, elapsedIn / halfDuration);
            yield return null;
        }

        bgm.volume = defaultBgmVolume; // 오차 보정
    }
}
