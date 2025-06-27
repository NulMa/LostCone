using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
public class GamaManager : MonoBehaviour{
    public static GamaManager Instance { get; private set; }

    public GameObject[] BGs;
    public GameObject[] Maps;
    public SceneManager SceneManager;
    public ItemDataManager ItemDataManager;
    public UICtrl UIManager;
    public Player player;
    public GameObject playerParent;
    public MapItems currentMap;
    public Image panel;
    public Camera mainCam;

    public GameObject rain;


    
    public int langue; // 언어 인덱스
    public int currentStageID;


    private void Awake() {
        if (PlayerPrefs.HasKey("Player_Pos_X"))
            LoadGame();


        // 싱글톤 인스턴스 설정
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject); // 중복된 인스턴스 제거
        }

        if(GamaManager.Instance.SceneManager.scenes[1].isDone) rain.SetActive(false);
    }

    private void Update() {
        ItemDataManager.CurrentMapID = currentStageID;
    }

    public void rainSwitch() {
        if (rain.activeSelf) {
            rain.SetActive(false);
        }
        else {
            rain.SetActive(true);
        }
    }

    public void passFirstScene() {
        Debug.Log("첫 씬을 넘깁니다 : " + SceneManager.scenes[0].isDone);
        DOTween.Kill(SceneManager.scenes[0].scene);

        player.isScenePlaying = false;
        panel.gameObject.SetActive(false);
        SceneManager.scenes[0].scene.SetActive(false);
        mainCam.orthographicSize = 7.5f;
        BGOn(currentStageID);
        Maps[0].GetComponent<MapNumbers>().OnOffAll();

        UIManager.Message.gameObject.SetActive(true);
        UIManager.Message.DOColor(Color.black, 1f);

        UIManager.Stage.gameObject.SetActive(true);
        UIManager.Items.gameObject.SetActive(true);
        Color color = new Color(0x63 / 255f, 0x63 / 255f, 0x63 / 255f);
        UIManager.Stage.DOColor(color, 1f);
        UIManager.Items.DOColor(color, 1f);

        foreach (Transform child in player.transform) {
            // 자식 오브젝트의 태그가 "Remove"인지 확인
            if (child.CompareTag("Remove")) {
                // 해당 오브젝트 삭제
                Destroy(child.gameObject);
            }
        }

        if(currentStageID == 0 && !SceneManager.scenes[1].isDone) {
            rainSwitch();
        }
        currentMap.gameObject.SetActive(true);

    }

    public void SaveGame() {
        player.SavePlayerPosition();
        ItemDataManager.Instance.SaveCurrentMapData();
        currentMap.SaveMapData();
        SceneManager.SaveSceneData();
        Debug.Log("게임 데이터가 저장되었습니다.");
    }

    public void LoadGame() {
        player.isScenePlaying = false;
        player.LoadPlayerPosition();
        ItemDataManager.Instance.LoadCurrentMapData();
        SceneManager.LoadSceneData();
        currentMap.LoadMapData();
        passFirstScene();
        Debug.Log("게임 데이터가 로드되었습니다.");
    }



    public void BGOn(int number) {
        if (BGs == null || BGs.Length == 0) {
            Debug.LogError("BGs 배열이 초기화되지 않았습니다. Unity Inspector에서 BGs 배열을 설정하세요.");
            return;
        }

        if (number < 0 || number >= BGs.Length) {
            Debug.LogError($"잘못된 인덱스: {number}. BGs 배열의 범위를 확인하세요.");
            return;
        }

        foreach (var chooseAll in BGs) {
            if (chooseAll != null && chooseAll.gameObject.activeSelf) {
                chooseAll.gameObject.SetActive(false);
            }
        }

        BGs[number].gameObject.SetActive(true);
    }
}
