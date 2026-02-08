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
    
    // [추가] 업적 아이콘을 저장할 Sprite 타입의 필드
    // public으로 선언해야 Unity Inspector 창에서 보입니다.
    public Sprite icon;        

    public AchiveData(string key, AchiveType type) {
        this.key = key;
        this.type = type;
        this.icon = null; // 생성자에서 기본값은 null로 설정
        isClear = false;
    }
}

[DefaultExecutionOrder(-10)]
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

    public void AchiveSort()
    {

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

        // 기존 UI 오브젝트 삭제
        foreach (Transform child in achiveListParent)
            Destroy(child.gameObject);

        int clearCount = 0;
        foreach (var achive in achives) {
            // 프리팹 생성
            GameObject go = Instantiate(achiveItemPrefab, achiveListParent);
            
            // 컴포넌트 찾아오기
            var texts = go.GetComponentsInChildren<TextMeshProUGUI>();
            var image = go.GetComponent<Image>();

            Image iconImage = null;
            Transform iconTransform = go.transform.Find("IconImage"); // Find 안에 있는 내용과 일치하는 게임 오브젝트
            if (iconTransform != null) {
                iconImage = iconTransform.GetComponent<Image>();
            }
            // [수정 끝]

            if (image != null) {
                image.color = achive.isClear ? Color.white : new Color(0.6f, 0.6f, 0.6f);
            }

            if (texts.Length >= 3) {
                if (achive.isClear) {
                    // 업적 클리어 시
                    string name = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.key + "_name");
                    string desc = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.key + "_desc");
                    string location = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.type.ToString());
                    texts[0].text = name;
                    texts[1].text = desc;
                    texts[2].text = location; // 업적 위치 표시
                    clearCount++;

                    //아이콘을 찾아 표시
                    if (iconImage != null && achive.icon != null) {
                        iconImage.sprite = achive.icon;    // 데이터에 저장된 스프라이트를 할당
                        iconImage.color = Color.white;     // 아이콘을 불투명하게 만들어 표시
                    }
                    // [수정 끝]
                }
                else {
                    // 업적 미달성 시
                    string location = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.type.ToString());
                    texts[0].text = "???";
                    texts[1].text = "???";
                    texts[2].text = location; // 업적 위치 표시

                    //클리어하지 않았다면 아이콘을 숨김
                    if (iconImage != null) {
                        iconImage.sprite = null;
                        iconImage.color = new Color(1, 1, 1, 0); // 아이콘을 투명하게 만들어 숨김
                    }
                }
            }
        }

        // 전체 달성률 UI 업데이트
        achiveSlider.value = clearCount / (float)achives.Count;
        achivePercentText.text = $"{clearCount} / {achives.Count}";
    }
    
    public bool IsAchiveCleared(string key)
    {
        var achive = achives.Find(a => a.key == key);
        return achive != null && achive.isClear;
    }
}
