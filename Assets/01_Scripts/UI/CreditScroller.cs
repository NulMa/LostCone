using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

// 특정 시점에 오브젝트를 활성화하는 설정
[System.Serializable]
public class CreditTimedActivation
{
    [Tooltip("활성화할 오브젝트")]
    public GameObject target;
    [Tooltip("스크롤 시작 후 몇 초 뒤에 활성화 (초 기반)")]
    public float triggerTime;

    private bool triggered;

    public void Reset() => triggered = false;

    public void TryTrigger(float elapsed)
    {
        if (triggered || target == null) return;
        if (elapsed >= triggerTime)
        {
            triggered = true;
            target.SetActive(true);
        }
    }
}

[RequireComponent(typeof(RectTransform))]
public class CreditScroller : MonoBehaviour
{
    [Header("Credit Text")]
    [Tooltip("크레딧 내용을 표시할 TMP Text 컴포넌트")]
    [SerializeField] private TMP_Text creditText;

    [Header("Outline")]
    [SerializeField] private Color outlineColor = Color.black;
    [SerializeField][Range(0f, 1f)] private float outlineWidth = 0.25f;

    [Header("Fade Edges")]
    [Tooltip("화면 상단 페이드 이미지 (Image 컴포넌트 할당)")]
    [SerializeField] private Image topFadeImage;
    [Tooltip("화면 하단 페이드 이미지 (Image 컴포넌트 할당)")]
    [SerializeField] private Image bottomFadeImage;
    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private float fadeSize = 120f;

    [Header("Screen Fade")]
    [Tooltip("전체 화면을 덮는 검정 Image의 CanvasGroup (씬 전환 페이드용)")]
    [SerializeField] private CanvasGroup screenFadeOverlay;
    [Tooltip("크레딧 시작 시 페이드 아웃 시간 (초)")]
    [SerializeField] private float fadeInDuration = 1.5f;
    [Tooltip("크레딧 종료 시 페이드 아웃 시간 (초)")]
    [SerializeField] private float fadeOutDuration = 1.5f;
    [Tooltip("크레딧 종료 후 이동할 씬 이름")]
    [SerializeField] private string nextSceneName = "DefaultScene";

    [Header("Spacing")]
    [Tooltip("역할명 바로 아래 이름 사이 간격 (px)")]
    [SerializeField] private float roleNameSpacing = 4f;
    [Tooltip("섹션 사이 간격 (px)")]
    [SerializeField] private float sectionSpacing = 40f;

    [Header("Scroll Settings")]
    [Tooltip("초당 이동하는 픽셀 수")]
    [SerializeField] private float scrollSpeed = 60f;
    [Tooltip("크레딧 시작 후 대기 시간")]
    [SerializeField] private float startDelay = 1.5f;
    [Tooltip("시작 위치 추가 오프셋 (양수 = 더 아래에서 시작)")]
    [SerializeField] private float startPositionOffset = 0f;
    [Tooltip("마지막 텍스트가 화면 위로 사라진 후 추가로 스크롤할 거리 (px)")]
    [SerializeField] private float endPadding = 400f;

    [Header("End Logo")]
    [Tooltip("크레딧 마지막에 표시할 로고 오브젝트 (CreditScroller와 같은 부모의 자식으로 배치)")]
    [SerializeField] private RectTransform endLogo;
    [Tooltip("텍스트 끝과 로고 사이 간격 (px)")]
    [SerializeField] private float logoSpacing = 80f;
    [Tooltip("로고 페이드 인 시간 (초)")]
    [SerializeField] private float logoFadeDuration = 1f;

    [Header("Timed Activations")]
    [Tooltip("스크롤 시작 후 특정 시간에 오브젝트를 활성화")]
    [SerializeField] private List<CreditTimedActivation> timedActivations = new();

    [Header("Events")]
    [Tooltip("크레딧 스크롤이 완료된 후 호출됩니다.")]
    public UnityEvent onCreditsFinished;

