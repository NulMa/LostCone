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

        if(gameObject.name == "Lemon_Sprout" && GamaManager.Instance.SceneManager.scenes[1].isDone) {
            OnVideoDisabled();
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Player player = collision.GetComponent<Player>();
            if (player != null && player.isInteracting) { // isInteracting �ʵ� Ȯ��
                // ��ȣ�ۿ� ���� ����
                OnInteract?.Invoke();

                switch (gameObject.name) {
                    case "Lemon_Sprout":
                        if (GamaManager.Instance.SceneManager.scenes[1].isDone) break;
                        // ������Ʈ �̸��� "Lemon_Sprout"���� Ȯ��
                        if (gameObject.name == "Lemon_Sprout" && animator != null) {
                            if (GamaManager.Instance.ItemDataManager.itemCount != GamaManager.Instance.ItemDataManager.bools.Length && !isPlayed) {
                                GamaManager.Instance.UIManager.PrintMSG("Lemon_Sprout");
                                return;
                            }
                            else {
                                // video ������Ʈ Ȱ��ȭ
                                playVideo();
                            }

                        }
                        break;

                    case "Gumi":
                        if (GamaManager.Instance.ItemDataManager.itemCount != GamaManager.Instance.ItemDataManager.bools.Length && !isPlayed) {
                            GamaManager.Instance.UIManager.PrintMSG("Gumi_Need_Help");
                            return;
                        }
                        else {
                            playVideo();
                        }
                        break;
                }
                StartCoroutine(ResetPlayerInteraction(player)); // ��ȣ�ۿ� ���� �ʱ�ȭ
            }
        }
    }

    private void OnVideoDisabled() {

        switch (gameObject.name) {
            case "Lemon_Sprout":
                animator.SetTrigger("Grow"); // Animator�� "Grow" Ʈ���� ����
                StartCoroutine(ActivateChainedAnimator()); //chained�� Animator Ȱ��ȭ
                break;

            case "Gumi":
                break;
        }

    }

    private IEnumerator ActivateChainedAnimator() {

        switch (gameObject.name) {
            case "Lemon_Sprout":
                if (GamaManager.Instance.SceneManager.scenes[1].isDone){
                    Animator chainedAnimator = chained.GetComponent<Animator>();
                    chainedAnimator.enabled = true;
                    break;
                }
                yield return new WaitForSeconds(1.5f); // 1.5�� ���
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
                break;

            case "Gumi":
                Debug.Log("Ikimono is free!");
                break;
        }
    }

    private IEnumerator ResetPlayerInteraction(Player player) {
        yield return new WaitForSeconds(0.1f); // 0.1�� ���
        player.isInteracting = false; // �÷��̾��� ��ȣ�ۿ� ���� �ʱ�ȭ
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
    public void playVideo() {
        if (video != null && !isPlayed) {
            video.SetActive(true);
            AudioManager.Instance.PlayBGM(0, true, 1f); // ����� ���
            StartCoroutine(FadeInVideoAndPlayAnimator());
            isPlayed = true; // isPlayed ���� ������Ʈ
        }
    }
}