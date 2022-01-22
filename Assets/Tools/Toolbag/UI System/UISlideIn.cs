using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;
using UnityEngine.UI;


public class UISlideIn : UIAnimation
{
    public Vector3 initPos;
    public Vector3 finalPos;
    RectTransform rectTransform;

    public override void Awake()
    {
        base.Awake();
        rectTransform = content.GetComponent<RectTransform>();
    }
    public override void OnShowAnimationStarted()
    {
        base.OnShowAnimationStarted();
    }

    public override void OnShowAnimationRunning(float animPerc)
    {
        rectTransform.anchoredPosition = Vector2.Lerp(initPos, finalPos, animPerc);
        //overlay.GetComponent<Image>().color = Color.black.WithAlpha(animPerc / 3f);
        base.OnShowAnimationRunning(animPerc);

    }

    public override void OnShowAnimationEnded()
    {
        rectTransform.anchoredPosition = finalPos;
        base.OnShowAnimationEnded();

    }

    public override void OnHideAnimationStarted()
    {
        base.OnHideAnimationStarted();
    }

    public override void OnHideAnimationRunning(float animPerc)
    {

        rectTransform.anchoredPosition = Vector2.Lerp(finalPos, initPos, animPerc);
        //overlay.GetComponent<Image>().color = Color.black.WithAlpha((1 - animPerc) / 3f);

        base.OnHideAnimationRunning(animPerc);

    }

    public override void OnHideAnimationEnded()
    {
        rectTransform.anchoredPosition = initPos;
        base.OnHideAnimationEnded();

    }
}
