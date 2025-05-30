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
            // Image ������Ʈ�� �ִ� ��� DOTween���� ���� ����
            img.DOFade(0f, 1f).OnComplete(() => gameObject.SetActive(false));
        }
        else {
            Debug.LogWarning("Image �Ǵ� SpriteRenderer�� �����ϴ�.");
            gameObject.SetActive(false); // �����ϰ� ��Ȱ��ȭ
        }
    }
}
