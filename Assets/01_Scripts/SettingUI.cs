using System.Collections;
using System.Collections.Generic;
using Blade.Managers;
using Blade.SoundSystem;
using PaperFlower.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    public float BGMUSIC_VOLUME;
    public float SFX_VOLUME;
    public int Language; // 0: en, 1: jp, 2: kr;

    public AudioMixer audioMixer;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public TextMeshProUGUI bgmText;
    public TextMeshProUGUI sfxText;
    public TMP_Dropdown langDropdown;
    public GameObject SettingPanel;
    public GameObject AchivePanel;

    public GameObject InGameUi;

    private bool wasSettingPanelActive = false; // 이전 프레임의 SettingPanel 상태

    public static SettingUI instance;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings(); // 게임 시작 시 저장된 설정 불러오기
            ApplySettingsToUI(); // UI에 설정 반영
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update(){
        if (bgmSlider != null){
            BGMUSIC_VOLUME = bgmSlider.value;
            //AudioManager.Instance.SetBGMVolume(BGMUSIC_VOLUME);
        }
        if (sfxSlider != null){
            SFX_VOLUME = sfxSlider.value;
            //AudioManager.Instance.SetSFXVolume(SFX_VOLUME);
        }

        // 현재 씬이 MainMenu일 때 InGameUi 비활성화
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu"){
            if (InGameUi != null && InGameUi.activeSelf){
                InGameUi.SetActive(false);
            }
        }
        else{
            if (InGameUi != null && !InGameUi.activeSelf){
                InGameUi.SetActive(true);
            }
        }

        // SettingPanel 상태 체크 및 시간/리버브 효과 제어
        CheckSettingPanelState();
    }

    public void ToggleSettingPanel(){

        if (AchivePanel.activeSelf)
        {
            AchivePanel.SetActive(false);
            return; // 업적 패널이 열려있으면 그것만 닫고 끝냄
        }

        if (SettingPanel != null)
        {
            SettingPanel.SetActive(!SettingPanel.activeSelf);
        }
    }
    
    private void CheckSettingPanelState()
    {
        if (SettingPanel == null) return;

        bool isSettingPanelActive = SettingPanel.activeSelf;

        // SettingPanel 상태가 변경되었을 때만 처리
        if (isSettingPanelActive != wasSettingPanelActive)
        {
            if (isSettingPanelActive)
            {
                // SettingPanel이 켜졌을 때
                Time.timeScale = 0f; // 시간 정지
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.SetBGMReverb(true); // 리버브 효과 적용
                }
            }
            else
            {
                // SettingPanel이 꺼졌을 때
                Time.timeScale = 1f; // 시간 복구
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.SetBGMReverb(false); // 리버브 효과 제거
                }
            }
            wasSettingPanelActive = isSettingPanelActive;
        }
    }

    public void toMain(){
        // 메인메뉴로 이동하기 전에 시간 복구 및 리버브 효과 제거
        Time.timeScale = 1f;
        if (AudioManager.Instance != null){
            AudioManager.Instance.SetBGMReverb(false);
            // 메인메뉴 BGM (5번) 재생
            AudioManager.Instance.PlayBGM(5);
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        SoundManager.Instance.StopAllLoopSounds();
    }

    public void gameExit(){
        Application.Quit();
    }

    // 플레이 중에 UI 컴포넌트를 할당하고 설정 동기화 및 이벤트 등록
    public void AssignUIAndSync(Slider newBgmSlider, Slider newSfxSlider, TMP_Dropdown newLangDropdown)
    {
        bgmSlider = newBgmSlider;
        sfxSlider = newSfxSlider;
        langDropdown = newLangDropdown;

        // 설정 동기화
        ApplySettingsToUI();

        // 슬라이더 이벤트 등록 및 기존 이벤트 제거
        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.RemoveAllListeners();
            bgmSlider.onValueChanged.AddListener(OnBGMSliderChanged);
        }
        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        }
        if (langDropdown != null)
        {
            langDropdown.onValueChanged.RemoveAllListeners();
            langDropdown.onValueChanged.AddListener(LangDropdown);
        }
    }

    public void LangDropdown(int langNum)
    {
        Language = langNum;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[langNum];
    }

    public void OnBGMSliderChanged(float value)
    {
        BGMUSIC_VOLUME = value;
        bgmText.text = Mathf.RoundToInt(value * 100).ToString();
        float volume = value <= 0.0001f ? -80f : Mathf.Log10(value) * 20;
        audioMixer.SetFloat("musicVolume", volume);
        AudioManager.Instance?.SetBGMVolume(value);
        SaveSettings();
    }

    public void OnSFXSliderChanged(float value)
    {
        SFX_VOLUME = value;
        sfxText.text = Mathf.RoundToInt(value * 100).ToString();
        float volume = value <= 0.0001f ? -80f : Mathf.Log10(value) * 20;
        audioMixer.SetFloat("sfxVolume", volume);
        AudioManager.Instance?.SetSFXVolume(value);
        SaveSettings();
    }

    // 저장
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("BGMUSIC_VOLUME", BGMUSIC_VOLUME);
        PlayerPrefs.SetFloat("SFX_VOLUME", SFX_VOLUME);
        PlayerPrefs.SetInt("Language", Language);
        PlayerPrefs.Save();
    }

    // 불러오기
    public void LoadSettings()
    {
        BGMUSIC_VOLUME = PlayerPrefs.HasKey("BGMUSIC_VOLUME") ? PlayerPrefs.GetFloat("BGMUSIC_VOLUME") : 0.3f;
        SFX_VOLUME = PlayerPrefs.HasKey("SFX_VOLUME") ? PlayerPrefs.GetFloat("SFX_VOLUME") : 0.3f;
        Language = PlayerPrefs.HasKey("Language") ? PlayerPrefs.GetInt("Language") : 0;
    }

    // UI에 설정 반영
    private void ApplySettingsToUI()
    {
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
