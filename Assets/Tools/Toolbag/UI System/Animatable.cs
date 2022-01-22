using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UISystem
{
    public class Animatable : MonoBehaviour
    {
        RectTransform rectTransform;
        Vector2 initPos;
        Vector2 toPos;
        Vector2 fromPos;


        [SerializeField] float duration = 0.4f;
        [SerializeField] float xOffset;
        [SerializeField] float yOffset;
        [SerializeField] bool fade;
        [SerializeField] AnimationCurve curve;
        float durationMux = 1;

        void Awake()
        {
            CacheComponents();
            duration = duration * durationMux;
        }

        void CacheComponents()
        {
            rectTransform = GetComponent<RectTransform>();
            initPos = rectTransform.anchoredPosition;
            toPos = initPos;
            fromPos = new Vector2(initPos.x + xOffset, initPos.y + yOffset);

            if (FindBaseUI(this.gameObject) != null)
            {
                FindBaseUI(this.gameObject).GetComponent<BaseUI>().RegisterAnimatable(this);
            }
        }

        public void ShowAnimation(bool shouldFade = false)
        {
            fade = shouldFade;
            StopAllCoroutines();
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(AnimateShow());
            }
        }

        Image img;
        public IEnumerator AnimateShow()
        {
            float elapsed = 0;
            Color ogColor = Color.white;
            if (fade)
            {
                img = GetComponent<Image>();
                img.color = img.color.WithAlpha(0);
                ogColor = img.color;
            }

            while (elapsed < duration)
            {
                rectTransform.anchoredPosition = Vector2.LerpUnclamped(fromPos, toPos, curve.Evaluate(elapsed / duration));
                if (fade)
                {
                    img.color = Color.Lerp(ogColor.WithAlpha(0), ogColor.WithAlpha(1), elapsed / duration);
                }

                // rectTransform.eulerAngles = Vector2.Lerp(fromRot, toRot, rotCurve.Evaluate(elapsed/duration));
                elapsed += Time.deltaTime;
                yield return null;
            }
            rectTransform.anchoredPosition = toPos;

            yield return null;

        }

        public void HideAnimation()
        {
            StopAllCoroutines();
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(AnimateHide());
            }
        }

        public IEnumerator AnimateHide()
        {
            float elapsed = 0;
            while (elapsed < duration)
            {
                rectTransform.anchoredPosition = Vector2.LerpUnclamped(toPos, fromPos, curve.Evaluate(elapsed / duration));
                elapsed += Time.deltaTime;
                yield return null;
            }
            // rectTransform.anchoredPosition = ;

            yield return null;

        }

        public GameObject FindBaseUI(GameObject childObject)
        {
            Transform t = childObject.transform;
            while (t.parent != null)
            {
                if (t.parent.GetComponent<BaseUI>() != null)
                {
                    return t.parent.gameObject;

                }
                t = t.parent.transform;
            }
            return null;
        }
    }
}