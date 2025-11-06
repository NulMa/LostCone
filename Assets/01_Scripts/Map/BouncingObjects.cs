using UnityEngine;

public class BouncingObject : MonoBehaviour {
    public float speed = 5f; // 기본 이동 속도
    public float rotationSpeed = 360f; // 회전 속도 (도/초)
    private Vector2 direction; // 방향 벡터
    private Rigidbody2D rb; // Rigidbody2D
    private Camera mainCamera;

    SpriteRenderer sprite;

    private void Start() {
        sprite = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        if (rb == null) {
            Debug.LogError("Rigidbody2D가 필요합니다. Rigidbody2D를 추가하세요.");
            return;
        }

        // 초기 이동 방향 설정 (랜덤)
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        // 초기 속도 설정
        rb.linearVelocity = direction * speed;
    }

    private void FixedUpdate() {
        // 화면 경계를 넘어갈 경우 방향 변경 및 속도 조정
        CheckBounds();

        // 회전 추가
        // 회전 추가 (회전 속도 범위 제한)
        rotationSpeed = Mathf.Clamp(rotationSpeed, 50, 200f); // 최소 50, 최대 200
        rb.MoveRotation(rb.rotation + rotationSpeed * Time.fixedDeltaTime);
    }

    private void CheckBounds() {
        Vector3 currentPosition = transform.position;

        // 화면 경계 계산
        Vector3 minBounds = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 maxBounds = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        // 화면 경계를 넘어갈 경우 방향 변경
        if (currentPosition.x < minBounds.x || currentPosition.x > maxBounds.x) {
            direction.x *= -1;
            AdjustSpeed();
            rb.linearVelocity = direction * speed;
        }
        if (currentPosition.y < minBounds.y || currentPosition.y > maxBounds.y) {
            direction.y *= -1;
            AdjustSpeed();
            rb.linearVelocity = direction * speed;
        }
    }

    private void AdjustSpeed() {
        // 속도에 -2에서 2 사이의 랜덤 값 추가
        speed += Random.Range(-2f, 2f);

        // 속도 범위 제한
        speed = Mathf.Clamp(speed, 2f, 9f); // 최소 2, 최대 9

        sprite.flipX =  Random.Range(0, 2) == 0; // 스프라이트 X축 반전
        sprite.flipY = Random.Range(0, 2) == 0; // 스프라이트 Y축 반전
    }
}