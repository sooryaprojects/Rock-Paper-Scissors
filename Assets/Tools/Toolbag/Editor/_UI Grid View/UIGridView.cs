using System;
using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEditor;
using UnityEngine;
public class UIGridView : Editor
{
    static List<GameObject> screenList = new List<GameObject>();
    static List<Vector2> ogPos = new List<Vector2>();
    static List<Vector2> gridPos = new List<Vector2>();

    [MenuItem("[Custom Tools]/Grid View/Show", false, 0)]
    static void ShowGridView()
    {
        screenList.Clear();
        ogPos.Clear();
        gridPos.Clear();

        GameObject uiObj = GameObject.Find("UI");
        int y = 0;
        int x = 0;
        for (int i = 0; i < uiObj.transform.childCount; i++)
        {
            GameObject g = uiObj.transform.GetChild(i).gameObject;
            if (g.GetComponent<BaseUI>() != null)
            {
                x++;
                if (i % 5 == 0)
                {
                    y++;
                    x = 0;
                }
                screenList.Add(g);
                ogPos.Add(Vector2.zero);
                gridPos.Add(
                    new Vector2(
                        g.transform.Find("Content").GetComponent<RectTransform>().anchoredPosition.x + (x * 4000),
                        g.transform.Find("Content").GetComponent<RectTransform>().anchoredPosition.y - (y * 2200)
                    )
                );
                g.transform.Find("Content").GetComponent<RectTransform>().anchoredPosition = gridPos[i];
                screenList[i].GetComponent<Canvas>().enabled = true;
            }
        }
    }

    [MenuItem("[Custom Tools]/Grid View/Reset", false, 0)]
    static void ResetGridView()
    {
        for (int i = 0; i < screenList.Count; i++)
        {
            screenList[i].transform.Find("Content").GetComponent<RectTransform>().anchoredPosition = ogPos[i];
            screenList[i].GetComponent<Canvas>().enabled = false;

        }

    }
}