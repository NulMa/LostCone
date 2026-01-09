using _01_Scripts.UI.Title;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneMgr = UnityEngine.SceneManagement.SceneManager;

    public static class Defines
    {
        public static string[] StageKeys =
        {
            "HalfCut",
            "Alive",
            "GarakutaKun_0_Clear",
            "Unknown"
        };
    }

    public class StageButton : MonoBehaviour
    {
        [SerializeField] private int stage;
        [SerializeField] private PositionDataSO positionData;
        [SerializeField] private GameObject lockedObj;

        private void Awake()
        {
            Debug.Log(" Check Stage3 :  " + PlayerPrefs.GetInt("GarakutaKun_0_Clear"));
            UpdateButton();
        }

        private void UpdateButton()
        {
            if (AchiveManager.instance == null) return;

            if (AchiveManager.instance.IsAchiveCleared(Defines.StageKeys[stage - 1]))
            {
                lockedObj.SetActive(false);
            }
            if(PlayerPrefs.GetInt("GarakutaKun_0_Clear") == 1 && stage == 3)
            {
                lockedObj.SetActive(false);
            }
    }

        public void OnClick()
        {
            SceneMgr.sceneLoaded += HandleSceneLoaded;
            SceneMgr.LoadScene("DefaultScene2");
        }

        private void HandleSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Debug.Assert(GamaManager.Instance != null, $"게임 매니저가 없습니다.");
            GamaManager.Instance.ignoreSavePos = true;
            GamaManager.Instance.player.transform.position = positionData.position;
            SceneMgr.sceneLoaded -= HandleSceneLoaded;
        }
    }
