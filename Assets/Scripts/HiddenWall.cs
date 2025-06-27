using UnityEngine.Tilemaps;
using UnityEngine;
using DG.Tweening;

public enum HiddenWallType { OnContact, OnFunction }

public class HiddenWall : MonoBehaviour {
    public HiddenWallType type = HiddenWallType.OnContact;
    public int wallId; // Inspector���� ���� ��ȣ �Ҵ�

    Tilemap tilemap;
    TilemapRenderer tilemapRenderer;
    private string saveKey;

    private void Awake() {
        tilemap = GetComponent<Tilemap>();
        tilemapRenderer = GetComponent<TilemapRenderer>();

        // Ÿ�԰� ���� ��ȣ�� ������ ���� Ű ����
        saveKey = $"HiddenWall_{type}_{wallId}";

        // ����� ���� �ҷ�����
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

    // �ܺο��� ȣ���� �Լ�
    public void HideWallByFunction() {
        if (type != HiddenWallType.OnFunction) return;
        HideWall();
    }

    private void HideWall() {
        if (tilemapRenderer != null && tilemapRenderer.material != null) {
            tilemapRenderer.material.DOFade(0f, 1f).OnComplete(() => {
                tilemap.gameObject.SetActive(false);
                PlayerPrefs.SetInt(saveKey, 1);
                PlayerPrefs.Save();
            });
        }
        else {
            Debug.LogWarning("TilemapRenderer �Ǵ� Material�� �������� �ʾҽ��ϴ�.");
        }
    }
}