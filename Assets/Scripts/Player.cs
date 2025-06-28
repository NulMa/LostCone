using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Player : MonoBehaviour{
    public float speed;
    public float jumpForce;

    public Vector2 inputVec2;
    public Vector3 moveDirection;

    public bool isJumping;
    public bool isScenePlaying;
    public bool isInteracting;
    private bool isMoveSfxPlaying = false;

    public GameObject cone;

    Animator anim;
    SpriteRenderer sprite;
    Rigidbody2D rigid;

    // �뽬 ���� ����
    public float dashMultiplier = 2.5f; // �뽬 �� �ӵ� ��� (2~3 ���� ��õ)
    public float dashDuration = 0.2f;   // �뽬 ���� �ð�(��)
    public float dashCooldown = 1f;     // �뽬 ��Ÿ��(��)
    private bool isDashing = false;
    private bool canDash = true;

    // �������� ���� ����
    public float downJumpRayLength = 0.2f; // �ٴ� Ž���� ���� ����
    public LayerMask platformLayer;         // ���� �ٴ� ���̾� ����
    private Collider2D playerCollider;


    void Start() {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
    }
    private void FixedUpdate() {
        if (isScenePlaying)
            return;
        CheckLanding();

        if (inputVec2.x == 0) {
            moveDirection = Vector3.zero;
            anim.SetBool("isMove", false);
        }
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    public Vector3 Position => transform.position; // ���� ��ġ ��ȯ

    public void SavePlayerPosition() {
        PlayerPrefs.SetFloat("Player_Pos_X", transform.position.x);
        PlayerPrefs.SetFloat("Player_Pos_Y", transform.position.y);
        PlayerPrefs.SetFloat("Player_Pos_Z", transform.position.z);
        PlayerPrefs.Save();
    }

    public void LoadPlayerPosition() {
        float x = PlayerPrefs.GetFloat("Player_Pos_X", transform.position.x);
        float y = PlayerPrefs.GetFloat("Player_Pos_Y", transform.position.y);
        float z = PlayerPrefs.GetFloat("Player_Pos_Z", transform.position.z);
        transform.position = new Vector3(x, y, z);
    }

    public void sceneSwitch() {
        isScenePlaying = isScenePlaying ? false : true;
    }
    public void jumpSwtich() {
        isJumping = isJumping ? false : true;
    }
    void CheckLanding() {
        if (!isJumping)
            return;

        Collider2D col = GetComponent<Collider2D>();
        Vector2 rayStart = new Vector2(col.bounds.center.x, col.bounds.min.y);
        float rayLength = col.bounds.extents.y + 0.05f;

        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, 0.1f, LayerMask.GetMask("Tilemap"));

        if (hit.collider != null) {
            anim.SetBool("isJump", false);
            isJumping = false;

            // ���� �� �¿� �Է��� ������ �ȱ� ���� ���
            if (inputVec2.x != 0 && !isMoveSfxPlaying) {
                AudioManager.Instance?.PlayLoopSFX(0, gameObject);
                isMoveSfxPlaying = true;
            }
        }
    }
    public void OnDash() {
        if (!canDash || isDashing || isScenePlaying) return;
        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine() {
        isDashing = true;
        canDash = false;
        float originalSpeed = speed;
        speed *= dashMultiplier;
        anim.SetTrigger("Dash"); // �뽬 �ִϸ��̼� Ʈ����(�ɼ�)

        yield return new WaitForSeconds(dashDuration);

        speed = originalSpeed;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void OnDownJump() {
        Debug.Log("OnDJ");
        if (isScenePlaying) return;

        // �Ʒ��� ����ĳ��Ʈ
        Vector2 origin = new Vector2(transform.position.x, playerCollider.bounds.min.y - 0.05f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, downJumpRayLength, platformLayer);
        if (hit.collider != null) {
            StartCoroutine(DownJumpRoutine(hit.collider));
        }
    }

    private IEnumerator DownJumpRoutine(Collider2D platform) {
        // �÷��̾� �ݶ��̴��� ��� ��Ȱ��ȭ(Ȥ�� PlatformEffector2D ���� oneWay ����)
        Physics2D.IgnoreCollision(playerCollider, platform, true);
        yield return new WaitForSeconds(0.3f); // ��� �ð�
        Physics2D.IgnoreCollision(playerCollider, platform, false);
    }

    public void OnMove(InputValue value) {
        if (isScenePlaying)
            return;

        inputVec2 = value.Get<Vector2>();
        if (inputVec2.x != 0) {
            anim.SetBool("isSwing", false);
            moveDirection = new Vector3(inputVec2.x, 0, inputVec2.y);
            anim.SetBool("isMove", true);

            // ���� ���� �ƴϰ�, ���尡 ��� ���� �ƴϸ� �ȱ� ���� ���� ���
            if (!isJumping && !isMoveSfxPlaying) {
                AudioManager.Instance?.PlayLoopSFX(0, gameObject);
                isMoveSfxPlaying = true;
            }
        }
        else {
            // �Է��� ������ �ȱ� ���� ����
            if (isMoveSfxPlaying) {
                AudioManager.Instance?.StopLoopSFX(gameObject);
                isMoveSfxPlaying = false;
            }
            moveDirection = Vector3.zero;
            anim.SetBool("isMove", false);
        }
        flipCtrl();
    }
    public void OnJump() {
        if (isScenePlaying)
            return;

        if (!anim.GetBool("isJump")) {
            anim.SetBool("isJump", true);
            rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // ���� �� �ȱ� ���� ����
            if (isMoveSfxPlaying) {
                AudioManager.Instance?.StopLoopSFX(gameObject);
                isMoveSfxPlaying = false;
            }
        }
    }
    public void OnFire() {
        isInteracting = true;
        StartCoroutine(ResetInteracting());
    }

    private IEnumerator ResetInteracting() {
        yield return new WaitForSeconds(0.1f);
        isInteracting = false;
    }


    void flipCtrl() {
        if (inputVec2.x > 0) {
            sprite.flipX = false;
        }
        else if (inputVec2.x < 0) {
            sprite.flipX = true;
        }
    }

    public void SwingAnim() {
        int swing = Random.Range(0, 5);
        if (swing == 0)
            anim.SetBool("isSwing", true);
    }
    public void StopSwingAnim() {
        int keepSwing = Random.Range(0, 3);
        if (keepSwing != 0)
            anim.SetBool("isSwing", false);
    }

    public void produPlayerMove(bool dirRight) {
        sprite.flipX = !dirRight;
        bool animMove = !anim.GetBool("isMove");
        anim.SetBool("isMove", animMove);
    }

}
