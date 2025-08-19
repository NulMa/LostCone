using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;

public class UICtrl : MonoBehaviour{

    public TextMeshProUGUI Message;
    public TextMeshProUGUI Stage;
    public TextMeshProUGUI Items; // This is the text we will animate

    public void PrintMSG(string msg) {
        Message.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = msg;
    }

    private void Update() {
        Stage.text = "Stage : " + (GamaManager.Instance.currentStageID + 1);
        Items.text = GamaManager.Instance.ItemDataManager.itemCount + " / " + GamaManager.Instance.ItemDataManager.bools.Length;
    }

    public void changeKey(string keyName) {
        //Message.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = keyName;
        
    }

    // New method to animate the Items text
    public void AnimateItemsTextOnFirstCollect()
    {
        Debug.Log("[UICtrl] AnimateItemsTextOnFirstCollect() called.");
        // Ensure Items TextMeshProUGUI is assigned
        if (Items == null)
        {
            Debug.LogWarning("UICtrl: Items TextMeshProUGUI is not assigned. Cannot animate.");
            return;
        }

        // Get the DOTweenAnimation component attached to the Items TextMeshProUGUI
        DOTweenAnimation dotweenAnimation = Items.GetComponent<DOTweenAnimation>();
        if (dotweenAnimation != null)
        {
            dotweenAnimation.DORestart(); // Restart the animation defined in the Inspector
            Debug.Log("[UICtrl] Playing DOTweenAnimation on Items text.");
        }
        else
        {
            Debug.LogWarning("UICtrl: DOTweenAnimation component not found on Items TextMeshProUGUI. Please add it in the Inspector.");
        }
    }
}
