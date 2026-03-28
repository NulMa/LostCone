using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Core;

public class GifVideo : MonoBehaviour
{

    private Image img;
    private SpriteRenderer sprite;

    public int VideoNum; // 간단한 분기 번호
    public GameObject[] videoObject; // 0: 대상 (Animator), 추가 슬롯은 확장용

    public Sprite[] sprites; // 필요시 여러 스프라이트 배열

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
            var playerPos = new PositionData(GamaManager.Instance.player.transform.position);
            SaveManager.Instance.Save<PositionData>(playerPos, "playerPos");
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
                Debug.Log("GifVideo: VideoNum 2 실행");
                //sprite's
                // 0 = fuze_fixed
                // 1 = opened_gate
                // 2 = power_on
                // 3 = power off
                // 4 = closed_gate

                // 0 garakutaKun0 change animation_normal
                videoObject[0].GetComponent<Animator>().Play("Normal_Idle");

                // 1 change garakuataKun1 change animation_fixed
                videoObject[1].GetComponent<Animator>().enabled = true;

                // 2 change fuze sprite
                videoObject[2].GetComponent<SpriteRenderer>().sprite = sprites[0]; 

                // 3 change KANDEN gate open
                videoObject[3].GetComponent<SpriteRenderer>().sprite = sprites[1];
                videoObject[3].GetComponent<BoxCollider2D>().enabled = false;
                // 4 change KANDEN POWER sprite
                videoObject[4].GetComponent<SpriteRenderer>().sprite = sprites[2];

                // 5 change NEKO gate open
                videoObject[5].GetComponent<SpriteRenderer>().sprite = sprites[1];
                videoObject[5].GetComponent<BoxCollider2D>().enabled = false;
                videoObject[5].GetComponent<Animator>().enabled = false;

                // 6 change NEKO POWER sprite
                videoObject[6].GetComponent<SpriteRenderer>().sprite = sprites[3];

                // 7 change NEKO back gate close
                videoObject[7].GetComponent<SpriteRenderer>().sprite = sprites[4];
                break;
            case 3:
                videoObject[0].gameObject.SetActive(true);
                videoObject[1].gameObject.SetActive(false);
                videoObject[2].gameObject.SetActive(false);
                Debug.Log("GifVideo: VideoNum 3 실행");
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

