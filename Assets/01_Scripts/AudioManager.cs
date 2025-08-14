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

    public AudioClip[] sfxClips; // Inspector에서 배열로 할당
    public AudioClip[] bgmClips; // Inspector에서 배열로 할당

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
        PlayLoopSFX(sfxIndex, owner, 1f);
    }

    public void PlayLoopSFX(int sfxIndex, GameObject owner, float pitch = 1f) {
        if (sfxClips == null || sfxIndex < 0 || sfxIndex >= sfxClips.Length) return;
        if (loopSfxDict.ContainsKey(owner)) return;

        AudioSource src = owner.AddComponent<AudioSource>();
        src.clip = sfxClips[sfxIndex];
        src.loop = true;
        src.spatialBlend = 0f;
        src.pitch = pitch;
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
        // 3D 효과음 볼륨은 개별로 PlaySFXAt에서 설정해야만 적용됨
    }

    // 배경음 재생 (인덱스)
    public void PlayBGM(int index, bool loop = true, float startTime = 0f) {
        if (bgmSource == null || bgmClips == null || index < 0 || index >= bgmClips.Length) return;
        bgmSource.clip = bgmClips[index];
        bgmSource.volume = SettingUI.instance != null ? SettingUI.instance.BGMUSIC_VOLUME : 1f;
        bgmSource.loop = loop;
        bgmSource.time = startTime; // 시작 위치 설정
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
        bgmSource.time = startTime; // 시작 위치 설정
        bgmSource.Play();
    }

    // 2D 효과음 (UI 용) - 풀에서 재생 (인덱스)
    public void PlayUISFX(int index) {
        if (sfxClips == null || index < 0 || index >= sfxClips.Length) return;
        PlayUISFX(sfxClips[index]);
    }

    // 2D 효과음 (UI 용) - 풀에서 재생 (인덱스, 속도 조절)
    public void PlayUISFX(int index, float pitch = 1f) {
        if (sfxClips == null || index < 0 || index >= sfxClips.Length) return;
        PlayUISFX(sfxClips[index], pitch);
    }

    // 2D 효과음 (UI 용) - 풀에서 재생 (이름)
    public void PlayUISFX(string clipName) {
        if (sfxClips == null) return;
        var clip = System.Array.Find(sfxClips, c => c != null && c.name == clipName);
        if (clip != null) PlayUISFX(clip);
    }

    // 2D 효과음 (UI 용) - 풀에서 재생 (이름, 속도 조절)
    public void PlayUISFX(string clipName, float pitch = 1f) {
        if (sfxClips == null) return;
        var clip = System.Array.Find(sfxClips, c => c != null && c.name == clipName);
        if (clip != null) PlayUISFX(clip, pitch);
    }

    // 2D 효과음 (UI 용) - 풀에서 재생 (AudioClip 직접)
    public void PlayUISFX(AudioClip clip) {
        PlayUISFX(clip, 1f);
    }

    // 2D 효과음 (UI 용) - 풀에서 재생 (AudioClip 직접, 속도 조절)
    public void PlayUISFX(AudioClip clip, float pitch = 1f) {
        float volume = SettingUI.instance != null ? SettingUI.instance.SFX_VOLUME : 1f;
        foreach (var src in uiSfxPool) {
            if (!src.isPlaying) {
                src.pitch = pitch;
                src.PlayOneShot(clip, volume);
                return;
            }
        }
        var extra = Instantiate(uiSfxSourcePrefab, transform);
        extra.spatialBlend = 0f;
        extra.pitch = pitch;
        uiSfxPool.Add(extra);
        extra.PlayOneShot(clip, volume);
    }

    // 3D 효과음 (오브젝트 위치에서, 인덱스)
    public void PlaySFXAt(int index, Vector3 position) {
        if (sfxClips == null || index < 0 || index >= sfxClips.Length) return;
        PlaySFXAt(sfxClips[index], position);
    }

    // 3D 효과음 (오브젝트 위치에서, 인덱스, 속도 조절)
    public void PlaySFXAt(int index, Vector3 position, float pitch = 1f) {
        if (sfxClips == null || index < 0 || index >= sfxClips.Length) return;
        PlaySFXAt(sfxClips[index], position, pitch);
    }

    // 3D 효과음 (오브젝트 위치에서, 이름)
    public void PlaySFXAt(string clipName, Vector3 position) {
        if (sfxClips == null) return;
        var clip = System.Array.Find(sfxClips, c => c != null && c.name == clipName);
        if (clip != null) PlaySFXAt(clip, position);
    }

    // 3D 효과음 (오브젝트 위치에서, 이름, 속도 조절)
    public void PlaySFXAt(string clipName, Vector3 position, float pitch = 1f) {
        if (sfxClips == null) return;
        var clip = System.Array.Find(sfxClips, c => c != null && c.name == clipName);
        if (clip != null) PlaySFXAt(clip, position, pitch);
    }

    // 3D 효과음 (오브젝트 위치에서, AudioClip 직접)
    public void PlaySFXAt(AudioClip clip, Vector3 position) {
        PlaySFXAt(clip, position, 1f);
    }

    // 3D 효과음 (오브젝트 위치에서, AudioClip 직접, 속도 조절)
    public void PlaySFXAt(AudioClip clip, Vector3 position, float pitch = 1f) {
        if (sfxSourcePrefab == null || clip == null) return;
        float volume = SettingUI.instance != null ? SettingUI.instance.SFX_VOLUME : 1f;
        AudioSource sfx = Instantiate(sfxSourcePrefab, position, Quaternion.identity);
        sfx.clip = clip;
        sfx.volume = volume;
        sfx.pitch = pitch;
        sfx.spatialBlend = 1f; // 3D 사운드
        sfx.Play();
        Destroy(sfx.gameObject, clip.length / pitch + 0.1f); // pitch에 따라 재생 시간 조정
    }

    // BGM 리버브 효과 제어
    public void SetBGMReverb(bool enable) {
        if (bgmSource == null) return;

        AudioReverbFilter reverbFilter = bgmSource.GetComponent<AudioReverbFilter>();
        
        if (enable) {
            // 리버브 효과 추가
            if (reverbFilter == null) {
                reverbFilter = bgmSource.gameObject.AddComponent<AudioReverbFilter>();
            }
            reverbFilter.enabled = true;
            // 설정창에 적합한 리버브 설정 (차분하고 부드러운 효과)
            reverbFilter.reverbPreset = AudioReverbPreset.Room;
            reverbFilter.dryLevel = -1000f; // 원본 소리 감소
            reverbFilter.room = -600f;      // 룸 효과
            reverbFilter.roomHF = -1200f;   // 고주파 감소
        } else {
            // 리버브 효과 제거
            if (reverbFilter != null) {
                reverbFilter.enabled = false;
            }
        }
    }
}