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
    public string key;         // 업적 고유 키 (저장용)
    public bool isClear;       // 클리어 여부
    public AchiveType type;    // 업적 종류

    public AchiveData(string key, AchiveType type) {
        this.key = key;
        this.type = type;
        isClear = false;
    }
}

public class AchiveManager : MonoBehaviour {
    public static AchiveManager instance;
    public List<AchiveData> achives = new List<AchiveData>();

    public Slider achiveSlider; // 업적 달성도 슬라이더 (UI용, 필요시 사용)
    public TextMeshProUGUI achivePercentText; // 업적 달성도 % 표시 (UI용, 필요시 사용)

    // UI용
    public Transform achiveListParent; // 업적 항목이 들어갈 부모 오브젝트
    public GameObject achiveItemPrefab; // 업적 항목 프리팹 (TextMeshProUGUI 2개: 이름, 설명)

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



    // 업적 달성 기록 (외부에서 호출)
    public void SetAchiveClear(string key) {
        var achive = achives.Find(a => a.key == key);
        if (achive != null && !achive.isClear) {
            achive.isClear = true;
            SaveAchives();
        }
    }

    // 업적 저장
    public void SaveAchives() {
        foreach (var achive in achives) {
            PlayerPrefs.SetInt("achive_" + achive.key, achive.isClear ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    // 업적 불러오기
    public void LoadAchives() {
        foreach (var achive in achives) {
            achive.isClear = PlayerPrefs.GetInt("achive_" + achive.key, 0) == 1;
        }
    }

    // 업적 UI 갱신
    public void RefreshUI() {
        if (achiveListParent == null || achiveItemPrefab == null)
            return;

        foreach (Transform child in achiveListParent)
            Destroy(child.gameObject);

        int clearCount = 0;
        foreach (var achive in achives) {
            GameObject go = Instantiate(achiveItemPrefab, achiveListParent);
            var texts = go.GetComponentsInChildren<TextMeshProUGUI>();
            var image = go.GetComponent<Image>();
            if (image != null) {
                image.color = achive.isClear ? Color.white : new Color(0.6f, 0.6f, 0.6f);
            }

            if (texts.Length >= 3) {
                if (achive.isClear) {
                    string name = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.key + "_name");
                    string desc = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.key + "_desc");
                    string location = LocalizationSettings.StringDatabase.GetLocalizedString("Achive",achive.type.ToString());
                    texts[0].text = name;
                    texts[1].text = desc;
                    texts[2].text = location; // 업적 종류 표시
                    clearCount++;
                }
                else {
                    string location = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.type.ToString());
                    texts[0].text = "???";
                    texts[1].text = "???";
                    texts[2].text = location; // 업적 종류 표시
                }
            }
        }

        achiveSlider.value = clearCount / (float)achives.Count;
        achivePercentText.text = $"{clearCount} / {achives.Count}";

    }
}