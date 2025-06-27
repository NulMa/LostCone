using System.Collections;
using UnityEngine;

public class MovingTile : MonoBehaviour {
    public Vector3 moveOffset;      // 이동할 상대 좌표 (Inspector에서 입력)
    public float moveTime = 2f;     // 한 방향 이동 시간
    private Vector3 startPos;
    private Vector3 endPos;
    private bool movingToEnd = true;

    private void Start() {
        startPos = transform.position;
        endPos = startPos + moveOffset;
        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine() {
        while (true) {
            Vector3 from = movingToEnd ? startPos : endPos;
            Vector3 to = movingToEnd ? endPos : startPos;
            float t = 0f;
            while (t < 1f) {
                t += Time.deltaTime / moveTime;
                transform.position = Vector3.Lerp(from, to, t);
                yield return null;
            }
            movingToEnd = !movingToEnd;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            if (GamaManager.Instance != null && GamaManager.Instance.playerParent != null)
                collision.transform.SetParent(GamaManager.Instance.playerParent.transform);
            else
                collision.transform.SetParent(null);
        }
    }
}