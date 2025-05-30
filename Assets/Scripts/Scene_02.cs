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

        // Y축 스윙 트윈 (0.5f만큼 위아래로, 0.25초씩 4번 반복)
        var swingY = transform.DOMoveY(transform.position.y + 0.5f, 0.5f)
            .SetLoops(4, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        // X축 이동이 끝나면 Fin() 호출
        moveX.OnComplete(() => Fin());
    }

    private void Fin() {
        gameObject.SetActive(false);
        GamaManager.Instance.SceneManager.sceneIsDone(2);
        GamaManager.Instance.SaveGame();
    }
}
