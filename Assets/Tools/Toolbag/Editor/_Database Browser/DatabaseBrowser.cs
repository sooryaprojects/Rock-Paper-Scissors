

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class DatabaseBrowser : EditorWindow
{
    static List<string> dataFileNames = new List<string>();

    static string path = "Assets/Game/Scripts/Data/Resources/";

    [MenuItem("[Custom Tools]/Database Browser", false)]
    internal static void Init()
    {

        var window = (DatabaseBrowser)GetWindow(typeof(DatabaseBrowser), false, "Database Browser");
        window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 100f);
        dataFileNames.Clear();
        Refresh();
    }

    void OnGUI()
    {

        // EditorGUILayout.BeginHorizontal();
        // path = GUILayout.TextField(path, 100);
        // if (path != string.Empty)
        // {
        //     if (path.Substring(path.Length - 1) != "/")
        //     {
        //         path = path + "/";
        //     }
        // }
        // if (GUILayout.Button("Refresh"))
        // {
        //     Refresh();
        // }
        // EditorGUILayout.EndHorizontal();
        // EditorGUILayout.Space();



        for (int i = 0; i < dataFileNames.Count; i++)
        {
            if (GUILayout.Button(dataFileNames[i]))
            {
                EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(path + dataFileNames[i] + ".asset"));
                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path + dataFileNames[i] + ".asset");
            }
        }

    }

    static void Refresh()
    {
        if (!Directory.Exists(path))
        {
            Debug.LogWarning("Invalid path");
            return;
        }

        dataFileNames.Clear();
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] info = dir.GetFiles("*.*");
        foreach (FileInfo f in info)
        {
            if (f.Name.Substring(f.Name.Length - 5) == "asset")
            {
                dataFileNames.Add(f.Name.Replace(".asset", ""));
            }
        }
    }
}