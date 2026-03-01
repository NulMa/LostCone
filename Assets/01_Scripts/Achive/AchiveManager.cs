using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Localization.Settings;

public enum AchiveType {
    Lemon,
    DonutHole,
    Junk,
    Common
}

[Serializable]
public class AchiveData {
    public string key;
    public bool isClear;
    public AchiveType type;
    public AnimationClip clearAnimation;
    public Sprite icon;

    public AchiveData(string key, AchiveType type) {
        this.key = key;
        this.type = type;
        this.icon = null;
        isClear = false;
    }
}

[DefaultExecutionOrder(-10)]
public class AchiveManager : MonoBehaviour {
    public static AchiveManager instance;
    public List<AchiveData> achives = new List<AchiveData>();
    public GameObject achivePopup;
    public Canvas Canvas;
    public Slider achiveSlider;
    public TextMeshProUGUI achivePercentText;
    public Sprite lockIcon;
    public Transform achiveListParent;
    public GameObject achiveItemPrefab;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAchives();
            RefreshUI();
        }
        else {
            Destroy(gameObject);
        }
    }

    public void SetAchiveClear(string key) {
        var achive = achives.Find(a => a.key == key);
        if (achive != null && !achive.isClear) {
            achive.isClear = true;
            SaveAchives();
            if (achivePopup != null && Canvas != null){
                GameObject popupObj = Instantiate(achivePopup, Canvas.transform);
                if (popupObj.transform.childCount > 1){
                    Transform child = popupObj.transform.GetChild(1);
                    var tmp = child.GetComponent<TextMeshProUGUI>();
                    if (tmp != null){
                        string name = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.key + "_name");
                        tmp.text = name;
                    }
                }
            }
        }
    }

    public void SaveAchives() {
        foreach (var achive in achives) {
            PlayerPrefs.SetInt("achive_" + achive.key, achive.isClear ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    public void LoadAchives() {
        foreach (var achive in achives) {
            achive.isClear = PlayerPrefs.GetInt("achive_" + achive.key, 0) == 1;
        }
    }

    public void RefreshUI() {
        if (achiveListParent == null || achiveItemPrefab == null)
            return;

        foreach (Transform child in achiveListParent)
            Destroy(child.gameObject);

        int clearCount = 0;
        foreach (var achive in achives) {
            GameObject go = Instantiate(achiveItemPrefab, achiveListParent);
            var texts = go.GetComponentsInChildren<TextMeshProUGUI>();
            
            Image backgroundImage = go.GetComponent<Image>();
            if (backgroundImage != null) {
                backgroundImage.color = achive.isClear ? Color.white : new Color(0.6f, 0.6f, 0.6f);
            }

            Image iconImage = null;
            Transform iconTransform = go.transform.Find("IconImage");
            if (iconTransform != null) {
                iconImage = iconTransform.GetComponent<Image>();
            }

            if (texts.Length >= 3) {
                if (achive.isClear) {
                    clearCount++;
                    texts[0].text = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.key + "_name");
                    texts[1].text = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.key + "_desc");
                    texts[2].text = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.type.ToString());

                    if (iconImage != null) {
                        // 기본 아이콘 설정
                        if (achive.icon != null) {
                            iconImage.sprite = achive.icon;
                        }
                        iconImage.color = Color.white;

                        Animator animator = iconImage.GetComponent<Animator>();
                        if (achive.clearAnimation != null && animator != null) {
                            animator.enabled = true;
                            
                            // AnimatorOverrideController 생성 및 적용
                            var overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
                            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                            
                            // 원본 컨트롤러의 모든 클립을 목표 클립으로 교체
                            foreach (var clip in animator.runtimeAnimatorController.animationClips) {
                                overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, achive.clearAnimation));
                            }
                            
                            overrideController.ApplyOverrides(overrides);
                            animator.runtimeAnimatorController = overrideController;

                            // 중요: Rebind 후 수동으로 Play 호출
                            animator.Rebind();
                            animator.Play(0, -1, 0f);
                        }
                        else if (animator != null) {
                            animator.enabled = false;
                        }
                    }
                }
                else {
                    texts[0].text = "???";
                    texts[1].text = "???";
                    texts[2].text = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.type.ToString());
                    if (iconImage != null) {
                        iconImage.sprite = lockIcon;
                        iconImage.color = Color.white;
                        Animator animator = iconImage.GetComponent<Animator>();
                        if (animator != null) animator.enabled = false;
                    }
                }
            }
        }

        if (achiveSlider != null) achiveSlider.value = achives.Count > 0 ? clearCount / (float)achives.Count : 0;
        if (achivePercentText != null) achivePercentText.text = $"{clearCount} / {achives.Count}";
    }
    
    public bool IsAchiveCleared(string key) {
        var achive = achives.Find(a => a.key == key);
        return achive != null && achive.isClear;
    }
}
