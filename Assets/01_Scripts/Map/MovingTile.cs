using System.Collections;
using UnityEngine;

public class MovingTile : MonoBehaviour {
    public enum TileMoveType { Auto, OnPlayer }
    public TileMoveType moveType = TileMoveType.Auto;

    public Vector3 moveOffset;      // 이동할 끝점 좌표 (Inspector에서 입력)
    public float moveTime = 2f;     // 한 방향 이동 시간
    private Vector3 startPos;
    private Vector3 endPos;
    // private bool movingToEnd = true; // Unused
    private Coroutine moveCoroutine;

    private void Start() {
        startPos = transform.position;
        endPos = startPos + moveOffset;

        if (moveType == TileMoveType.Auto) {
            moveCoroutine = StartCoroutine(MoveRoutine());
        }
    }

    private IEnumerator MoveRoutine() {
        while (true) {
            yield return MoveOnce(startPos, endPos);
            yield return MoveOnce(endPos, startPos);
        }
    }

    private IEnumerator MoveOnce(Vector3 from, Vector3 to) {
        float t = 0f;
        while (t < 1f) {
            t += Time.deltaTime / moveTime;
            transform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }
    }

    private IEnumerator MoveCycle() {
        yield return MoveOnce(startPos, endPos);
        yield return MoveOnce(endPos, startPos);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            collision.transform.SetParent(transform);

            if (moveType == TileMoveType.OnPlayer && moveCoroutine == null) {
                moveCoroutine = StartCoroutine(PlayerMoveCycle());
            }
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

    private IEnumerator PlayerMoveCycle() {
        yield return MoveCycle();
        moveCoroutine = null;
    }
}