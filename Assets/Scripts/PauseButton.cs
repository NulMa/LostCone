using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    private SettingUI settingUI;

    void Awake() {
        // 자동 할당
        settingUI = SettingUI.instance;
    }

    public void OnPauseButtonClicked() {
        if (settingUI != null)
            settingUI.SettingPanel.SetActive(true);
    }
}
