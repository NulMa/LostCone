using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPCs : MonoBehaviour {
    public Action OnInteract; // 인터렉션 처리
    private Animator animator; // Animator 컴포넌트 참조

    public GameObject[] chained; // 연결된 오브젝트
    public GameObject video;

    public bool isPlayed;

    private void Awake()
    {
        // Animator 컴포넌트 가져오기
        animator = GetComponent<Animator>();

        // 기본 동작 설정
        OnInteract = () => Debug.Log("기본 인터렉션 입니다.");

        if (gameObject.name == "Lemon_Sprout" && GamaManager.Instance.SceneManager.scenes[1].isDone)
        {
            OnVideoDisabled();
        }

        switch (gameObject.name) {
            case "Lemon_Sprout":
                break;

            case "Gumi":
                if (PlayerPrefs.GetInt("HiddenWall_OnFunction_1", 0) == 1){
                    animator.SetTrigger("Front");
                }
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Player player = collision.GetComponent<Player>();
            if (player != null && player.isInteracting) { // isInteracting 필드 확인
                Debug.Log("[NPCs] Player is interacting with " + gameObject.name);
                // 인터렉션에 대한 처리
                OnInteract?.Invoke();

                switch (gameObject.name)
                {
                    case "Lemon_Sprout":
                        if (GamaManager.Instance.SceneManager.scenes[1].isDone) break;
                        // 오브젝트 이름이 "Lemon_Sprout"인지 확인
                        if (gameObject.name == "Lemon_Sprout" && animator != null)
                        {
                            if (GamaManager.Instance.ItemDataManager.itemCount != GamaManager.Instance.ItemDataManager.bools.Length && !isPlayed)
                            {
                                GamaManager.Instance.UIManager.PrintMSG("Lemon_Sprout");
                                return;
                            }
                            else
                            {
                                // video 오브젝트 활성화
                                GamaManager.Instance.achiveCall("HalfCut");
                                playVideo();
                            }

                        }
                        break;

                    case "YourName":
                        if (GamaManager.Instance.ItemDataManager.itemCount != GamaManager.Instance.ItemDataManager.bools.Length && !isPlayed){
                            GamaManager.Instance.UIManager.PrintMSG("Gumi_Need_Help");
                            return;
                        }

                        else{
                            playVideo();
                            GamaManager.Instance.achiveCall("Alive");
                            animator.SetTrigger("Front");
                        }
                        break;

                    case "GarakutaKun_0":
                        if (GamaManager.Instance.ItemDataManager.itemCount != GamaManager.Instance.ItemDataManager.bools.Length && !isPlayed){
                            Debug.Log("GarakutaKun_0 need help");
                            return;
                        }

                        else{
                            playVideo();
                            Debug.Log("Clear GarakutaKun_0");
                        }
                        break;
                }
                StartCoroutine(ResetPlayerInteraction(player)); // 인터렉션 상태 초기화
            }
        }
    }

    private void OnVideoDisabled() {

        switch (gameObject.name) {
            case "Lemon_Sprout":
                animator.SetTrigger("Grow"); // Animator의 "Grow" 트리거 설정
                StartCoroutine(ActivateChainedAnimator()); //chained의 Animator 활성화
                chained[1].GetComponent<FollowUVLight>().stagePresets[0].mode = FollowMode.Fixed;
                break;

            case "Gumi":
                StartCoroutine(ActivateChainedAnimator());
                break;
        }

    }

    private IEnumerator ActivateChainedAnimator() {

        switch (gameObject.name) {
            case "Lemon_Sprout":
                if (GamaManager.Instance.SceneManager.scenes[1].isDone){
                    Animator chainedAnimator = chained[0].GetComponent<Animator>();
                    chainedAnimator.enabled = true;
                    break;
                }
                yield return new WaitForSeconds(1.5f); // 1.5초 대기
                if (chained != null) {
                    Animator chainedAnimator = chained[0].GetComponent<Animator>();
                    if (chainedAnimator != null) {
                        chainedAnimator.enabled = true; // chained의 Animator 활성화
                        GamaManager.Instance.SceneManager.sceneIsDone(1);
                        GamaManager.Instance.UIManager.PrintMSG("Lemon_Tree");
                        GamaManager.Instance.rainSwitch();
                    }
                    else {
                        Debug.LogWarning("Chained 오브젝트의 Animator가 없습니다.");
                    }
                }
                else {
                    Debug.LogWarning("Chained 오브젝트가 설정되지 않았습니다.");
                }
                break;

            case "Gumi":
                Debug.Log("Ikimono is free!");
                break;
        }
    }

    private IEnumerator ResetPlayerInteraction(Player player) {
        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        player.isInteracting = false; // 플레이어의 인터렉션 상태 초기화
    }
    private IEnumerator FadeInVideoAndPlayAnimator() {
        // video의 Image 또는 SpriteRenderer 가져오기
        var image = video.GetComponent<Image>();
        var spriteRenderer = video.GetComponent<SpriteRenderer>();

        if (image != null) {
            // Image의 알파값을 0에서 1로 변경
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            yield return image.DOFade(1f, 1f).SetEase(Ease.InOutQuad).WaitForCompletion();
        }
        else {
            Debug.LogWarning("video 오브젝트에 Image 또는 SpriteRenderer가 없습니다.");
        }

        video.GetComponent<Animator>().enabled = true; // video의 Animator 활성화
        // 애니메이터 실행
        if (animator != null) {
            StartCoroutine(WaitForVideoToDisable()); // video가 비활성화될 때까지 대기
        }
    }

    private IEnumerator WaitForVideoToDisable() {
        // video 오브젝트가 비활성화될 때까지 대기
        while (video != null && video.activeSelf) {
            yield return null; // 매번 프레임까지 대기
        }

        // video가 비활성화된 후 후속 함수를 호출
        OnVideoDisabled();
    }
    public void playVideo() {
        if (video != null && !isPlayed) {
            video.SetActive(true);
            //AudioManager.Instance.PlayBGM(0, true, 1f); // 스토리 음악 //////만듦 
            StartCoroutine(FadeInVideoAndPlayAnimator());
            isPlayed = true; // isPlayed 상태 업데이트
        }
    }
}