using System.Linq;
using Code.Map;
using DG.Tweening;
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
        [SerializeField] private float textDuration = 0.7f;
        [SerializeField] private Animator irisOutAnimator;
        
        private readonly int _circleSizeHash = Shader.PropertyToID("_CircleSize");

        protected override void Awake()
        {
            base.Awake();
            fadeImage.material = new Material(fadeImage.material);
            
        }

        [ContextMenu("IRIS OUT")]
        protected async override void OnMessageRecieved()
        {
            irisOutAnimator.enabled = true;
            await Awaitable.WaitForSecondsAsync(0.2f);

            Sequence seq = DOTween.Sequence();
            mask.gameObject.SetActive(true);
            mask.transform.localScale = Vector3.zero;

            fadeImage.material.SetFloat(_circleSizeHash, 2f);
            seq.Append(fadeImage.material.DOFloat(0f, _circleSizeHash, 1f))
                .AppendInterval(0.25f);

            for (int i = 1; i <= 4; i++)
            {
                int idx = i;
                seq.AppendCallback(() => {
                    irisOutText.text = string.Join("  ",
                        Enumerable.Repeat("I ", idx));
                });
                seq.AppendInterval(textDuration / 4f);
            }

            seq.AppendCallback(() =>
                {
                    irisOutText.transform.localScale = Vector3.one * 1.25f;
                    GamaManager.Instance.achiveCall("IrisOut");
                    //irisOutText.text = "IRIS OUT";
                })
                .AppendInterval(1f)
                .Append(mask.transform.DOScale(15f, 1f))
                .AppendCallback(() =>
                {
                    AchiveManager.instance.SetAchiveClear(message);
                    irisOutText.transform.localScale = Vector3.one;
                    irisOutText.text = string.Empty;
                    mask.gameObject.SetActive(false);
                })
                .Append(fadeImage.material.DOFloat(2f, _circleSizeHash, 1f));
        }
    }
}