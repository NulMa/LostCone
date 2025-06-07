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
    public AudioSource sfxSourcePrefab;      // 3D ȿ������ ������
    public AudioSource uiSfxSourcePrefab;    // 2D ȿ������ ������

    public AudioClip[] sfxClips; // Inspector���� ������� �Ҵ�
    public AudioClip[] bgmClips; // Inspector���� ������� �Ҵ�

    private List<AudioSource> uiSfxPool = new List<AudioSource>();
    private int poolSize = 5;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 2D ȿ���� Ǯ ����
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
        // 3D ȿ���� ������ ������ PlaySFXAt���� ���������� �����
    }

    // ����� ��� (�ε���)
    public void PlayBGM(int index, bool loop = true, float startTime = 0f) {
        if (bgmSource == null || bgmClips == null || index < 0 || index >= bgmClips.Length) return;
        bgmSource.clip = bgmClips[index];
        bgmSource.volume = SettingUI.instance != null ? SettingUI.instance.BGMUSIC_VOLUME : 1f;
        bgmSource.loop = loop;
        bgmSource.time = startTime; // ���� ��ġ ����
        bgmSource.Play();
    }

    // ����� ��� (�̸�)
    public void PlayBGM(string clipName, bool loop = true, float startTime = 0f) {
        if (bgmSource == null || bgmClips == null) return;
        var clip = System.Array.Find(bgmClips, c => c != null && c.name == clipName);
        if (clip == null) return;
        bgmSource.clip = clip;
        bgmSource.volume = SettingUI.instance != null ? SettingUI.instance.BGMUSIC_VOLUME : 1f;
        bgmSource.loop = loop;
        bgmSource.time = startTime; // ���� ��ġ ����
        bgmSource.Play();
    }

    // 2D ȿ���� (UI ��) - Ǯ���� ��� (�ε���)
    public void PlayUISFX(int index) {
        if (sfxClips == null || index < 0 || index >= sfxClips.Length) return;
        PlayUISFX(sfxClips[index]);
    }

    // 2D ȿ���� (UI ��) - Ǯ���� ��� (�̸�)
    public void PlayUISFX(string clipName) {
        if (sfxClips == null) return;
        var clip = System.Array.Find(sfxClips, c => c != null && c.name == clipName);
        if (clip != null) PlayUISFX(clip);
    }

    // 2D ȿ���� (UI ��) - Ǯ���� ��� (AudioClip ����)
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

    // 3D ȿ���� (������Ʈ ��ġ����, �ε���)
    public void PlaySFXAt(int index, Vector3 position) {
        if (sfxClips == null || index < 0 || index >= sfxClips.Length) return;
        PlaySFXAt(sfxClips[index], position);
    }

    // 3D ȿ���� (������Ʈ ��ġ����, �̸�)
    public void PlaySFXAt(string clipName, Vector3 position) {
        if (sfxClips == null) return;
        var clip = System.Array.Find(sfxClips, c => c != null && c.name == clipName);
        if (clip != null) PlaySFXAt(clip, position);
    }

    // 3D ȿ���� (������Ʈ ��ġ����, AudioClip ����)
    public void PlaySFXAt(AudioClip clip, Vector3 position) {
        if (sfxSourcePrefab == null || clip == null) return;
        float volume = SettingUI.instance != null ? SettingUI.instance.SFX_VOLUME : 1f;
        AudioSource sfx = Instantiate(sfxSourcePrefab, position, Quaternion.identity);
        sfx.clip = clip;
        sfx.volume = volume;
        sfx.spatialBlend = 1f; // 3D ����
        sfx.Play();
        Destroy(sfx.gameObject, clip.length + 0.1f);
    }
}