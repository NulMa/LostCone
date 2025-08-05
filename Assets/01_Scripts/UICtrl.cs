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
    public TextMeshProUGUI Items;

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
}
