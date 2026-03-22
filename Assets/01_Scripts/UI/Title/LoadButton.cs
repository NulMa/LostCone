using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts.UI.Title
{
    public class LoadButton : MonoBehaviour
    {
        [SerializeField] private GameObject popup;
        [SerializeField] private GameObject stageCanvas;

        Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            if (!AchiveManager.instance.IsAchiveCleared("Begin"))
            {
                button.interactable = false;
            }
            else
            {
                button.interactable = true;
            }
        }
        public void OnClick()
        {
            popup.SetActive(true);
        }

        public void SelectStage()
        {
            popup?.SetActive(false);
            stageCanvas?.SetActive(true);
        }
        
        public void ContinueGame() {
            UnityEngine.SceneManagement.SceneManager.LoadScene(MainMenuManager.DEFAULT_SCENE);
            StartCoroutine(LoadGameAfterSceneLoad());
        }
        
        private System.Collections.IEnumerator LoadGameAfterSceneLoad()
        {
            while (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("MainScene"))
            {
                yield return null;
            }

            if (GamaManager.Instance != null)
            {
                GamaManager.Instance.ignoreSavePos = false;
                GamaManager.Instance.LoadGame();
            }
            else
            {
                Debug.LogError("GamaManager.Instance가 null입니다. GamaManager가 DefaultScene에 포함되어 있는지 확인하세요.");
            }

            yield return null;
        }
    }
}