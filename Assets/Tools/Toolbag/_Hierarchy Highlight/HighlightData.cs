using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class HighlightData
{

    static string path, jsonString;

    public static void Load(Tags tags)
    {
        path = Application.dataPath + "/highlightData.json";
        if (File.Exists(path))
        {
            jsonString = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(jsonString, tags);
        }
        else
        {
            // Debug.Log("No file.");
        }

    }

    public static void Save(Tags tags)
    {
        path = Application.dataPath + "/highlightData.json";
        if (File.Exists(path))
        {
            jsonString = JsonUtility.ToJson(tags);
            File.WriteAllText(path, jsonString);


        }
        else
        {
            jsonString = JsonUtility.ToJson(tags);
            File.WriteAllText(path, jsonString);
        }


    }
}
