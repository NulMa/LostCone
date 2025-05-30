using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;

public class HiddenWall : MonoBehaviour{
    Tilemap tilemap;
    TilemapRenderer tilemapRenderer;

    private string saveKey; // 상태를 저장할 키

    private void Awake() {
        tilemap = GetComponent<Tilemap>();
        tilemapRenderer = GetComponent<TilemapRenderer>();

        // 고유한 저장 키 생성 (오브젝트 이름 + 고유 ID)
        saveKey = $"{gameObject.name}_{transform.position.x}_{transform.position.y}_{transform.position.z}";

        // 저장된 상태 불러오기
        if (PlayerPrefs.HasKey(saveKey)) {
            bool isHidden = PlayerPrefs.GetInt(saveKey) == 1;
            if (isHidden) {
                tilemap.gameObject.SetActive(false); // 저장된 상태가 비활성화라면 비활성화
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            // TilemapRenderer의 Material을 통해 투명도 조정
            
            if (tilemapRenderer != null && tilemapRenderer.material != null) {
                tilemapRenderer.material.DOFade(0f, 1f).OnComplete(() => {
                    tilemap.gameObject.SetActive(false);

                    // 상태 저장
                    PlayerPrefs.SetInt(saveKey, 1); // 1은 비활성화 상태를 의미
                    PlayerPrefs.Save();
                });
            }
            else {
                Debug.LogWarning("TilemapRenderer 또는 Material이 설정되지 않았습니다.");
            }
        }
    }
}
