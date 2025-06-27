using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MapTransit : MonoBehaviour{
    public int transitMapNum;
    public int requiredSceneNum;
    public Transform destination;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            //GamaManager.Instance.SceneManager.playScene(requiredSceneNum);
            GamaManager.Instance.player.GetComponent<Animator>().SetBool("isMove", true);
            GamaManager.Instance.panel.gameObject.SetActive(true);
            GamaManager.Instance.player.sceneSwitch();
            GamaManager.Instance.panel.color = new Color(0, 0, 0, 0); // 투명한 검은색으로 초기화

            GamaManager.Instance.panel.DOFade(1f, 1f).OnComplete(() => {
                GamaManager.Instance.player.GetComponent<Animator>().SetBool("isMove", true);
                GamaManager.Instance.player.inputVec2 = new Vector2(1, 0);
                GamaManager.Instance.player.transform.position = destination.position;

                // 일정 시간 대기 후 페이드 아웃
                GamaManager.Instance.player.GetComponent<Animator>().SetBool("isMove", false);
                GamaManager.Instance.panel.DOFade(0f, 1f).SetDelay(1f).OnComplete(() => {
                    GamaManager.Instance.player.sceneSwitch();
                    GamaManager.Instance.player.SavePlayerPosition();
                    GamaManager.Instance.player.inputVec2 = new Vector2(0, 0);
                    //GamaManager.Instance.UIManager.PrintMSG("Lemon_Sprout");
                });

            });
        }
    }
}
