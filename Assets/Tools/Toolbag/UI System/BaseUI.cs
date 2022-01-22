using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UISystem
{

    public class BaseUI : MonoBehaviour
    {
        protected GameObject content;
        protected Canvas canvas;
        protected UIAnimator uiAnimator;
        protected UIAnimation uiAnimation;
        protected CanvasGroup canvasGroup;


        List<Animatable> animatables = new List<Animatable>();
        public bool isActive { get; private set; }

        public virtual void Awake()
        {
            Application.targetFrameRate = 60;
            content = transform.GetChild(1).gameObject;
            canvas = GetComponent<Canvas>();
            canvasGroup = content.GetComponent<CanvasGroup>();

            uiAnimator = GetComponent<UIAnimator>();
            uiAnimation = GetComponent<UIAnimation>();
        }

        public virtual void RegisterAnimatable(Animatable animatable) => animatables.Add(animatable);
        public virtual void Disable() => canvas.enabled = false;//screws with the joysticks because apparently they scale (what?)// content.SetActive(false);

        public virtual void Enable() => canvas.enabled = true;//screws with the joysticks because apparently they scale (what?)// content.SetActive(true);

        public void ShowAnimatables()
        {
            for (int i = 0; i < animatables.Count; i++)
            {
                animatables[i].ShowAnimation();
            }
        }

        public void HideAnimatables()
        {
            for (int i = 0; i < animatables.Count; i++)
            {
                animatables[i].HideAnimation();
            }
        }
        public virtual void Show()
        {
            if (isActive)
                return;

            canvasGroup.interactable = true;
            if (uiAnimator)
            {
                uiAnimator.StopHide();
                uiAnimator.StartShow();
                isActive = true;
            }
            else
            {
                Enable();
                isActive = true;
            }
            ShowAnimatables();
            Redraw();
        }
        public virtual void Hide()
        {
            canvasGroup.interactable = false;
            if (uiAnimator)
            {
                uiAnimator.StopShow();
                uiAnimator.StartHide();
                isActive = false;
            }
            else
            {
                Disable();
                isActive = false;
            }
            HideAnimatables();

        }

        public virtual void Redraw()
        {
        }

    }
}