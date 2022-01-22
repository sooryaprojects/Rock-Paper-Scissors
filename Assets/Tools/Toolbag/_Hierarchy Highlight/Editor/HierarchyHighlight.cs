using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using SimpleJSON;
using UISystem;
[InitializeOnLoad]
public class HierarchyHighlight
{
    public static Tags myTags;
    static Texture2D screenOn;
    static Texture2D screenOff;
    static Texture2D popupOn;
    static Texture2D popupOff;
    static Texture2D moduleOn;
    static Texture2D moduleOff;



    static HierarchyHighlight()
    {
        myTags = new Tags();

        screenOn = AssetDatabase.LoadAssetAtPath("Assets/Tools/Toolbag/_Hierarchy Highlight/screenOn.png", typeof(Texture2D)) as Texture2D;
        screenOff = AssetDatabase.LoadAssetAtPath("Assets/Tools/Toolbag/_Hierarchy Highlight/screenOff.png", typeof(Texture2D)) as Texture2D;
        popupOn = AssetDatabase.LoadAssetAtPath("Assets/Tools/Toolbag/_Hierarchy Highlight/popupOn.png", typeof(Texture2D)) as Texture2D;
        popupOff = AssetDatabase.LoadAssetAtPath("Assets/Tools/Toolbag/_Hierarchy Highlight/popupOff.png", typeof(Texture2D)) as Texture2D;
        moduleOn = AssetDatabase.LoadAssetAtPath("Assets/Tools/Toolbag/_Hierarchy Highlight/moduleOn.png", typeof(Texture2D)) as Texture2D;
        moduleOff = AssetDatabase.LoadAssetAtPath("Assets/Tools/Toolbag/_Hierarchy Highlight/moduleOff.png", typeof(Texture2D)) as Texture2D;



        EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItemRedraw;
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemRedraw;

        // EditorApplication
        HighlightData.Load(myTags);

    }
    //called by the editor window when a tag is added or removed
    public static void Init(Tags tags)
    {
        myTags = tags;
    }

    private static void OnHierarchyItemRedraw(int instanceID, Rect selectionRect)
    {
        //rect to be used to draw the highlighted box
        Rect myRect;
        //get gameobject from the supplied instance id
        GameObject go = (GameObject)EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (go != null)
        {
            for (int i = 0; i < myTags.tagNames.Count; i++)
            {
                if (go.name == myTags.tagNames[i])
                {
                    var color = myTags.tagColors[i];
                    myRect = new Rect(selectionRect.x - 15, selectionRect.y + 1, selectionRect.width - 20, selectionRect.height);

                    GUI.backgroundColor = color;

                    //multiple boxes mean darker color, lmao!
                    GUI.Box(myRect, "");
                    GUI.Box(myRect, "");
                    GUI.Box(myRect, "");
                    GUI.Box(myRect, "");
                    GUI.Box(myRect, "");


                    GUI.backgroundColor = Color.white;

                }
            }
            Canvas c = go.GetComponent<Canvas>();
            if (c != null)
            {
                Rect eyeRect = new Rect(selectionRect);

                //Screen width returns the width of the current screen.
                //Use it instead of selection rect's width because rects, 
                //at different depths in the hierarchy, have different widths
                //and we want the icon at the right near the edge regardless
                //of the depth of the selection in the hierarchy.
                eyeRect.x = UnityEngine.Screen.width - 40;
                eyeRect.width = 20;
                eyeRect.height = 20;

                Texture2D tex = null;

                GUI.color = Color.clear;
                if (GUI.Button(eyeRect, tex, GUI.skin.box))
                {

                    if (c.enabled)
                    {
                        c.enabled = false;
                    }
                    else
                    {
                        c.enabled = true;
                    }
                }
                GUI.color = Color.white;

                if (c.enabled)
                {
                    if (go.GetComponent<Popup>() != null)
                    {
                        GUI.Label(eyeRect, popupOn);
                    }
                    else if (go.GetComponent<UISystem.Screen>() != null)
                    {
                        GUI.Label(eyeRect, screenOn);
                    }
                    else if (go.GetComponent<UISystem.Module>() != null)
                    {
                        GUI.Label(eyeRect, moduleOn);
                    }
                    else
                    {

                    }
                }

                else
                {
                    if (go.GetComponent<Popup>() != null)
                    {
                        GUI.Label(eyeRect, popupOff);
                    }
                    else if (go.GetComponent<UISystem.Screen>() != null)
                    {
                        GUI.Label(eyeRect, screenOff);
                    }
                    else if (go.GetComponent<UISystem.Module>() != null)
                    {
                        GUI.Label(eyeRect, moduleOff);
                    }
                    else
                    {

                    }
                }
            }
        }
    }

    // static void DrawQuad(Rect position, Color color)
    // {
    //     Texture2D texture = new Texture2D(1, 1);
    //     texture.SetPixel(0, 0, color);
    //     texture.Apply();
    //     GUI.skin.box.normal.background = texture;
    //     GUI.Box(position, GUIContent.none);
    // }

}

