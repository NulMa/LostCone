using UnityEngine;

public class DevEasterEgg : MonoBehaviour {
    [Header("Settings")]
    public int stageId; // 1, 2, 3 중 하나로 설정

    private bool isInteracted = false;

    private void OnTriggerStay2D(Collider2D collision) {
        if (isInteracted) return;

        if (collision.CompareTag("Player")) {
            Player player = collision.GetComponent<Player>();
            
            if (player != null && player.isInteracting) {
                isInteracted = true;
                if (AchiveManager.instance != null) {
                    AchiveManager.instance.CollectDevEggPiece(stageId);
                } else {
                    Debug.LogError("[DevEasterEgg] AchiveManager 인스턴스를 찾을 수 없습니다!");
                }
            }
        }
    }
}
