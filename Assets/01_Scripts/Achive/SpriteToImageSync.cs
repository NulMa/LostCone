using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SpriteToImageSync : MonoBehaviour {
    private SpriteRenderer sr;
    private Image img;

    void Awake() {
        sr = GetComponent<SpriteRenderer>();
        img = GetComponent<Image>();
    }

     // 애니메이션이 SpriteRenderer를 바꿀 때마다 Image에도 적용
    void LateUpdate() {
        if (sr != null && img != null && sr.sprite != null) {
            img.sprite = sr.sprite;
            img.color = sr.color; // 투명도나 색상 애니메이션도 동기화
        }
   }
}