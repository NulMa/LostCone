using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    public Button loadButton;

    private void Awake() {
        if (PlayerPrefs.HasKey("Player_Pos_X")) {
            loadButton.interactable = true; // 이어하기 버튼 활성화
        }
        else {
            loadButton.interactable = false; // 이어하기 버튼 비활성화
        }
    }

    public void NewGame() {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("DefaultScene"); // Unity의 SceneManager 사용
    }

    public void ContinueGame() {
        // 이어하기: 게임 씬 로드 후 LoadGame() 호출
        Debug.Log("1");
        UnityEngine.SceneManagement.SceneManager.LoadScene("DefaultScene");
        StartCoroutine(LoadGameAfterSceneLoad());
    }

    private System.Collections.IEnumerator LoadGameAfterSceneLoad() {
        while (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("DefaultScene")) {
            yield return null;
        }

        // GamaManager.Instance가 null인지 확인
        if (GamaManager.Instance != null) {
            GamaManager.Instance.LoadGame();
        }
        else {
            Debug.LogError("GamaManager.Instance가 null입니다. GamaManager가 DefaultScene에 포함되어 있는지 확인하세요.");
        }
    }
    public void gameExit() {
        Application.Quit();
    }
}