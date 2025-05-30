using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    public Button loadButton;

    private void Awake() {
        if (PlayerPrefs.HasKey("Player_Pos_X")) {
            loadButton.interactable = true; // �̾��ϱ� ��ư Ȱ��ȭ
        }
        else {
            loadButton.interactable = false; // �̾��ϱ� ��ư ��Ȱ��ȭ
        }
    }

    public void NewGame() {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("DefaultScene"); // Unity�� SceneManager ���
    }

    public void ContinueGame() {
        // �̾��ϱ�: ���� �� �ε� �� LoadGame() ȣ��
        Debug.Log("1");
        UnityEngine.SceneManagement.SceneManager.LoadScene("DefaultScene");
        StartCoroutine(LoadGameAfterSceneLoad());
    }

    private System.Collections.IEnumerator LoadGameAfterSceneLoad() {
        while (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("DefaultScene")) {
            yield return null;
        }

        // GamaManager.Instance�� null���� Ȯ��
        if (GamaManager.Instance != null) {
            GamaManager.Instance.LoadGame();
        }
        else {
            Debug.LogError("GamaManager.Instance�� null�Դϴ�. GamaManager�� DefaultScene�� ���ԵǾ� �ִ��� Ȯ���ϼ���.");
        }
    }
    public void gameExit() {
        Application.Quit();
    }
}