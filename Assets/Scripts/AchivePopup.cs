using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class AchivePopup : MonoBehaviour {
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Start() {
        rectTransform = GetComponent<RectTransform>();

        // 앵커/피벗이 우하단(1,0)으로 설정되어 있어야 함
        rectTransform.anchorMin = new Vector2(1f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 0f);
        rectTransform.pivot = new Vector2(1f, 0f);

        // 시작 위치 설정 (anchoredPosition 기준)
        rectTransform.anchoredPosition = new Vector2(-50f, -200f);

        // 이동 트윈
        rectTransform.DOAnchorPos(new Vector2(-50f, 0f), 1f).OnComplete(() => {
            StartCoroutine(FadeOutRoutine());
        });

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutRoutine() {
        yield return new WaitForSeconds(2f);
        canvasGroup.DOFade(0f, 1f);
    }
}