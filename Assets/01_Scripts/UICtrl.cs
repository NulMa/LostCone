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

    // [설정 필요] 애니메이션에 사용할 참조 변수들
    public GameObject itemIconPrefab;
    public RectTransform mainCanvas;
    public Transform arrivalTarget; // [추가] 아이콘의 최종 도착 지점

    public void PrintMSG(string msg) {
        Message.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = msg;
    }

    private void Start() {
        // 씬 시작 시 한 번만 현재 아이템 카운트를 반영
        UpdateItemCount(GamaManager.Instance.ItemDataManager.itemCount, GamaManager.Instance.ItemDataManager.bools.Length);
    }

    private void Update() {
        Stage.text = "Stage : " + (GamaManager.Instance.currentStageID + 1);
        // 매 프레임 카운트를 업데이트하는 코드를 제거하여 애니메이션 연출이 가능하도록 함
    }

    public void UpdateItemCount(int current, int total)
    {
        Items.text = $"{current} / {total}";
    }

    public void changeKey(string keyName) {
        //Message.GetComponent<LocalizeStringEvent>().StringReference.TableEntryReference = keyName;
    }

    // [수정됨] 요청에 따라 텍스트 애니메이션 로직 전체 변경
    public void AnimateItemsTextOnFirstCollect()
    {
        Debug.Log("[UICtrl] AnimateItemsTextOnFirstCollect() called for sequence animation.");
        if (Items == null) return;

        // 텍스트가 커지고, 숫자가 업데이트된 후, 다시 작아지는 시퀀스 애니메이션
        Sequence textSequence = DOTween.Sequence();
        textSequence.Append(Items.transform.DOScale(1.5f, 0.2f).SetEase(Ease.OutQuad)); // 1.5배 커지는 애니메이션
        textSequence.AppendCallback(() => {
            // 가장 커졌을 때 숫자를 업데이트
            UpdateItemCount(GamaManager.Instance.ItemDataManager.itemCount, GamaManager.Instance.ItemDataManager.bools.Length);
        });
        textSequence.Append(Items.transform.DOScale(1f, 0.15f).SetEase(Ease.InQuad)); // 원래 크기로 돌아오는 애니메이션
    }

    public void PlayItemAcquireAnimation(Vector3 playerWorldPosition, Sprite itemSprite)
    {
        if (itemIconPrefab == null || mainCanvas == null || arrivalTarget == null)
        {
            Debug.LogError("UICtrl: itemIconPrefab, mainCanvas, 또는 arrivalTarget이 설정되지 않았습니다. Inspector를 확인하세요.");
            return;
        }

        Vector2 startPosition = Camera.main.WorldToScreenPoint(playerWorldPosition);
        GameObject icon = Instantiate(itemIconPrefab, mainCanvas);
        icon.transform.position = startPosition;
        
        Image iconImage = icon.GetComponent<Image>();
        if (iconImage != null)
        {
            iconImage.sprite = itemSprite;
            iconImage.color = new Color(1, 1, 1, 0);
        }
        icon.transform.localScale = Vector3.zero;
        icon.SetActive(true);

        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(icon.transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutQuad));
        mySequence.Join(iconImage.DOFade(1f, 0.2f));
        mySequence.Append(icon.transform.DOScale(1f, 0.1f));

        // **연출 2: 포물선 비행**
        Vector3 endPos = arrivalTarget.position;
        Vector3 controlPoint = (icon.transform.position + endPos) / 2 + Vector3.up * 100f;
        mySequence.Append(icon.transform.DOPath(new Vector3[] { controlPoint, endPos }, 2, PathType.CatmullRom)
            .SetEase(Ease.InQuad));

        mySequence.Append(icon.transform.DOScale(1.5f, 0.2f));
        mySequence.Join(iconImage.DOFade(0f, 0.2f));

        mySequence.OnComplete(() =>
        {
            // [수정됨] AnimateItemsTextOnFirstCollect가 숫자 업데이트를 포함하므로, 여기서는 호출만 하면 됨
            AnimateItemsTextOnFirstCollect();

            // 연출이 끝난 아이콘은 파괴
            Destroy(icon);
        });
    }
}