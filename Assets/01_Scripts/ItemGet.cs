using UnityEngine;
using DG.Tweening;

public class ItemGet : MonoBehaviour {
    Collider2D coll;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer; // [추가] 스프라이트 렌더러 참조

    public int StageID; // 스테이지 번호
    public int ItemID; // 아이템 ID
    public bool isHave; // 보유 상태

    private void Awake() {
        if (isHave) {
            gameObject.SetActive(false);
        }

        coll = GetComponent<Collider2D>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // [추가] 컴포넌트 가져오기

        // Tweening: 위아래로 반복 이동
        transform.DOMoveY(transform.position.y + 0.25f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            AudioManager.Instance?.PlayUISFX(1); // 아이템 획득 사운드 재생
            DOTween.Kill(transform);
            isHave = true;
            gameObject.SetActive(false);

            Debug.Log("[ItemGet] Player collided with item. Calling GamaManager.OnItemCollected().");
            // [수정] OnItemCollected 호출 시, 자신의 스프라이트를 인자로 전달
            GamaManager.Instance.OnItemCollected(spriteRenderer.sprite);
        }
    }
}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemGet : MonoBehaviour{
    Collider2D coll;
    Rigidbody2D rigid;

    public int StageID;
    public int ItemID;
    public bool isHave;

    private void Awake() {
        if(isHave)
            gameObject.SetActive(false);


        coll = GetComponent<Collider2D>();
        rigid = GetComponent<Rigidbody2D>();

        //tweening move, up down repeat
        transform.DOMoveY(transform.position.y + 0.25f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Debug.Log("아이템을 획득했습니다!");
            DOTween.Kill(transform);
            isHave = true;
            gameObject.SetActive(false);
        }
    }

}
*/