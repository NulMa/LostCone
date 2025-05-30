using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct BGSpeedData {
    public MeshRenderer meshRenderer;
    public float speed;
    public bool isAutoPlay;
}

public class BGSpeedManage : MonoBehaviour {
    public Player player;
    public BGSpeedData[] bg;

    private float previousPlayerX; // 이전 프레임의 플레이어 x좌표

    private void Start() {
        // 초기화: 플레이어의 현재 x좌표를 저장
        if (player != null) {
            previousPlayerX = player.transform.position.x;
        }
    }

    private void FixedUpdate() {
        BGRoll();
    }

    public void BGRoll() {
        if (player == null) return;

        // 현재 플레이어 x좌표
        float currentPlayerX = player.transform.position.x;

        // x좌표 이동량 계산
        float deltaX = currentPlayerX - previousPlayerX;

        // 배경 이동
        foreach (var bgData in bg) {
            if (bgData.isAutoPlay)
                continue;

            bgData.meshRenderer.material.mainTextureOffset += 0.01f * deltaX * bgData.speed * Vector2.right;
        }

        // 현재 x좌표를 이전 x좌표로 업데이트
        previousPlayerX = currentPlayerX;
    }
}
