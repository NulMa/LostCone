using UnityEngine;
using DG.Tweening;

public class ItemGet : MonoBehaviour {
    Collider2D coll;
    Rigidbody2D rigid;

    public int StageID; // �� ��ȣ
    public int ItemID; // ������ ID
    public bool isHave; // ���� ����

    private void Awake() {
        if (isHave) {
            gameObject.SetActive(false);
        }

        coll = GetComponent<Collider2D>();
        rigid = GetComponent<Rigidbody2D>();

        // Tweening: ���Ʒ��� �ݺ� �̵�
        transform.DOMoveY(transform.position.y + 0.25f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            AudioManager.Instance?.PlayUISFX(1); // ������ ȹ�� ���� ���
            DOTween.Kill(transform);
            isHave = true;
            GamaManager.Instance.ItemDataManager.SaveCurrentMapData();
            GamaManager.Instance.SaveGame();
            gameObject.SetActive(false);
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
            Debug.Log("�������� ȹ���߽��ϴ�!");
            DOTween.Kill(transform);
            isHave = true;
            gameObject.SetActive(false);
        }
    }

}
*/