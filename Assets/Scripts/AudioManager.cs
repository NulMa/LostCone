using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFXType {
    BGM,
    UI,
    Sfx,
}

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    public AudioSource bgmSource;
    public AudioSource sfxSourcePrefab;      // 3D 효과음용 프리팹
    public AudioSource uiSfxSourcePrefab;    // 2D 효과음용 프리팹

    public AudioClip[] sfxClips; // Inspector에서 순서대로 할당
    public AudioClip[] bgmClips; // Inspector에서 순서대로 할당

    private List<AudioSource> uiSfxPool = new List<AudioSource>();
    private int poolSize = 5;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 2D 효과음 풀 생성
            for (int i = 0; i < poolSize; i++) {
                var src = Instantiate(uiSfxSourcePrefab, transform);
                src.spatialBlend = 0f;
                uiSfxPool.Add(src);
            }
        }
        else {
            Destroy(gameObject);
        }
    }

    private Dictionary<GameObject, AudioSource> loopSfxDict = new Dictionary<GameObject, AudioSource>();

    public void PlayLoopSFX(int sfxIndex, GameObject owner) {
        if (sfxClips == null || sfxIndex < 0 || sfxIndex >= sfxClips.Length) return;
        if (loopSfxDict.ContainsKey(owner)) return;

        AudioSource src = owner.AddComponent<AudioSource>();
        src.clip = sfxClips[sfxIndex];
        src.loop = true;
        src.spatialBlend = 0f;
        src.volume = SettingUI.instance != null ? SettingUI.instance.SFX_VOLUME : 1f;
        src.Play();
        loopSfxDict[owner] = src;
    }

    public void StopLoopSFX(GameObject owner) {
        if (loopSfxDict.TryGetValue(owner, out var src)) {
            src.Stop();
            Destroy(src);
            loopSfxDict.Remove(owner);
        }
    }


    public void SetBGMVolume(float volume) {
        if (bgmSource != null)
            bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume) {
        foreach (var src in uiSfxPool)
            src.volume = volume;
        // 3D 효과음 프리팹 볼륨은 PlaySFXAt에서 개별적으로 적용됨
    }

    // 배경음 재생 (인덱스)
    public void PlayBGM(int index, bool loop = true, float startTime = 0f) {
        if (bgmSource == null || bgmClips == null || index < 0 || index >= bgmClips.Length) return;
        bgmSource.clip = bgmClips[index];
        bgmSource.volume = SettingUI.instance != null ? SettingUI.instance.BGMUSIC_VOLUME : 1f;
        bgmSource.loop = loop;
        bgmSource.time = startTime; // 시작 위치 지정
        bgmSource.Play();
    }

    // 배경음 재생 (이름)
    public void PlayBGM(string clipName, bool loop = true, float startTime = 0f) {
        if (bgmSource == null || bgmClips == null) return;
        var clip = System.Array.Find(bgmClips, c => c != null && c.name == clipName);
        if (clip == null) return;
        bgmSource.clip = clip;
        bgmSource.volume = SettingUI.instance != null ? SettingUI.instance.BGMUSIC_VOLUME : 1f;
        bgmSource.loop = loop;
        bgmSource.time = startTime; // 시작 위치 지정
        bgmSource.Play();
    }

    // 2D 효과음 (UI 등) - 풀에서 사용 (인덱스)
    public void PlayUISFX(int index) {
        if (sfxClips == null || index < 0 || index >= sfxClips.Length) return;
        PlayUISFX(sfxClips[index]);
    }

    // 2D 효과음 (UI 등) - 풀에서 사용 (이름)
    public void PlayUISFX(string clipName) {
        if (sfxClips == null) return;
        var clip = System.Array.Find(sfxClips, c => c != null && c.name == clipName);
        if (clip != null) PlayUISFX(clip);
    }

    // 2D 효과음 (UI 등) - 풀에서 사용 (AudioClip 직접)
    public void PlayUISFX(AudioClip clip) {
        float volume = SettingUI.instance != null ? SettingUI.instance.SFX_VOLUME : 1f;
        foreach (var src in uiSfxPool) {
            if (!src.isPlaying) {
                src.PlayOneShot(clip, volume);
                return;
            }
        }
        var extra = Instantiate(uiSfxSourcePrefab, transform);
        extra.spatialBlend = 0f;
        uiSfxPool.Add(extra);
        extra.PlayOneShot(clip, volume);
    }

    // 3D 효과음 (오브젝트 위치에서, 인덱스)
    public void PlaySFXAt(int index, Vector3 position) {
        if (sfxClips == null || index < 0 || index >= sfxClips.Length) return;
        PlaySFXAt(sfxClips[index], position);
    }

    // 3D 효과음 (오브젝트 위치에서, 이름)
    public void PlaySFXAt(string clipName, Vector3 position) {
        if (sfxClips == null) return;
        var clip = System.Array.Find(sfxClips, c => c != null && c.name == clipName);
        if (clip != null) PlaySFXAt(clip, position);
    }

    // 3D 효과음 (오브젝트 위치에서, AudioClip 직접)
    public void PlaySFXAt(AudioClip clip, Vector3 position) {
        if (sfxSourcePrefab == null || clip == null) return;
        float volume = SettingUI.instance != null ? SettingUI.instance.SFX_VOLUME : 1f;
        AudioSource sfx = Instantiate(sfxSourcePrefab, position, Quaternion.identity);
        sfx.clip = clip;
        sfx.volume = volume;
        sfx.spatialBlend = 1f; // 3D 사운드
        sfx.Play();
        Destroy(sfx.gameObject, clip.length + 0.1f);
    }
}