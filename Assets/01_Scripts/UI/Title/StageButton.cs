using System;
using Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneMgr = UnityEngine.SceneManagement.SceneManager;
namespace _01_Scripts.UI.Title
{
    public class StageButton : MonoBehaviour
    {
        [SerializeField] private int stage;
        [SerializeField] private PositionDataSO positionData;
        [SerializeField] private GameObject lockedObj;

        private readonly string[] _stageKeys =
        {
            "HalfCut",
            "Alive",
            "DSDF"
        };

        private void Awake()
        {
            UpdateButton();
        }

        private void UpdateButton()
        {
            if (AchiveManager.instance == null) return;
            if (AchiveManager.instance.IsAchiveCleared(_stageKeys[stage - 1]))
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
}