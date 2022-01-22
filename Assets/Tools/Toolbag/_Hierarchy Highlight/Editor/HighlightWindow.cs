using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HighlightWindow : EditorWindow
{
    private Vector2 scrollPos;
    static Tags tags = new Tags();
    [MenuItem("[Custom Tools]/Hierarchy Tags", false, 102)]
    static internal void Init()
    {
        var window = (HighlightWindow)GetWindow(typeof(HighlightWindow), false, "Highlight Manager");
        window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 100f);
        HighlightData.Load(tags);
    }
    string tagKey = "";
    Color tagValue = Color.white;


    void OnGUI()
    {
        HighlightData.Load(tags);
        EditorGUILayout.BeginVertical(GUI.skin.box);
        this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos, false, false);

        //add new data
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        tagKey = EditorGUILayout.TextField(tagKey);
        tagValue = EditorGUILayout.ColorField("", tagValue);
        if (GUILayout.Button("+", GUILayout.Width(40), GUILayout.Height(20)))
        {
            tags.tagNames.Add(tagKey);
            tags.tagColors.Add(tagValue);
            HighlightData.Save(tags);
            HighlightData.Load(tags);
            HierarchyHighlight.Init(tags);
        }


        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        //display old data
        for (int i = 0; i < tags.tagNames.Count; i++)
        {

            GUI.color = tags.tagColors[i];
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUI.color = Color.white;
            GUIStyle lField = new GUIStyle();
            lField.normal.textColor = Color.white;
            EditorGUILayout.LabelField(tags.tagNames[i], lField);


            if (GUILayout.Button("x", GUILayout.Width(40), GUILayout.Height(20)))
            {
                tags.tagNames.RemoveAt(i);
                tags.tagColors.RemoveAt(i);
                HighlightData.Save(tags);
                HighlightData.Load(tags);
                HierarchyHighlight.Init(tags);

            }

            EditorGUILayout.EndHorizontal();

            if(tags.tagNames[i] == "GameManager" || tags.tagNames[i] == "Game Manager" || tags.tagNames[i] == "Game manager")
            {
                EditorGUILayout.HelpBox("THEHRO!\n\nUSING GAME MANAGERS IN 2018 IS A SIN.\nHERE'S HOW YOU CAN STILL REPENT AND ESCAPE THE DEPTHS OF HELL.\n\nSTEP 1: REMOVE THE GAME MANAGER SCRIPT FROM YOUR GAME.\n\nSTEP 3: KILL AND EAT ANOTHER PROGRAMMER WHO USES GAME MANAGERS.\n\nSTEP 2: SAY 'SORRY SHAKTIMAN' WHEN YOU'RE DONE.", MessageType.Warning, true);
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }



}


