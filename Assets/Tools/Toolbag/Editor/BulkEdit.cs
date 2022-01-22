using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class BulkEdit : EditorWindow
{
    [MenuItem("[Custom Tools]/Bulk Edit")]
    static void CreateWizard()
    {
        var window = (BulkEdit)GetWindow(typeof(BulkEdit), false, "Bulk Edit");
        window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 100f);
    }

    string input = "";
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        input = EditorGUILayout.TextField("Input", input, GUILayout.MinWidth(400));
        if (GUILayout.Button("Fill Text"))
        {
            //separate csv
            string[] values = input.Split(',');
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Selection.gameObjects[i].GetComponent<Text>().text = values[i];
                Debug.Log(Selection.gameObjects[i].GetInstanceID());
            }
        }
    }
}
