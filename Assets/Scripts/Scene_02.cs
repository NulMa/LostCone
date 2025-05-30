using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Scene_02 : MonoBehaviour{
    Animator anim;
    SpriteRenderer sprite;

    private void Awake() {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        if (GamaManager.Instance.SceneManager.scenes[2].isDone){
            gameObject.SetActive(false);
        }
    }
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            GamaManager.Instance.SceneManager.playScene(2);
            anim.Play("Cone_Stand");
            sprite.flipX = true;
            StartCoroutine(RollAndMove());
        }
    }

    private IEnumerator RollAndMove() {
        yield return new WaitForSeconds(2f);
        anim.Play("Cone_Roll");

        var moveX = transform.DOMoveX(transform.position.x + 9f, 1.5f)
            .SetEase(Ease.Linear);

        // Y�� ���� Ʈ�� (0.5f��ŭ ���Ʒ���, 0.25�ʾ� 4�� �ݺ�)
        var swingY = transform.DOMoveY(transform.position.y + 0.5f, 0.5f)
            .SetLoops(4, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        // X�� �̵��� ������ Fin() ȣ��
        moveX.OnComplete(() => Fin());
    }

    private void Fin() {
        gameObject.SetActive(false);
        GamaManager.Instance.SceneManager.sceneIsDone(2);
        GamaManager.Instance.SaveGame();
    }
}
