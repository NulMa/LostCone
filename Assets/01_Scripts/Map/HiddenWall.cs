using UnityEngine.Tilemaps;
using UnityEngine;
using DG.Tweening;

public enum HiddenWallType { OnContact, OnFunction }

public class HiddenWall : MonoBehaviour {
    public HiddenWallType type = HiddenWallType.OnContact;
    public int wallId; // Inspector에서 벽의 번호 할당

    Tilemap tilemap;
    TilemapRenderer tilemapRenderer;
    TilemapCollider2D tilemapCollider;

    private string saveKey;
    
    private void Awake() {
        tilemap = GetComponent<Tilemap>();
        tilemapRenderer = GetComponent<TilemapRenderer>();
        tilemapCollider = GetComponent<TilemapCollider2D>();

        // 타입과 벽의 번호를 조합한 저장 키 생성
        saveKey = $"HiddenWall_{type}_{wallId}";

        // 저장된 상태 불러오기
        if (PlayerPrefs.HasKey(saveKey)) {
            bool isHidden = PlayerPrefs.GetInt(saveKey) == 1;
            if (isHidden) {
                tilemap.gameObject.SetActive(false);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (type != HiddenWallType.OnContact) return;
        if (collision.gameObject.CompareTag("Player")) {
            HideWall();
        }
    }
    
    // 외부에서 호출할 함수
    public void HideWallByFunction() {
        if (type != HiddenWallType.OnFunction) return;
        HideWall();
    }

    private void HideWall() {
        if (tilemapRenderer != null && tilemapRenderer.material != null) {
            tilemapCollider.enabled = false;
            tilemapRenderer.material.DOFade(0f, 1f).OnComplete(() => {
                tilemap.gameObject.SetActive(false);
                PlayerPrefs.SetInt(saveKey, 1);
                PlayerPrefs.Save();
            });
        }
        else {
            Debug.LogWarning("TilemapRenderer 또는 Material이 설정되지 않았습니다.");
        }
    }
}