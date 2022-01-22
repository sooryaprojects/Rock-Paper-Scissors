using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem
{

    public enum UIAnimationType
    {
        Show,
        Hide
    }
    public class UIAnimator : MonoBehaviour
    {
        [SerializeField] float animationTime;
        IEnumerator showCoroutine;
        IEnumerator hideCoroutine;

        UIAnimationType animationType;
        [SerializeField] UIAnimation showAnim;
        [SerializeField] UIAnimation hideAnim;

        public void StartShow()
        {
            showCoroutine = ShowAnimation();
            showAnim = GetComponent<UIAnimation>();
            showAnim.StartCoroutine(showCoroutine);
        }

        public void StartHide()
        {
            hideCoroutine = HideAnimation();
            hideAnim = GetComponent<UIAnimation>();
            hideAnim.StartCoroutine(hideCoroutine);

        }

        // public void Animate(UIAnimation animLogic)
        // {
        //     // SetCallbackType(animLogic);
        //     animationCoroutine = Animation(animLogic);
        //     animLogic.StartCoroutine(animationCoroutine);
        // }

        public IEnumerator ShowAnimation()
        {
            float elapsed = 0;
            float perc;
            showAnim.OnShowAnimationStarted();
            while (elapsed < animationTime)
            {
                perc = elapsed / animationTime;
                showAnim.OnShowAnimationRunning(showAnim.animationCurve.Evaluate(perc));
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            showAnim.OnShowAnimationEnded();

            yield return null;
        }

        public IEnumerator HideAnimation()
        {
            float elapsed = 0;
            float perc;
            hideAnim.OnHideAnimationStarted();
            while (elapsed < animationTime)
            {
                perc = elapsed / animationTime;
                hideAnim.OnHideAnimationRunning(hideAnim.animationCurve.Evaluate(perc));
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            hideAnim.OnHideAnimationEnded();
            yield return null;
        }

        public void StopShow()
        {
            if (showCoroutine != null)
            {
                showAnim.StopCoroutine(showCoroutine);
            }
        }

        public void StopHide()
        {
            if (hideCoroutine != null)
            {
                hideAnim.StopCoroutine(hideCoroutine);
            }
        }
    }

}