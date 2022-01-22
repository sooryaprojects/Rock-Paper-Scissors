using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class StepMove : EditorWindow
{
    [MenuItem("[Custom Tools]/Step Move", false, 102)]
    internal static void Init()
    {

        var window = (StepMove)GetWindow(typeof(StepMove), false, "Step Move");
        window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 100f);
    }

    float stepSize;
    void Update()
    {
        Repaint();
    }
    float editorDeltaTime = 0f;
    float lastTimeSinceStartup = 0f;

    float value1 = 0f;
    float value2 = 100f;
    private void SetEditorDeltaTime()
    {
        // if (lastTimeSinceStartup == 0f)
        // {
        //     lastTimeSinceStartup = (float)EditorApplication.timeSinceStartup;
        // }
        // editorDeltaTime = (float)EditorApplication.timeSinceStartup - lastTimeSinceStartup;
        // lastTimeSinceStartup = (float)EditorApplication.timeSinceStartup;
    }

    float Width(int offset)
    {
        return Mathf.Abs(Mathf.Sin((float)EditorApplication.timeSinceStartup + offset) * 200) + 100;
    }

    float Height(int offset)
    {
        return Mathf.Abs(Mathf.Sin((float)EditorApplication.timeSinceStartup + offset) * 10) + 15;
    }

    float OscVal(float offset)
    {
        return Helper.Map(-1, 1, 0, 1, (Mathf.Sin((float)(EditorApplication.timeSinceStartup * stepSize) + offset)));
    }
    Vector3 col;
    Vector3 offsets = new Vector3(0f, 45f, 90f);
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        stepSize = EditorGUILayout.FloatField("Step size", stepSize);
        offsets = new Vector3(0f, 90f, 180f);
        col = new Vector3(OscVal(offsets.x), OscVal(offsets.y), OscVal(offsets.z));
        GUI.backgroundColor = new Color(col.x, col.y, col.z);

        if (GUILayout.Button("Up", GUILayout.Width(Width(0)), GUILayout.Height(Height(0))))
        {
            foreach (var item in Selection.gameObjects)
            {
                if (item.GetComponent<RectTransform>())
                {

                    item.GetComponent<RectTransform>().anchoredPosition = new Vector2(item.GetComponent<RectTransform>().anchoredPosition.x, item.GetComponent<RectTransform>().anchoredPosition.y + stepSize);
                }
                else
                {
                    item.transform.localPosition = new Vector3(item.transform.localPosition.x, item.transform.localPosition.y + stepSize, item.transform.localPosition.z);
                }

            }

        }

        AddOffset();

        if (GUILayout.Button("Down", GUILayout.Width(Width(3)), GUILayout.Height(Height(3))))
        {
            foreach (var item in Selection.gameObjects)
            {
                if (item.GetComponent<RectTransform>())
                {
                    item.GetComponent<RectTransform>().anchoredPosition = new Vector2(item.GetComponent<RectTransform>().anchoredPosition.x, item.GetComponent<RectTransform>().anchoredPosition.y - stepSize);
                }
                else
                {
                    item.transform.localPosition = new Vector3(item.transform.localPosition.x, item.transform.localPosition.y - stepSize, item.transform.localPosition.z);
                }
            }

        }
        AddOffset();

        if (GUILayout.Button("Left", GUILayout.Width(Width(6)), GUILayout.Height(Height(6))))
        {
            foreach (var item in Selection.gameObjects)
            {
                if (item.GetComponent<RectTransform>())
                {
                    item.GetComponent<RectTransform>().anchoredPosition = new Vector2(item.GetComponent<RectTransform>().anchoredPosition.x - stepSize, item.GetComponent<RectTransform>().anchoredPosition.y);
                }
                else
                {
                    item.transform.localPosition = new Vector3(item.transform.localPosition.x - stepSize, item.transform.localPosition.y, item.transform.localPosition.z);

                }
            }

        }
        AddOffset();

        if (GUILayout.Button("Right", GUILayout.Width(Width(9)), GUILayout.Height(Height(9))))
        {
            foreach (var item in Selection.gameObjects)
            {
                if (item.GetComponent<RectTransform>())
                {

                    item.GetComponent<RectTransform>().anchoredPosition = new Vector2(item.GetComponent<RectTransform>().anchoredPosition.x + stepSize, item.GetComponent<RectTransform>().anchoredPosition.y);
                }
                else
                {
                    item.transform.localPosition = new Vector3(item.transform.localPosition.x + stepSize, item.transform.localPosition.y, item.transform.localPosition.z);
                }
            }

        }
        AddOffset();

        if (GUILayout.Button("Forward", GUILayout.Width(Width(12)), GUILayout.Height(Height(12))))
        {
            foreach (var item in Selection.gameObjects)
            {
                if (item.GetComponent<RectTransform>())
                {

                }
                else
                {
                    item.transform.localPosition = new Vector3(item.transform.localPosition.x, item.transform.localPosition.y, item.transform.localPosition.z + stepSize);
                }
            }

        }

        AddOffset();

        if (GUILayout.Button("Back", GUILayout.Width(Width(15)), GUILayout.Height(Height(15))))
        {
            foreach (var item in Selection.gameObjects)
            {
                if (item.GetComponent<RectTransform>())
                {

                }
                else
                {
                    item.transform.localPosition = new Vector3(item.transform.localPosition.x, item.transform.localPosition.y, item.transform.localPosition.z - stepSize);
                }
            }

        }
    }
    void AddOffset()
    {
        offsets = new Vector3(offsets.x + 45, offsets.y + 45, offsets.z + 45);
        col = new Vector3(OscVal(offsets.x), OscVal(offsets.y), OscVal(offsets.z));
        GUI.backgroundColor = new Color(col.x, col.y, col.z);
    }


}
