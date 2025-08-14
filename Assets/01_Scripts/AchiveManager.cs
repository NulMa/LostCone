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
    public string key;         // 업적 고유 키 (로컬화용)
    public bool isClear;       // 클리어 여부
    public AchiveType type;    // 업적 타입

    public AchiveData(string key, AchiveType type) {
        this.key = key;
        this.type = type;
        isClear = false;
    }
}

public class AchiveManager : MonoBehaviour {
    public static AchiveManager instance;
    public List<AchiveData> achives = new List<AchiveData>();
    public GameObject achivePopup;
    public Canvas Canvas; // 업적 팝업을 표시할 캔버스 (UI용, 필요시 할당)

    public Slider achiveSlider; // 업적 달성률 슬라이더 (UI용, 필요시 할당)
    public TextMeshProUGUI achivePercentText; // 업적 달성률 % 표시 (UI용, 필요시 할당)

    // UI용
    public Transform achiveListParent; // 업적 리스트 UI 부모 오브젝트
    public GameObject achiveItemPrefab; // 업적 리스트 프리팹 (TextMeshProUGUI 2개: 이름, 설명)

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

    // 업적 달성 설정 (외부에서 호출)
    public void SetAchiveClear(string key) {
        var achive = achives.Find(a => a.key == key);
        if (achive != null && !achive.isClear) {
            achive.isClear = true;
            SaveAchives();
            Debug.Log($"[AchiveManager] 업적 '{achive.key}' 달성!");
            

            // 업적 팝업 생성 (Canvas의 부모로)
            if (achivePopup != null && Canvas != null){
                GameObject popupObj = Instantiate(achivePopup, Canvas.transform);

                // 1번째 자식의 TextMeshProUGUI에 업적 이름 입력
                if (popupObj.transform.childCount > 1){
                    Transform child = popupObj.transform.GetChild(1);
                    var tmp = child.GetComponent<TMPro.TextMeshProUGUI>();
                    if (tmp != null){
                        string name = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.key + "_name");
                        tmp.text = name;
                    }
                }
            }
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

    // 업적 UI 새로고침
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
                    texts[2].text = location; // 업적 위치 표시
                    clearCount++;
                }
                else {
                    string location = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.type.ToString());
                    texts[0].text = "???";
                    texts[1].text = "???";
                    texts[2].text = location; // 업적 위치 표시
                }
            }
        }

        achiveSlider.value = clearCount / (float)achives.Count;
        achivePercentText.text = $"{clearCount} / {achives.Count}";
    }
}
