using Code.Map;
using DG.Tweening;
using PaperFlower.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Achive
{
    public class IrisoutAchive : MessageReciever
    {
        [SerializeField] private Image fadeImage;
        [SerializeField] private Image mask;
        [SerializeField] private TextMeshProUGUI irisOutText;
        [SerializeField] private float textDuration = 1f;
        
        private readonly int _circleSizeHash = Shader.PropertyToID("_CircleSize");

        protected override void Awake()
        {
            base.Awake();
            fadeImage.material = new Material(fadeImage.material);
            
        }

        [ContextMenu("IRIS OUT")]
        protected override void OnMessageRecieved()
        {
            Sequence seq = DOTween.Sequence();
            mask.gameObject.SetActive(true);
            mask.transform.localScale = Vector3.zero;

            fadeImage.material.SetFloat(_circleSizeHash, 2f);
            seq.Append(fadeImage.material.DOFloat(0f, _circleSizeHash, 1f))
                .AppendInterval(0.25f)
                .Append(irisOutText.DOText("IRIS OUT", textDuration))
                .AppendInterval(1f)
                .Append(mask.transform.DOScale(15f, 1f))
                .AppendCallback(() => {
                    irisOutText.text = string.Empty;
                    mask.gameObject.SetActive(false); 
                })
                .Append(fadeImage.material.DOFloat(2f, _circleSizeHash, 1f));
        }
    }
}