using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Blade.SoundSystem;
using PaperFlower.Core;
using UnityEngine.Localization.Settings;
using DG.Tweening;

public enum AchiveType {
    Lemon,
    DonutHole,
    Junk,
    Common
}

[Serializable]
public class AchiveData {
    public string key;         // 업적 고유 키 (로컬화용)
    public bool isClear;       // 클리어 여부
    public AchiveType type;    // 업적 타입
    
    // [추가] 업적 아이콘을 저장할 Sprite 타입의 필드
    public Sprite icon;        
    public Sprite orgIcon;     // [추가] 원본 아이콘 (상세 보기용)

    // [추가] 업적 애니메이션을 저장할 필드
    public AnimationClip achiveClip;
    public AnimationClip orgClip;  // [추가] 원본 애니메이션 (상세 보기용)

    public AchiveData(string key, AchiveType type) {
        this.key = key;
        this.type = type;
        this.icon = null; 
        this.orgIcon = null;
        this.achiveClip = null;
        this.orgClip = null;
        isClear = false;
    }
}

[DefaultExecutionOrder(-10)]
public class AchiveManager : MonoBehaviour {
    public static AchiveManager instance;
    public List<AchiveData> achives = new List<AchiveData>();
    public GameObject achivePopup;
    public Canvas Canvas; 

    public RuntimeAnimatorController baseAnimatorController; 

    public Slider achiveSlider; 
    public TextMeshProUGUI achivePercentText; 

    public Transform achiveListParent; 
    public GameObject achiveItemPrefab; 

    public SoundSO achiveSound;

    [Header("Developer Easter Egg UI")]
    public GameObject devEggPanel;      // 사진 조각 패널
    public Image[] photoPieces;         // 3개의 조각 이미지 (인덱스 0, 1, 2)
    public Image fullPhoto;             // 합쳐진 완성 사진 이미지
    public CanvasGroup devEggCanvasGroup; // 페이드 효과용

    [Header("Achive Detail UI")]
    public GameObject detailPanel;      // 상세 보기 패널
    public Image detailIconImage;       // 상세 보기용 큰 아이콘
    public Animator detailAnimator;     // 상세 보기용 큰 애니메이터
    public TextMeshProUGUI detailNameText; // 상세 보기용 이름
    public TextMeshProUGUI detailDescText; // 상세 보기용 설명
    
    private readonly PlaySoundEvent _playSoundEvent = new PlaySoundEvent();

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAchives();
            RefreshUI();

            // 개발자 이스터에그 UI 초기화
            if (devEggPanel != null) devEggPanel.SetActive(false);
            if (fullPhoto != null) fullPhoto.gameObject.SetActive(false);

            // 상세 보기 패널 초기화
            if (detailPanel != null) detailPanel.SetActive(false);
        }
        else {
            Destroy(gameObject);
        }
    }

    public void AchiveSort()
    {

    }

    // 업적 달성 설정 (외부에서 호출)
    public void SetAchiveClear(string key) {
        var achive = achives.Find(a => a.key == key);
        if (achive != null && !achive.isClear) {
            achive.isClear = true;
            SaveAchives();
            Debug.Log($"[AchiveManager] 업적 '{achive.key}' 달성!");
            GameEventBus.RaiseEvent(_playSoundEvent.Initialize(achiveSound));

            // 업적 팝업 생성 (Canvas의 부모로)
            if (achivePopup != null && Canvas != null){
                GameObject popupObj = Instantiate(achivePopup, Canvas.transform);

                // 1번째 자식의 TextMeshProUGUI에 업적 이름 입력
                if (popupObj.transform.childCount > 1){
                    Transform child = popupObj.transform.GetChild(1);
                    var tmp = child.GetComponent<TMPro.TextMeshProUGUI>();
                    if (tmp != null){
                        string name = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.key + "_name");
                        tmp.text = name;
                    }
                }
            }
        }
    }

    // 업적 저장
    public void SaveAchives() {
        foreach (var achive in achives) {
            PlayerPrefs.SetInt("achive_" + achive.key, achive.isClear ? 1 : 0);
        }
        PlayerPrefs.Save();
    }
