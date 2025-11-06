using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct BGSpeedData {
    public MeshRenderer meshRenderer;
    public float speed;          // 가로 패럴럭스 (텍스처 오프셋)
    public bool isAutoPlay;
    public bool useVertical;     // 세로 패럴럭스 사용 여부
    public float verticalFactor; // 플레이어 Y 변화 * factor
    public float maxYDeviation;  // 초기 local Y 기준 최대 편차
}

public class BGSpeedManage : MonoBehaviour {
    public Player player;
    public BGSpeedData[] bg;

    [Header("Vertical Parallax Base Y")]
    public bool useCustomBaseY;      // true면 customBaseY 사용
    public float customBaseY;        // 외부 지정 기준 Y

    private float previousPlayerX;    // 이전 프레임 플레이어 X (텍스처 스크롤용)
    private float initialPlayerY;     // 시작 시(또는 리셋 시) 기준 플레이어 Y
    private float[] initialLocalY;    // 각 BG 초기 local Y

    private void Start() {
        if (player != null) {
            previousPlayerX = player.transform.position.x;
            initialPlayerY  = player.transform.position.y;
        }
        CacheInitialLocalY();
    }

    private void FixedUpdate() {
        // 시간 기반 오토 스크롤 (isAutoPlay == true)
        ApplyAutoPlayScroll();
        if (player == null) return;
        BGRoll();
        ApplyVerticalParallax();
    }

    void CacheInitialLocalY() {
        if (bg == null) return;
        initialLocalY = new float[bg.Length];
        for (int i = 0; i < bg.Length; i++) {
            if (bg[i].meshRenderer != null) {
                initialLocalY[i] = bg[i].meshRenderer.transform.localPosition.y;
            }
        }
    }

    // isAutoPlay 레이어 자동 가로 스크롤 (시간 기반)
    void ApplyAutoPlayScroll() {
        if (bg == null) return;
        float dt = Time.fixedDeltaTime; // FixedUpdate와 동기화
        foreach (var data in bg) {
            if (!data.isAutoPlay) continue;
            if (data.meshRenderer == null) continue;

            var mr = data.meshRenderer;
            Vector2 offset = mr.material.mainTextureOffset;
            offset.x += 0.01f * data.speed * dt; // speed의 부호로 방향 결정
            mr.material.mainTextureOffset = offset;
        }
    }

    // 외부에서 기준 Y 재설정 (커스텀 모드 자동 활성)
    public void SetVerticalBaseY(float baseY) {
        customBaseY = baseY;
        useCustomBaseY = true;
        initialPlayerY = baseY; // 내부 계산에도 사용
    }

    // 가로 텍스처 스크롤
    public void BGRoll() {
        float currentPlayerX = player.transform.position.x;
        float deltaX = currentPlayerX - previousPlayerX;
        foreach (var bgData in bg) {
            if (bgData.isAutoPlay) continue;
            if (bgData.meshRenderer == null) continue;
            bgData.meshRenderer.material.mainTextureOffset += 0.01f * deltaX * bgData.speed * Vector2.right;
        }
        previousPlayerX = currentPlayerX;
    }

    // 세로 패럴럭스 (transform.localPosition 기반)
    void ApplyVerticalParallax() {
        if (initialLocalY == null || initialLocalY.Length != bg.Length) return;
        float currentRefY = useCustomBaseY ? customBaseY : initialPlayerY;
        float playerYDelta = player.transform.position.y - currentRefY;
        for (int i = 0; i < bg.Length; i++) {
            var data = bg[i];
            if (!data.useVertical) continue;
            if (data.meshRenderer == null) continue;

            float baseLocalY = initialLocalY[i];
            float offset = playerYDelta * data.verticalFactor;
            if (data.maxYDeviation > 0f) offset = Mathf.Clamp(offset, -data.maxYDeviation, data.maxYDeviation);
            Transform t = data.meshRenderer.transform;
            Vector3 lp = t.localPosition;
            lp.y = baseLocalY + offset;
            t.localPosition = lp;
        }
    }

    // 플레이어 현재 Y를 새 기준으로 사용 (커스텀 모드 해제)
    public void ResetVerticalBaseline() {
        if (player == null) return;
        useCustomBaseY = false;
        initialPlayerY = player.transform.position.y;
        CacheInitialLocalY();
    }
}
