using UnityEngine;

namespace Code.Map
{
    public class LemonTree : MonoBehaviour
    {
        [SerializeField] private Animator lemonTree;
        [SerializeField] private GameObject vines;
        [SerializeField] private GameObject interactButton;

        [SerializeField] private Sprite lemonTreeSprite;

        private readonly string _growState = "Grow";

        private void Start()
        {
            if (AchiveManager.instance.IsAchiveCleared(Defines.StageKeys[0]))
            {
                vines?.SetActive(false);
                interactButton?.SetActive(false);
                lemonTree.SetTrigger(_growState);
            }
        }
    }
}