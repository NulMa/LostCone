using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SceneData {
    public GameObject scene; // ¾À GameObject
    public bool isDone;      // ¾À Àç»ý ¿Ï·á ¿©ºÎ
}
public class SceneManager : MonoBehaviour{
    public SceneData[] scenes; // ¾À µ¥ÀÌÅÍ ¹è¿­

    private void Awake() {
        LoadSceneData();
    }

    public void playScene(int sceneNum) {
        if (scenes[sceneNum].scene == null)
            return;

        scenes[sceneNum].scene.SetActive(true);
        GamaManager.Instance.player.isScenePlaying = true;
        GamaManager.Instance.player.GetComponent<Animator>().SetBool("isMove", false);
        GamaManager.Instance.player.inputVec2 = Vector2.zero;
    }

    public void sceneIsDone(int sceneNum) {
        if (scenes[sceneNum].scene == null)
            return;
        Debug.Log("isdone");
        scenes[sceneNum].isDone = true;
        SaveSceneData();
        GamaManager.Instance.player.isScenePlaying = false;
    }

    public void SaveSceneData() {
        for (int i = 0; i < scenes.Length; i++) {
            PlayerPrefs.SetInt($"Scene_{i}_isDone", scenes[i].isDone ? 1 : 0);
        }
        PlayerPrefs.Save();
        Debug.Log("Scene data saved.");
    }

    public void LoadSceneData() {
        for (int i = 0; i < scenes.Length; i++) {
            scenes[i].isDone = PlayerPrefs.GetInt($"Scene_{i}_isDone", 0) == 1;
        }
        Debug.Log("Scene data loaded.");
    }
}
