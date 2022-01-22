using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class SceneViewContextMenu
{
    // private static Vector3[] corners = new Vector3[4];
    // static List<GameObject> uiCanvases = new List<GameObject>();
    // static SceneViewContextMenu()
    // {
    //     SceneView.onSceneGUIDelegate += OnSceneGUI;
    //     uiCanvases.Clear();
    //     GameObject[] root = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
    //     for (int i = 0; i < root.Length; i++)
    //     {
    //         if (root[i].gameObject.name == "UI")
    //         {
    //             for (int j = 0; j < root[i].transform.childCount; j++)
    //             {
    //                 uiCanvases.Add(root[i].gameObject.transform.GetChild(j).gameObject);
    //             }
    //         }
    //     }

    // }

    // static Vector2 initPos;
    // private static void OnSceneGUI(SceneView sceneView)
    // {
    //     var e = Event.current;

    //     if (e.type == EventType.MouseDown)
    //     {
    //         initPos = e.mousePosition;
    //     }
    //     if (e.type == EventType.KeyDown)
    //     {
    //         Debug.Log(e.keyCode);

    //         if (e.keyCode == KeyCode.LeftCommand)
    //         {
    //         }
    //     }


    //     if (e == null)
    //     {
    //         return;
    //     }

    //     if (e.button != 1)
    //     {
    //         return;
    //     }

    //     if (e.type != EventType.MouseUp)
    //     {
    //         return;
    //     }
    //     else
    //     {
    //         if (initPos == e.mousePosition && Input.GetKeyDown(KeyCode.LeftCommand))
    //         {
    //             e.Use();
    //             ShowUINames(e.mousePosition);
    //         }

    //     }
    // }

    // static int index;

    // static void ShowUINames(Vector2 pos)
    // {
    //     var menu = new GenericMenu();
    //     for (int i = 0; i < uiCanvases.Count; i++)
    //     {
    //         index = i;
    //         menu.AddItem(new GUIContent(uiCanvases[i].gameObject.name), false, index => OnSelect((int)index), index);

    //     }
    //     menu.ShowAsContext();

    // }


    // private static void OnSelect(int index)
    // {

    //     Selection.activeTransform = uiCanvases[index].transform;
    //     EditorGUIUtility.PingObject(uiCanvases[index]);
    // }

}