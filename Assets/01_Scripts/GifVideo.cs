using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GifVideo : MonoBehaviour
{
    private Image img;
    private SpriteRenderer sprite;

    public int VideoNum; // 간단한 분기 번호
    public GameObject[] videoObject; // 0: 대상 (Animator), 추가 슬롯은 확장용

    Player player => GamaManager.Instance.player;

    private void Awake()
    {
        img = GetComponent<Image>();
        sprite = GetComponent<SpriteRenderer>();
        player.isScenePlaying = true;
    }

    private void Update()
    {
        //GamaManager.Instance.player.isScenePlaying = true;


        if (img != null && sprite != null)
            img.sprite = sprite.sprite;
    }

    public void offVideo()
    {
        if (img != null)
        {
            img.DOFade(0f, 1f).OnComplete(() => gameObject.SetActive(false));
        }
        else
        {
            Debug.LogWarning("GifVideo: Image 또는 SpriteRenderer가 없습니다.");
            gameObject.SetActive(false);
        }
    }
    // Animation Event 에서 호출 (필요시 애니메이션 이벤트로 직접 연결)
    public void ifNeed()
    {
        if (videoObject == null || videoObject.Length == 0) return;

        switch (VideoNum)
        {
            case 0:
                break;      // 예약
            case 1:         // ikimono function activate.
                videoObject[0].GetComponent<Animator>().SetTrigger("Untied");

                break;
            case 2:
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        player.isScenePlaying = false;
    }
}

