using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    public Button loadButton;
    public GameObject InGameUi; // InGame UI 오브젝트

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Player_Pos_X"))
        {
            loadButton.interactable = true; // 이어하기 버튼 활성화
        }
        else
        {
            loadButton.interactable = false; // 이어하기 버튼 비활성화
        }
        
        loadButton.interactable = true; 
    }

    
    public void NewGame() {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        // 업적 매니저가 존재한다면 업적 초기화
        if (AchiveManager.instance != null) {
            // 모든 업적을 미달성 상태로 초기화
            foreach (var achive in AchiveManager.instance.achives) {
                achive.isClear = false;
            }
            // 업적 UI 새로고침
            AchiveManager.instance.RefreshUI();
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("DefaultScene"); // Unity의 SceneManager 사용
    }

    public void ContinueGame() {
        // 이어하기: 씬을 먼저 로드 후 LoadGame() 호출
        Debug.Log("1");
        UnityEngine.SceneManagement.SceneManager.LoadScene("DefaultScene");
        StartCoroutine(LoadGameAfterSceneLoad());
    }

    public void settings(){
        if (SettingUI.instance != null){
            SettingUI.instance.SettingPanel.SetActive(true);
        }
    }

    private void Update()
    {
        if (InGameUi == null)
        {
            if (GameObject.Find("SettingUI") == null)
                return;
            else
                InGameUi = GameObject.Find("SettingUI");
        }
        return;
    }

    private System.Collections.IEnumerator LoadGameAfterSceneLoad()
    {
        while (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("MainScene"))
        {
            yield return null;
        }

        // GamaManager.Instance가 null인지 확인
        if (GamaManager.Instance != null)
        {
            GamaManager.Instance.LoadGame();
        }
        else
        {
            Debug.LogError("GamaManager.Instance가 null입니다. GamaManager가 DefaultScene에 포함되어 있는지 확인하세요.");
        }

        yield return null;
    }
    public void gameExit() {
        Application.Quit();
    }
}