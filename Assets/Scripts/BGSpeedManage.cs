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

    private float previousPlayerX; // ���� �������� �÷��̾� x��ǥ

    private void Start() {
        // �ʱ�ȭ: �÷��̾��� ���� x��ǥ�� ����
        if (player != null) {
            previousPlayerX = player.transform.position.x;
        }
    }

    private void FixedUpdate() {
        BGRoll();
    }

    public void BGRoll() {
        if (player == null) return;

        // ���� �÷��̾� x��ǥ
        float currentPlayerX = player.transform.position.x;

        // x��ǥ �̵��� ���
        float deltaX = currentPlayerX - previousPlayerX;

        // ��� �̵�
        foreach (var bgData in bg) {
            if (bgData.isAutoPlay)
                continue;

            bgData.meshRenderer.material.mainTextureOffset += 0.01f * deltaX * bgData.speed * Vector2.right;
        }

        // ���� x��ǥ�� ���� x��ǥ�� ������Ʈ
        previousPlayerX = currentPlayerX;
    }
}
