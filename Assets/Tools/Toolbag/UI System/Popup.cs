using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UISystem
{

    public class Popup : BaseUI
    {
        [HideInInspector] public GameObject overlay;

        public override void Awake()
        {
            content = transform.GetChild(1).gameObject;
            overlay = transform.GetChild(0).gameObject;
            canvas = GetComponent<Canvas>();

            // content = transform.GetChild(0).gameObject;
            // canvas = GetComponent<Canvas>();
            canvasGroup = content.GetComponent<CanvasGroup>();

            uiAnimator = GetComponent<UIAnimator>();
            uiAnimation = GetComponent<UIAnimation>();
        }

        public void Fill(string description)
        {
            content.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = description.ToUpper();
        }
    }
}