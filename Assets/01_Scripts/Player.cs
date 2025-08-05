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

    // 대시 관련 변수
    public float dashMultiplier = 2.5f; // 대시 시 속도 배수 (2~3 정도 추천)
    public float dashDuration = 0.2f;   // 대시 지속 시간(초)
    public float dashCooldown = 1f;     // 대시 쿨타임(초)
    private bool isDashing = false;
    private bool canDash = true;

    // 아래점프 관련 변수
    public float downJumpRayLength = 0.2f; // 바닥 탐지용 레이 길이
    public LayerMask platformLayer;         // 통과 바닥 레이어 마스크
    private Collider2D playerCollider;
    private Vector3 dashDirection; // 대시 방향 저장변수


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

        if (isDashing) {
            transform.position += dashDirection * speed * Time.deltaTime;
            return;
        }

        // 입력값에 따른 moveDirection을 항상 저장
        if (inputVec2.x != 0) {
            moveDirection = new Vector3(inputVec2.x, 0, inputVec2.y);
        }
        else {
            moveDirection = Vector3.zero;
        }

        // 애니메이션된 입력값에 따른 움직임 설정
        anim.SetBool("isMove", inputVec2.x != 0);

        transform.position += moveDirection * speed * Time.deltaTime;
    }

    public Vector3 Position => transform.position; // 현재 위치 반환

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

            // 착지 시 좌우 입력이 있으면 걷기 사운드 재생
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
        dashDirection = moveDirection; // 대시 방향을 미리 저장
        speed *= dashMultiplier;
        anim.SetTrigger("Dash");

        yield return new WaitForSeconds(dashDuration);

        speed = originalSpeed;
        isDashing = false;

        // 대시 종료 후 입력값에 따른 moveDirection을 다시 설정
        if (inputVec2.x != 0) {
            Debug.Log("dash move");
            moveDirection = new Vector3(inputVec2.x, 0, inputVec2.y);
            anim.SetBool("isMove", true);
        }
        else {
            moveDirection = Vector3.zero;
            Debug.Log("dash stop");
            anim.SetBool("isMove", false);
        }

        // 애니메이터에서 Exit 상태가 자동으로 전환됨

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void OnDownJump() {
        Debug.Log("OnDJ");
        if (isScenePlaying) return;

        // 아래점프 레이캐스트
        Vector2 origin = new Vector2(transform.position.x, playerCollider.bounds.min.y - 0.05f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, downJumpRayLength, platformLayer);
        if (hit.collider != null) {
            StartCoroutine(DownJumpRoutine(hit.collider));
        }
    }

    private IEnumerator DownJumpRoutine(Collider2D platform) {
        // 플레이어 콜라이더와 바닥 비활성화(혹은 PlatformEffector2D 사용시 oneWay 설정)
        Physics2D.IgnoreCollision(playerCollider, platform, true);
        yield return new WaitForSeconds(0.3f); // 잠깐 시간
        Physics2D.IgnoreCollision(playerCollider, platform, false);
    }

    public void OnMove(InputValue value) {
        if (isScenePlaying)
            return;

        inputVec2 = value.Get<Vector2>(); // 항상 입력값 저장

        if (isDashing) {
            // 대시 중에는 방향만 사용, flip만 사용
            flipCtrl();
            return;
        }

        if (inputVec2.x != 0) {
            anim.SetBool("isSwing", false);
            moveDirection = new Vector3(inputVec2.x, 0, inputVec2.y);
            anim.SetBool("isMove", true);

            if (!isJumping && !isMoveSfxPlaying) {
                AudioManager.Instance?.PlayLoopSFX(0, gameObject);
                isMoveSfxPlaying = true;
            }
        }
        else {
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

            // 점프 시 걷기 사운드 정지
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

    private void OnTriggerEnter2D(Collider2D collision) {
        switch (collision.name) {
            case "eineUmb":
                GamaManager.Instance.achiveCall("Nachtmusik");
                break;

            case "WearwolfHiddenWall":
                GamaManager.Instance.achiveCall("MacGuffin");
                break;

            case "FlamingoHerd":
                GamaManager.Instance.achiveCall("FlaFla");
                break;

            case "cran&pan":
                GamaManager.Instance.achiveCall("HotMeal");
                break;

            case "Gumi":
                GamaManager.Instance.achiveCall("YourName");
                break;
        }
    }
}
