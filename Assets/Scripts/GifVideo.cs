using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GifVideo : MonoBehaviour{
    Image img;
    SpriteRenderer sprite;


    private void Awake() {
        img = GetComponent<Image>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update(){
        img.sprite = sprite.sprite;

    }

    public void offVideo() {
        if (img != null) {
            // Image 컴포넌트가 있는 경우 DOTween으로 투명도 조정
            img.DOFade(0f, 1f).OnComplete(() => gameObject.SetActive(false));
        }
        else {
            Debug.LogWarning("Image 또는 SpriteRenderer가 없습니다.");
            gameObject.SetActive(false); // 안전하게 비활성화
        }
    }
}
