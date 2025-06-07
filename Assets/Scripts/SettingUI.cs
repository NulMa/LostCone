using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    public float BGMUSIC_VOLUME;
    public float SFX_VOLUME;
    public int Language; // 0: en, 1: jp, 2: kr;

    public Slider bgmSlider;
    public Slider sfxSlider;
    public TMP_Dropdown langDropdown;
    public GameObject SettingPanel;

    public static SettingUI instance;
    // Start is called before the first frame update
    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings(); // 시작 시 저장된 값 불러오기
            ApplySettingsToUI(); // UI에 값 반영
        }
        else {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update(){
        if (bgmSlider != null) {
            BGMUSIC_VOLUME = bgmSlider.value;
            //AudioManager.Instance.SetBGMVolume(BGMUSIC_VOLUME);
        }
        if (sfxSlider != null) {
            SFX_VOLUME = sfxSlider.value;
            //AudioManager.Instance.SetSFXVolume(SFX_VOLUME);
        }

    }

    // 플레이 씬에서 UI 오브젝트를 할당하고 값 동기화 및 이벤트 연결
    public void AssignUIAndSync(Slider newBgmSlider, Slider newSfxSlider, TMP_Dropdown newLangDropdown) {
        bgmSlider = newBgmSlider;
        sfxSlider = newSfxSlider;
        langDropdown = newLangDropdown;

        // 값 동기화
        ApplySettingsToUI();

        // 기존 리스너 제거 후 새로 연결
        if (bgmSlider != null) {
            bgmSlider.onValueChanged.RemoveAllListeners();
            bgmSlider.onValueChanged.AddListener(OnBGMSliderChanged);
        }
        if (sfxSlider != null) {
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        }
        if (langDropdown != null) {
            langDropdown.onValueChanged.RemoveAllListeners();
            langDropdown.onValueChanged.AddListener(LangDropdown);
        }
    }

    public void LangDropdown(int langNum) {
        Language = langNum;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[langNum];
    }

    public void OnBGMSliderChanged(float value) {
        BGMUSIC_VOLUME = value;
        AudioManager.Instance?.SetBGMVolume(value);
        SaveSettings();
    }

    public void OnSFXSliderChanged(float value) {
        SFX_VOLUME = value;
        AudioManager.Instance?.SetSFXVolume(value);
        SaveSettings();
    }

    // 저장
    public void SaveSettings() {
        PlayerPrefs.SetFloat("BGMUSIC_VOLUME", BGMUSIC_VOLUME);
        PlayerPrefs.SetFloat("SFX_VOLUME", SFX_VOLUME);
        PlayerPrefs.SetInt("Language", Language);
        PlayerPrefs.Save();
    }

    // 불러오기
    public void LoadSettings() {
        BGMUSIC_VOLUME = PlayerPrefs.HasKey("BGMUSIC_VOLUME") ? PlayerPrefs.GetFloat("BGMUSIC_VOLUME") : 0.3f;
        SFX_VOLUME = PlayerPrefs.HasKey("SFX_VOLUME") ? PlayerPrefs.GetFloat("SFX_VOLUME") : 0.3f;
        Language = PlayerPrefs.HasKey("Language") ? PlayerPrefs.GetInt("Language") : 0;
    }

    // UI에 값 반영
    private void ApplySettingsToUI() {
        if (bgmSlider != null)
            bgmSlider.value = BGMUSIC_VOLUME;
        if (sfxSlider != null)
            sfxSlider.value = SFX_VOLUME;
        if (langDropdown != null)
            LangDropdown(Language);

        AudioManager.Instance?.SetBGMVolume(BGMUSIC_VOLUME);
        AudioManager.Instance?.SetSFXVolume(SFX_VOLUME);
    }

}