    private RectTransform rectTransform;
    private float endPositionY;
    private bool isScrolling = false;
    private float scrollElapsed = 0f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // 시작 시 오버레이를 완전히 불투명하게 (암전 상태)
        if (screenFadeOverlay != null)
        {
            screenFadeOverlay.alpha = 1f;
            screenFadeOverlay.blocksRaycasts = true;
        }
    }

    private IEnumerator Start()
    {
        yield return BuildCreditsText();

        ApplyOutline();
        SetupFadeImages();

        // TMP가 높이를 완전히 계산할 때까지 여러 프레임 대기
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(creditText.rectTransform);
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        // 텍스트 높이가 확정된 후 로고 배치
        PositionEndLogo();

        // 로고 포함 전체 콘텐츠 높이 최종 확정
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        endPositionY = rectTransform.rect.height + Screen.height + endPadding;
        rectTransform.anchoredPosition = new Vector2(
            rectTransform.anchoredPosition.x,
            -Screen.height - startPositionOffset
        );

        // 페이드 인 (암전 → 화면 표시)
        if (screenFadeOverlay != null)
        {
            screenFadeOverlay.DOFade(0f, fadeInDuration).OnComplete(() =>
            {
                screenFadeOverlay.blocksRaycasts = false;
            });
        }

        foreach (var activation in timedActivations)
            activation.Reset();

        yield return new WaitForSeconds(startDelay);
        isScrolling = true;

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(Locale locale)
    {
        StartCoroutine(BuildCreditsText());
    }

    private IEnumerator BuildCreditsText()
    {
        var handle = LocalizationSettings.StringDatabase.GetTableAsync("Credit");
        yield return handle;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("[CreditScroller] 로컬라이제이션 테이블 'Credit' 로드 실패");
            yield break;
        }

        var table = handle.Result;
        var sorted = table.Values
            .Where(e => e.SharedEntry != null)
            .OrderBy(e => e.SharedEntry.Id)
            .Where(e => !string.IsNullOrEmpty(e.Value))
            .ToList();

        string roleNameSep = $"\n<size={roleNameSpacing}> </size>";
        string sectionSep  = $"\n<size={sectionSpacing}> </size>";

        var sb = new StringBuilder();
        string prevKey = null;

        foreach (var entry in sorted)
        {
            string key = entry.SharedEntry.Key;
            if (sb.Length > 0)
            {
                bool isName      = key.Contains("_Name_");
                bool prevWasRole = prevKey != null && prevKey.Contains("_Role_");
                sb.Append(isName && prevWasRole ? roleNameSep : sectionSep);
            }
            sb.Append(entry.Value);
            prevKey = key;
        }

        creditText.text = sb.ToString();
    }

    private void PositionEndLogo()
    {
        if (endLogo == null) return;

        // 텍스트 아래에 로고를 배치 (앵커가 top-center 기준)
        // rect.yMin = 텍스트 하단 모서리 (로컬 좌표), 피벗 설정에 무관하게 정확함
        float textBottomY = creditText.rectTransform.anchoredPosition.y
                          + creditText.rectTransform.rect.yMin;
        endLogo.anchoredPosition = new Vector2(0f, textBottomY - logoSpacing);

        // 로고는 처음엔 투명하게 숨겨두고 스크롤 중 페이드 인
        var logoGroup = endLogo.GetComponent<CanvasGroup>();
        if (logoGroup == null)
            logoGroup = endLogo.gameObject.AddComponent<CanvasGroup>();
        logoGroup.alpha = 0f;
    }

    // 로고가 화면 중앙에 도달했을 때 페이드 인 (Update에서 호출)
    private bool logoShown = false;
    private void TryShowLogo()
    {
        if (logoShown || endLogo == null) return;

        // 로고의 월드 Y 위치가 화면 중앙 근처에 오면 페이드 인
        float logoScreenY = endLogo.position.y;
        if (logoScreenY <= Screen.height * 0.6f)
        {
            logoShown = true;
            var logoGroup = endLogo.GetComponent<CanvasGroup>();
            if (logoGroup != null)
                logoGroup.DOFade(1f, logoFadeDuration);
        }
    }

    private void ApplyOutline()
    {
        creditText.outlineColor = outlineColor;
        creditText.outlineWidth = outlineWidth;
    }

    private void SetupFadeImages()
    {
        if (topFadeImage != null)
        {
            topFadeImage.sprite = CreateVerticalGradient(topOpaque: true);
            topFadeImage.rectTransform.sizeDelta = new Vector2(
                topFadeImage.rectTransform.sizeDelta.x, fadeSize);
        }
        if (bottomFadeImage != null)
        {
            bottomFadeImage.sprite = CreateVerticalGradient(topOpaque: false);
            bottomFadeImage.rectTransform.sizeDelta = new Vector2(
                bottomFadeImage.rectTransform.sizeDelta.x, fadeSize);
        }
    }

    private Sprite CreateVerticalGradient(bool topOpaque)
    {
        const int h = 64;
        var tex = new Texture2D(1, h, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        for (int y = 0; y < h; y++)
        {
            float t = (float)y / (h - 1);
            float alpha = topOpaque ? (1f - t) : t;
            tex.SetPixel(0, y, new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha));
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, h), new Vector2(0.5f, 0.5f));
    }

    private void Update()
    {
        if (!isScrolling) return;

        scrollElapsed += Time.deltaTime;

        foreach (var activation in timedActivations)
            activation.TryTrigger(scrollElapsed);

        TryShowLogo();

        rectTransform.anchoredPosition += Vector2.up * (scrollSpeed * Time.deltaTime);

        if (rectTransform.anchoredPosition.y >= endPositionY)
        {
            isScrolling = false;
            StartCoroutine(FinishCredits());
        }
    }

    private IEnumerator FinishCredits()
    {
        onCreditsFinished?.Invoke();

        // 페이드 아웃 후 씬 전환
        if (screenFadeOverlay != null)
        {
            screenFadeOverlay.blocksRaycasts = true;
            screenFadeOverlay.DOFade(1f, fadeOutDuration);
            yield return new WaitForSeconds(fadeOutDuration);
        }

        if (!string.IsNullOrEmpty(nextSceneName))
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }

    public void SkipCredits()
    {
        if (!isScrolling) return;
        isScrolling = false;
        StartCoroutine(FinishCredits());
    }
}
