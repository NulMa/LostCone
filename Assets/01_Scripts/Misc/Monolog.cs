using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Localization.Settings;
using UnityEngine.InputSystem;

public class Monolog : MonoBehaviour {
    TextMeshProUGUI textMeshProUGUI;
    public float time = 2f; // 대기 시간(초)
    public float typeDuration = 1f; // 타이핑 효과 시간
    public float showDuration = 3f; // 표시 지속 시간
    public float fadeDuration = 2f; // 페이드아웃 시간

    private Coroutine talkCoroutine;
    private Tween fadeTween;
    private bool isShowing = false;

    private void Awake() {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    private void Update() {
        // 플레이어 정지시 시작
        if (GamaManager.Instance.player.inputVec2 == Vector2.zero && !isShowing) {
            ShowIdleTalk();
        }
        // 움직일 시 독백 즉시 중단 및 빠른 페이드아웃 후 초기화
        if (isShowing && GamaManager.Instance.player.inputVec2 != Vector2.zero) {
            StopTalkImmediate();
        }
    }

    public void ShowIdleTalk()
    {
        StopTalkImmediate();
        talkCoroutine = StartCoroutine(IdleTalkRoutine());
    }

    private IEnumerator IdleTalkRoutine() {
        isShowing = true;
        textMeshProUGUI.text = "";
        textMeshProUGUI.color = new Color(textMeshProUGUI.color.r, textMeshProUGUI.color.g, textMeshProUGUI.color.b, 1f);

        // time만큼 대기(나중에 스킵하면 빠른 구간)
        float elapsed = 0f;
        while (elapsed < time) {
            if (GamaManager.Instance.player.inputVec2 != Vector2.zero) {
                isShowing = false;
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 텍스트 표시
        string localizedString = LocalizationSettings.StringDatabase.GetLocalizedString("New Table", "Idle_talk");
        textMeshProUGUI.DOText(localizedString, typeDuration);
        GamaManager.Instance.achiveCall("Suspense");

        // 텍스트 표시(나중에 스킵하면 즉시 종료)
        float showElapsed = 0f;
        while (showElapsed < showDuration + typeDuration) {
            if (GamaManager.Instance.player.inputVec2 != Vector2.zero) {
                StopTalkImmediate();
                yield break;
            }
            showElapsed += Time.deltaTime;
            yield return null;
        }

        // 페이드아웃
        fadeTween = textMeshProUGUI.DOFade(0f, fadeDuration);
        yield return fadeTween.WaitForCompletion();

        isShowing = false;
    }

    private void StopTalkImmediate() {
        if (talkCoroutine != null) {
            StopCoroutine(talkCoroutine);
            talkCoroutine = null;
        }
        fadeTween?.Kill();
        textMeshProUGUI.text = "";
        textMeshProUGUI.color = new Color(textMeshProUGUI.color.r, textMeshProUGUI.color.g, textMeshProUGUI.color.b, 0f);
        isShowing = false;
    }
}