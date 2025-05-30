using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;

public class HiddenWall : MonoBehaviour{
    Tilemap tilemap;
    TilemapRenderer tilemapRenderer;

    private string saveKey; // ���¸� ������ Ű

    private void Awake() {
        tilemap = GetComponent<Tilemap>();
        tilemapRenderer = GetComponent<TilemapRenderer>();

        // ������ ���� Ű ���� (������Ʈ �̸� + ���� ID)
        saveKey = $"{gameObject.name}_{transform.position.x}_{transform.position.y}_{transform.position.z}";

        // ����� ���� �ҷ�����
        if (PlayerPrefs.HasKey(saveKey)) {
            bool isHidden = PlayerPrefs.GetInt(saveKey) == 1;
            if (isHidden) {
                tilemap.gameObject.SetActive(false); // ����� ���°� ��Ȱ��ȭ��� ��Ȱ��ȭ
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            // TilemapRenderer�� Material�� ���� ���� ����
            
            if (tilemapRenderer != null && tilemapRenderer.material != null) {
                tilemapRenderer.material.DOFade(0f, 1f).OnComplete(() => {
                    tilemap.gameObject.SetActive(false);

                    // ���� ����
                    PlayerPrefs.SetInt(saveKey, 1); // 1�� ��Ȱ��ȭ ���¸� �ǹ�
                    PlayerPrefs.Save();
                });
            }
            else {
                Debug.LogWarning("TilemapRenderer �Ǵ� Material�� �������� �ʾҽ��ϴ�.");
            }
        }
    }
}
