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

    void Start() {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
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

            // 착지 후 좌우 입력이 있으면 걷기 사운드 재생
            if (inputVec2.x != 0 && !isMoveSfxPlaying) {
                AudioManager.Instance?.PlayLoopSFX(0, gameObject);
                isMoveSfxPlaying = true;
            }
        }
    }

    public void OnMove(InputValue value) {
        if (isScenePlaying)
            return;

        inputVec2 = value.Get<Vector2>();
        if (inputVec2.x != 0) {
            anim.SetBool("isSwing", false);
            moveDirection = new Vector3(inputVec2.x, 0, inputVec2.y);
            anim.SetBool("isMove", true);

            // 점프 중이 아니고, 사운드가 재생 중이 아니면 걷기 사운드 루프 재생
            if (!isJumping && !isMoveSfxPlaying) {
                AudioManager.Instance?.PlayLoopSFX(0, gameObject);
                isMoveSfxPlaying = true;
            }
        }
        else {
            // 입력이 없으면 걷기 사운드 정지
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

}
