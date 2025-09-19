using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour{
    public float speed;
    public float jumpForce;

    // --- Crouch 추가 (원본 복구) ---
    [Header("Crouch Settings")] [SerializeField] float crouchSpeedMultiplier = 0.5f; // 앉기 이동 속도 배수
    [SerializeField] bool instantCrouchAnim = true; // 즉시 전환 옵션 (Play 강제)
    bool isCrouching; // 현재 앉은 상태
    float baseSpeed;  // 원래 속도 저장
    [Header("Crouch State Names")] [SerializeField] string crouchIdleStateName = "CrouchIdle"; // Animator 상태 이름
    [SerializeField] string crouchWalkStateName = "CrouchWalk"; // Animator 상태 이름
    int crouchIdleHash; int crouchWalkHash; // 해시 캐시
    bool hasCrouchIdle; bool hasCrouchWalk; bool loggedCrouchStateWarning; // 상태 존재 여부

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

    // ===== 착지 / 점프 안정화 추가 변수 =====
    [Header("Ground Check")]
    [SerializeField] LayerMask groundMask; // Tilemap + OneWayPlatform 등 포함
    [SerializeField] Transform groundCheck; // 발 위치 기준 Transform (없으면 자동 생성)
    [SerializeField] float groundCheckRadius = 0.12f;
    bool isGrounded;              // 현재 지상 여부
    bool wasGroundedLastFrame;    // 이전 프레임 지상 여부
    bool isDroppingThrough;       // 아래점프 중 여부

    // (옵션) 점프 버퍼 & 코요테 타임 (필요시 조정)
    [SerializeField] float jumpBufferTime = 0.1f;
    [SerializeField] float coyoteTime = 0.1f;
    float jumpBufferCounter;
    float coyoteCounter;

    void Start() {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        baseSpeed = speed; // 속도 기준 저장
        if (groundCheck == null) {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = new Vector3(0f, -playerCollider.bounds.extents.y, 0f);
            groundCheck = gc.transform;
        }
        ValidateCrouchStates(); // 복구
    }

    private void FixedUpdate() {
        if (isScenePlaying || isDashing) return;

        UpdateGroundedState();
        HandleBufferedJump();

        baseSpeed = speed; // 매 프레임 기본 속도 복구
        // InputManager로부터 받은 입력으로 이동 처리
        float appliedSpeed = isCrouching ? baseSpeed * crouchSpeedMultiplier : baseSpeed;
        rigid.velocity = new Vector2(inputVec2.x * appliedSpeed, rigid.velocity.y);
    }

    void Update() {
        if (isScenePlaying) return;

        HandleInput();
        UpdateCrouchState();
        UpdateLocomotionAnim();
    }

    void HandleInput() {
        // InputManager 싱글톤이 준비되었는지 확인
        if (InputManager.instance == null) return;

        // 이동 입력
        inputVec2 = InputManager.instance.MoveInput;
        flipCtrl();

        // 점프 입력
        if (InputManager.instance.IsJumpPressed) {
            if (!isCrouching) { // 앉은 상태에서는 점프 금지
                jumpBufferCounter = jumpBufferTime;
                if ((isGrounded || coyoteCounter > 0f) && !isJumping) {
                    ExecuteJump();
                    jumpBufferCounter = 0f;
                }
            }
        }

        // 상호작용 입력 (OnFire)
        if (InputManager.instance.IsInteractionPressed) {
            isInteracting = true;
            StartCoroutine(ResetInteracting());
        }

        // 대시 입력
        if (InputManager.instance.IsDashPressed) {
            OnDash();
        }

        // 아래점프 입력
        if (InputManager.instance.IsDownJumpPressed) {
            OnDownJump();
        }
    }

    void UpdateLocomotionAnim(){
        bool isMoving = Mathf.Abs(rigid.velocity.x) > 0.1f;

        if (isCrouching){
            if (isMoveSfxPlaying){
                AudioManager.Instance?.StopLoopSFX(gameObject);
                isMoveSfxPlaying = false;
            }
            if (instantCrouchAnim && (hasCrouchIdle || hasCrouchWalk)){
                string target = isMoving ? crouchWalkStateName : crouchIdleStateName;
                int targetHash = isMoving ? crouchWalkHash : crouchIdleHash;
                AnimatorStateInfo st = anim.GetCurrentAnimatorStateInfo(0);
                if (!st.IsName(target)) {
                    if ((isMoving && hasCrouchWalk) || (!isMoving && hasCrouchIdle))
                        anim.Play(targetHash, 0, 0f);
                }
            } else {
                anim.SetBool("isMove", false);
                anim.SetBool("CrouchIdle", !isMoving);
                anim.SetBool("CrouchWalk", isMoving);
            }
        } else {
            if (instantCrouchAnim){
                anim.SetBool("CrouchIdle", false);
                anim.SetBool("CrouchWalk", false);
                anim.SetBool("isMove", isMoving);
            } else {
                anim.SetBool("CrouchIdle", false);
                anim.SetBool("CrouchWalk", false);
                anim.SetBool("isMove", isMoving);
            }
            bool wantWalkSound = isMoving && isGrounded && !isJumping;
            if (wantWalkSound && !isMoveSfxPlaying){
                AudioManager.Instance?.PlayLoopSFX(0, gameObject);
                isMoveSfxPlaying = true;
            }
            if (!wantWalkSound && isMoveSfxPlaying){
                AudioManager.Instance?.StopLoopSFX(gameObject);
                isMoveSfxPlaying = false;
            }
        }
    }

    void UpdateCrouchState() {
        bool wantCrouch = inputVec2.y < -0.5f && isGrounded && !isDashing;
        if (wantCrouch != isCrouching) {
            isCrouching = wantCrouch;
        }
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

    // ===== 새 Ground 체크 로직 =====
    void UpdateGroundedState() {
        wasGroundedLastFrame = isGrounded;

        if (isDroppingThrough) {
            isGrounded = false;
        } else {
            if (groundCheck != null) {
                isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
            } else {
                Vector2 rayStart = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.min.y);
                float rayLength = playerCollider.bounds.extents.y + 0.05f;
                isGrounded = Physics2D.Raycast(rayStart, Vector2.down, rayLength, groundMask);
            }
        }

        if (isGrounded) coyoteCounter = coyoteTime; else coyoteCounter -= Time.fixedDeltaTime;

        if (!wasGroundedLastFrame && isGrounded) {
            anim.SetBool("isJump", false);
            isJumping = false;
            if (inputVec2.x != 0 && !isMoveSfxPlaying) {
                AudioManager.Instance?.PlayLoopSFX(0, gameObject);
                isMoveSfxPlaying = true;
            }
        }
    }

    void HandleBufferedJump() {
        if (jumpBufferCounter > 0f) {
            jumpBufferCounter -= Time.fixedDeltaTime;
            if ((isGrounded || coyoteCounter > 0f) && !isJumping) {
                ExecuteJump();
                jumpBufferCounter = 0f;
            }
        }
    }

    void ExecuteJump() {
        isJumping = true;
        isGrounded = false;
        anim.SetBool("isJump", true);
        rigid.velocity = new Vector2(rigid.velocity.x, 0f);
        rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        if (isMoveSfxPlaying) {
            AudioManager.Instance?.StopLoopSFX(gameObject);
            isMoveSfxPlaying = false;
        }
    }

    public void OnDash() {
        if (!canDash || isDashing || isScenePlaying) return;
        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine() {
        isDashing = true;
        canDash = false;
        anim.SetTrigger("Dash");
        
        // 대시 중에는 Rigidbody 속도를 직접 제어
        float originalGravity = rigid.gravityScale;
        rigid.gravityScale = 0;
        Vector2 dashVelocity = new Vector2(sprite.flipX ? -1 : 1, 0) * speed * dashMultiplier;
        rigid.velocity = dashVelocity;

        yield return new WaitForSeconds(dashDuration);

        rigid.velocity = Vector2.zero;
        rigid.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void OnDownJump() {
        if (isScenePlaying || !isGrounded) return;

        Vector2 origin = new Vector2(transform.position.x, playerCollider.bounds.min.y - 0.05f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, downJumpRayLength, platformLayer);
        if (hit.collider != null) {
            StartCoroutine(DownJumpRoutine(hit.collider));
        }
    }

    private IEnumerator DownJumpRoutine(Collider2D platform) {
        isDroppingThrough = true;
        Physics2D.IgnoreCollision(playerCollider, platform, true);
        yield return new WaitForSeconds(0.3f);
        Physics2D.IgnoreCollision(playerCollider, platform, false);
        yield return new WaitForFixedUpdate();
        isDroppingThrough = false;
    }

    private IEnumerator ResetInteracting() {
        yield return new WaitForSeconds(0.1f);
        isInteracting = false;
    }

    void flipCtrl() {
        if (inputVec2.x > 0) {
            sprite.flipX = false;
        } else if (inputVec2.x < 0) {
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
            case "IdleWolf":
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

    void OnDrawGizmosSelected() {
        if (groundCheck != null) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    void ValidateCrouchStates(){
        crouchIdleHash = Animator.StringToHash(crouchIdleStateName);
        crouchWalkHash = Animator.StringToHash(crouchWalkStateName);
        hasCrouchIdle = anim != null && anim.HasState(0, crouchIdleHash);
        hasCrouchWalk = anim != null && anim.HasState(0, crouchWalkHash);
        if ((!hasCrouchIdle || !hasCrouchWalk) && !loggedCrouchStateWarning){
            Debug.LogWarning($"[Player] 지정한 Crouch 애니 상태를 찾지 못했습니다. Idle:{crouchIdleStateName} 존재:{hasCrouchIdle} / Walk:{crouchWalkStateName} 존재:{hasCrouchWalk}. Bool 파라미터 폴백 사용.");
            loggedCrouchStateWarning = true;
        }
    }
}
