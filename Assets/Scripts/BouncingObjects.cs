using UnityEngine;

public class BouncingObject : MonoBehaviour {
    public float speed = 5f; // �⺻ �̵� �ӵ�
    public float rotationSpeed = 360f; // ȸ�� �ӵ� (��/��)
    private Vector2 direction; // �̵� ����
    private Rigidbody2D rb; // Rigidbody2D ������Ʈ
    private Camera mainCamera;

    SpriteRenderer sprite;

    private void Start() {
        sprite = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        if (rb == null) {
            Debug.LogError("Rigidbody2D�� �ʿ��մϴ�. Rigidbody2D�� �߰��ϼ���.");
            return;
        }

        // �ʱ� �̵� ���� ���� (����)
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        // �ʱ� �ӵ� ����
        rb.velocity = direction * speed;
    }

    private void FixedUpdate() {
        // ȭ�� ��踦 ����� ���� ����
        CheckBounds();

        // ȸ�� �߰�
        // ȸ�� �߰� (ȸ�� �ӵ� ���� ����)
        rotationSpeed = Mathf.Clamp(rotationSpeed, 50, 200f); // �ּ� 50, �ִ� 200
        rb.MoveRotation(rb.rotation + rotationSpeed * Time.fixedDeltaTime);
    }

    private void CheckBounds() {
        Vector3 currentPosition = transform.position;

        // ī�޶��� ȭ�� ��� ���
        Vector3 minBounds = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 maxBounds = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        // ȭ�� ��踦 ����� ���� ���� �� �ӵ� ����
        if (currentPosition.x < minBounds.x || currentPosition.x > maxBounds.x) {
            direction.x *= -1;
            AdjustSpeed();
            rb.velocity = direction * speed;
        }
        if (currentPosition.y < minBounds.y || currentPosition.y > maxBounds.y) {
            direction.y *= -1;
            AdjustSpeed();
            rb.velocity = direction * speed;
        }
    }

    private void AdjustSpeed() {
        // �ӵ��� ��2�� ���� ���� �߰�
        speed += Random.Range(-2f, 2f);

        // �ӵ��� �ʹ� �����ų� ������ �ʵ��� ����
        speed = Mathf.Clamp(speed, 2f, 9f); // �ּ� 2, �ִ� 9

        sprite.flipX =  Random.Range(0, 2) == 0; // �������� ��������Ʈ ����
        sprite.flipY = Random.Range(0, 2) == 0; // �������� ��������Ʈ ����
    }
}