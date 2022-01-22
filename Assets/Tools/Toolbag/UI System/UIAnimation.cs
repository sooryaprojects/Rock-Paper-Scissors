using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace UISystem
{


    [RequireComponent(typeof(UIAnimator))]
    public class UIAnimation : MonoBehaviour
    {
        protected GameObject content;
        protected GameObject overlay;
        protected Canvas canvas;
        protected BaseUI baseUI;
        protected bool isAnimationRunning;
        [SerializeField] public AnimationCurve animationCurve;
        public UnityEvent OnShowed;
        public UnityEvent OnHideEnd;
        public virtual void Awake()
        {
            if (GetComponent<Popup>() != null)
            {
                content = transform.GetChild(1).gameObject;
                overlay = transform.GetChild(0).gameObject;

            }
            else if (GetComponent<Screen>() != null)
            {
                overlay = null;
                content = transform.GetChild(1).gameObject;
            }
            else
            {
                Debug.Log("No Base UI on : " + gameObject.name);
            }
            canvas = GetComponent<Canvas>();
            baseUI = GetComponent<BaseUI>();
        }
        public virtual void OnShowAnimationStarted()
        {
            // canvas.enabled = true;
            baseUI.Enable();
            isAnimationRunning = false;

        }

        public virtual void OnShowAnimationRunning(float animPerc)
        {

            isAnimationRunning = true;

        }

        public virtual void OnShowAnimationEnded()
        {
            isAnimationRunning = false;
            OnShowed?.Invoke();
        }

        public virtual void OnHideAnimationStarted()
        {
            isAnimationRunning = false;

        }

        public virtual void OnHideAnimationRunning(float animPerc)
        {

            isAnimationRunning = true;

        }

        public virtual void OnHideAnimationEnded()
        {
            // canvas.enabled = false;
            baseUI.Disable();
            isAnimationRunning = false;
            OnHideEnd.Invoke();
        }


    }
}