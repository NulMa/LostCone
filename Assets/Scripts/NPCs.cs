using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPCs : MonoBehaviour {
    public Action OnInteract; // ��ȣ�ۿ� ����
    private Animator animator; // Animator ������Ʈ ����

    public GameObject chained; // ����� ������Ʈ
    public GameObject video;

    public bool isPlayed;

    private void Awake() {
        // Animator ������Ʈ ��������
        animator = GetComponent<Animator>();

        // �⺻ ���� ����
        OnInteract = () => Debug.Log("�⺻ ��ȣ�ۿ� �����Դϴ�.");
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Player player = collision.GetComponent<Player>();
            if (player != null && player.isInteracting) { // isInteracting �ʵ� Ȯ��
                // ��ȣ�ۿ� ���� ����
                OnInteract?.Invoke();

                // ������Ʈ �̸��� "Lemon_Sprout"���� Ȯ��
                if (gameObject.name == "Lemon_Sprout" && animator != null) {
                    if (GamaManager.Instance.ItemDataManager.itemCount != GamaManager.Instance.ItemDataManager.bools.Length || isPlayed) {
                        GamaManager.Instance.UIManager.PrintMSG("Lemon_Sprout");
                        return;
                    }


                    // video ������Ʈ Ȱ��ȭ
                    if (video != null) {
                        video.SetActive(true);
                        StartCoroutine(FadeInVideoAndPlayAnimator());
                        isPlayed = true; // isPlayed ���� ������Ʈ

                    }
                }

                StartCoroutine(ResetPlayerInteraction(player)); // ��ȣ�ۿ� ���� �ʱ�ȭ
            }
        }
    }

    private IEnumerator FadeInVideoAndPlayAnimator() {
        // video�� Image �Ǵ� SpriteRenderer ��������
        var image = video.GetComponent<Image>();
        var spriteRenderer = video.GetComponent<SpriteRenderer>();

        if (image != null) {
            // Image�� ������ 0���� 1�� ����
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            yield return image.DOFade(1f, 1f).SetEase(Ease.InOutQuad).WaitForCompletion();
        }
        else {
            Debug.LogWarning("video ������Ʈ�� Image �Ǵ� SpriteRenderer�� �����ϴ�.");
        }

        video.GetComponent<Animator>().enabled = true; // video�� Animator Ȱ��ȭ
        // �ִϸ����� ���
        if (animator != null) {
            StartCoroutine(WaitForVideoToDisable()); // video�� ��Ȱ��ȭ�� ������ ���
        }
    }

    private IEnumerator WaitForVideoToDisable() {
        // video ������Ʈ�� ��Ȱ��ȭ�� ������ ���
        while (video != null && video.activeSelf) {
            yield return null; // ���� �����ӱ��� ���
        }

        // video�� ��Ȱ��ȭ�� �� ������ �Լ��� ȣ��
        OnVideoDisabled();
    }

    private void OnVideoDisabled() {
        animator.SetTrigger("Grow"); // Animator�� "Grow" Ʈ���� ����
        StartCoroutine(ActivateChainedAnimator()); // 1�� �� chained�� Animator Ȱ��ȭ
    }

    private IEnumerator ActivateChainedAnimator() {
        yield return new WaitForSeconds(1.5f); // 1�� ���

        if (chained != null) {
            Animator chainedAnimator = chained.GetComponent<Animator>();
            if (chainedAnimator != null) {
                chainedAnimator.enabled = true; // chained�� Animator Ȱ��ȭ
                GamaManager.Instance.SceneManager.sceneIsDone(1);
                GamaManager.Instance.UIManager.PrintMSG("Lemon_Tree");
                GamaManager.Instance.rainSwitch();
            }
            else {
                Debug.LogWarning("Chained ������Ʈ�� Animator�� �����ϴ�.");
            }
        }
        else {
            Debug.LogWarning("Chained ������Ʈ�� �������� �ʾҽ��ϴ�.");
        }
    }

    private IEnumerator ResetPlayerInteraction(Player player) {
        yield return new WaitForSeconds(0.1f); // 0.1�� ���
        player.isInteracting = false; // �÷��̾��� ��ȣ�ۿ� ���� �ʱ�ȭ
    }
}