// 기존 UI 오브젝트 삭제
    public void LoadAchives() {
        foreach (var achive in achives) {
            achive.isClear = PlayerPrefs.GetInt("achive_" + achive.key, 0) == 1;
        }
    }

    // 업적 UI 새로고침
    public void RefreshUI() {
        if (achiveListParent == null || achiveItemPrefab == null)
            return;

        // 기존 UI 오브젝트 삭제
        foreach (Transform child in achiveListParent)
            Destroy(child.gameObject);

        int clearCount = 0;
        foreach (var achive in achives) {
            // 프리팹 생성
            GameObject go = Instantiate(achiveItemPrefab, achiveListParent);
            
            // [추가 시작] 클릭 이벤트 연결
            Button btn = go.GetComponent<Button>();
            if (btn == null) btn = go.AddComponent<Button>();
            
            AchiveData currentData = achive; // 람다 캡처용
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => ShowAchiveDetail(currentData));
            // [추가 끝]

            // 컴포넌트 찾아오기
            var texts = go.GetComponentsInChildren<TextMeshProUGUI>();
            var image = go.GetComponent<Image>();

            // [수정 시작] IconImage 컴포넌트를 이름으로 찾아옵니다.
            Image iconImage = null;
            Animator animator = null;
            Transform iconTransform = go.transform.Find("IconImage");
            if (iconTransform != null) {
                iconImage = iconTransform.GetComponent<Image>();
                animator = iconTransform.GetComponent<Animator>();
            }
            // [수정 끝]

            if (image != null) {
                image.color = achive.isClear ? Color.white : new Color(0.6f, 0.6f, 0.6f);
            }

            if (texts.Length >= 3) {
                if (achive.isClear) {
                    // 업적 클리어 시
                    string name = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.key + "_name");
                    string desc = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.key + "_desc");
                    string location = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.type.ToString());
                    texts[0].text = name;
                    texts[1].text = desc;
                    texts[2].text = location;
                    clearCount++;

                    // [수정 시작] 클리어했다면 아이콘을 찾아 표시하고 애니메이션을 할당합니다.
                    if (iconImage != null && achive.icon != null) {
                        iconImage.sprite = achive.icon;
                        iconImage.color = Color.white;
                    }

                    if (animator != null && achive.achiveClip != null && baseAnimatorController != null) {
                        // AnimatorOverrideController를 사용하여 클립을 동적으로 교체
                        AnimatorOverrideController overrideController = new AnimatorOverrideController(baseAnimatorController);
                        
                        // 기본 컨트롤러의 첫 번째 애니메이션 클립 이름을 가져와 교체
                        var clips = baseAnimatorController.animationClips;
                        if (clips.Length > 0) {
                            overrideController[clips[0].name] = achive.achiveClip;
                        }

                        animator.runtimeAnimatorController = overrideController;
                        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
                        animator.enabled = true;
                    }
                    else if (animator != null) {
                        animator.enabled = false;
                    }
                    // [수정 끝]
                }
                else {
                    // 업적 미달성 시
                    string location = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", achive.type.ToString());
                    texts[0].text = "???";
                    texts[1].text = "???";
                    texts[2].text = location;

                    // [수정 시작] 클리어하지 않았다면 아이콘과 애니메이션을 숨깁니다.
                    if (iconImage != null) {
                        iconImage.sprite = null;
                        iconImage.color = new Color(1, 1, 1, 0);
                    }
                    if (animator != null) animator.enabled = false;
                }
            }
        }

        achiveSlider.value = clearCount / (float)achives.Count;
        achivePercentText.text = $"{clearCount} / {achives.Count}";
    }
    
    public bool IsAchiveCleared(string key) {
        var achive = achives.Find(a => a.key == key);
        return achive != null && achive.isClear;
    }

    // ==========================================
    // 개발자 이스터에그 전용 로직
    // ==========================================

    public void CollectDevEggPiece(int stageId) {
        if (devEggPanel == null) {
            Debug.LogError("[AchiveManager] devEggPanel이 할당되지 않았습니다!");
            return;
        }

        // 1. 조각 수집 상태 저장
        PlayerPrefs.SetInt($"DevEgg_Piece_{stageId}", 1);
        PlayerPrefs.Save();

        // 2. UI 표시 초기화
        devEggPanel.SetActive(true);
        if (devEggCanvasGroup != null) {
            devEggCanvasGroup.alpha = 0;
            devEggCanvasGroup.DOFade(1f, 0.5f).SetUpdate(true);
        }

        for (int i = 0; i < photoPieces.Length; i++) {
            if (photoPieces[i] != null) {
                photoPieces[i].gameObject.SetActive(i == (stageId - 1));
                photoPieces[i].color = Color.white;
                photoPieces[i].transform.localScale = Vector3.one;
            }
        }
        
        if (fullPhoto != null) fullPhoto.gameObject.SetActive(false);

        // 3. 모든 조각 수집 여부 체크
        bool isPiece1Collected = PlayerPrefs.GetInt("DevEgg_Piece_1", 0) == 1;
        bool isPiece2Collected = PlayerPrefs.GetInt("DevEgg_Piece_2", 0) == 1;
        bool isPiece3Collected = PlayerPrefs.GetInt("DevEgg_Piece_3", 0) == 1;

        if (isPiece1Collected && isPiece2Collected && isPiece3Collected) {
            PerformMergeAnimation();
        } else {
            DOVirtual.DelayedCall(3f, CloseDevEggPanel).SetUpdate(true);
        }
    }

    private void PerformMergeAnimation() {
        if (fullPhoto == null) {
            Debug.LogError("[AchiveManager] fullPhoto가 할당되지 않았습니다!");
            return;
        }

        Sequence seq = DOTween.Sequence().SetUpdate(true);
        seq.AppendInterval(1.5f);
        seq.AppendCallback(() => {
            fullPhoto.gameObject.SetActive(true);
            fullPhoto.color = new Color(1, 1, 1, 0);
        });
        seq.Append(fullPhoto.DOFade(1f, 1f));
        seq.Join(photoPieces[2].DOFade(0f, 1f));
        seq.AppendInterval(3f);
        seq.AppendCallback(() => {
            SetAchiveClear("DevEgg"); // 최종 업적 달성
            CloseDevEggPanel();
        });
    }

    public void CloseDevEggPanel() {
        if (devEggCanvasGroup == null) return;
        devEggCanvasGroup.DOFade(0f, 0.5f).SetUpdate(true).OnComplete(() => {
            devEggPanel.SetActive(false);
        });
    }

    // ==========================================
    // 업적 상세 보기 로직
    // ==========================================

    public void ShowAchiveDetail(AchiveData data) {
        if (!data.isClear || detailPanel == null) return;

        detailPanel.SetActive(true);
        
        // 텍스트 설정
        if (detailNameText != null) 
            detailNameText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", data.key + "_name");
        if (detailDescText != null) 
            detailDescText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Achive", data.key + "_desc");

        // [수정] 원본 리소스가 있다면 우선 사용, 없다면 기존 리소스 사용 (Fallback)
        AnimationClip targetClip = data.orgClip != null ? data.orgClip : data.achiveClip;
        Sprite targetIcon = data.orgIcon != null ? data.orgIcon : data.icon;

        bool hasAnimation = targetClip != null && baseAnimatorController != null;

        // 1. 일반 이미지 설정 (애니메이션이 없을 때만)
        if (detailIconImage != null) {
            detailIconImage.gameObject.SetActive(!hasAnimation);
            if (!hasAnimation && targetIcon != null) {
                detailIconImage.sprite = targetIcon;
            }
        }

        // 2. 애니메이터 설정
        if (detailAnimator != null) {
            detailAnimator.gameObject.SetActive(false);
            
            // [수정] Image와 SpriteRenderer 모두 초기화하여 잔상 제거
            Image animImage = detailAnimator.GetComponent<Image>();
            if (animImage != null) animImage.sprite = null;
            
            SpriteRenderer animSR = detailAnimator.GetComponent<SpriteRenderer>();
            if (animSR != null) animSR.sprite = null;

            if (hasAnimation) {
                detailAnimator.gameObject.SetActive(true);
                detailAnimator.enabled = false;
                detailAnimator.runtimeAnimatorController = null;

                // 새로운 OverrideController 생성
                AnimatorOverrideController overrideController = new AnimatorOverrideController(baseAnimatorController);
                
                // [개선] GetOverrides를 사용하여 모든 애니메이션 슬롯을 타겟 클립으로 정확히 교체
                List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                overrideController.GetOverrides(overrides);
                for (int i = 0; i < overrides.Count; i++) {
                    overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, targetClip);
                }
                overrideController.ApplyOverrides(overrides);

                detailAnimator.runtimeAnimatorController = overrideController;
                detailAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
                detailAnimator.enabled = true;

                // 상태 완전 초기화 및 강제 재생
                detailAnimator.Rebind();
                detailAnimator.Play(0, -1, 0f);
                detailAnimator.Update(0f);
            }
        }

        // 상세 패널 페이드 효과
        CanvasGroup cg = detailPanel.GetComponent<CanvasGroup>();
        if (cg != null) {
            cg.alpha = 0;
            cg.DOFade(1f, 0.3f).SetUpdate(true);
        }
    }

    public void CloseAchiveDetail() {
        if (detailPanel == null) return;
        
        CanvasGroup cg = detailPanel.GetComponent<CanvasGroup>();
        if (cg != null) {
            cg.DOFade(0f, 0.3f).SetUpdate(true).OnComplete(() => detailPanel.SetActive(false));
        } else {
            detailPanel.SetActive(false);
        }
    }
}
