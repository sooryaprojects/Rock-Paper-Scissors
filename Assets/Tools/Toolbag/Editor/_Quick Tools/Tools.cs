using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class Tools : Editor
{
    static string[] folderNames = new string[] { "_Scripts", "_Scenes", "Sprites", "Prefabs", "Misc", "Audio", "Materials" };
    static string[] allAssetPaths;
    static string extension;
    static string assetName;
    static string destinationPath;
    static int currentSceneNumber;

    [MenuItem("[Custom Tools]/Create My Folders", false, 0)]
    static void CreateMyFolders()
    {
        for (int i = 0; i < folderNames.Length; i++)
        {
            if (!AssetDatabase.IsValidFolder("Assets/" + folderNames[i]))
            {
                AssetDatabase.CreateFolder("Assets", folderNames[i]);
            }
        }
    }

    [MenuItem("[Custom Tools]/Organize/All", false, 0)]
    static void OrganizeAll()
    {
        OrganizeScenes();
        OrganizeScripts();
        OrganizeAnimations();
        OrganizePrefabs();
        OrganizeAnimatorControllers();
        OrganizeSprites();
        OrganizeAudio();
    }

    [MenuItem("[Custom Tools]/Organize/Scripts", false, 52)]
    static void OrganizeScripts()
    {
        if (EditorUtility.DisplayDialog("Heads up!", "Organizing scripts will close any scripts in your script editor that are being moved. You'll need to re-open them.", "I'm okay with that", "Cancel") == true)
        {
            allAssetPaths = AssetDatabase.GetAllAssetPaths();
            destinationPath = "Assets/_Scripts/";
            //Check if a script folder exists. If not, create one.
            if (!AssetDatabase.IsValidFolder("Assets/" + "_Scripts"))
            {
                AssetDatabase.CreateFolder("Assets", "_Scripts");
            }
            Move("cs");
        }
    }

    [MenuItem("[Custom Tools]/Organize/Scenes", false, 52)]
    static void OrganizeScenes()
    {
        allAssetPaths = AssetDatabase.GetAllAssetPaths();
        destinationPath = "Assets/_Scenes/";
        //Check if a scene folder exists. If not, create one.
        if (!AssetDatabase.IsValidFolder("Assets/" + "_Scenes"))
        {
            AssetDatabase.CreateFolder("Assets", "_Scenes");
        }
        Move("unity");
    }

    [MenuItem("[Custom Tools]/Organize/Animations", false, 52)]
    static void OrganizeAnimations()
    {
        allAssetPaths = AssetDatabase.GetAllAssetPaths();
        destinationPath = "Assets/Animations/";
        //Check if a animations folder exists. If not, create one.
        if (!AssetDatabase.IsValidFolder("Assets/" + "Animations"))
        {
            AssetDatabase.CreateFolder("Assets", "Animations");
        }
        Move("anim");
    }

    [MenuItem("[Custom Tools]/Organize/Prefabs", false, 52)]
    static void OrganizePrefabs()
    {
        allAssetPaths = AssetDatabase.GetAllAssetPaths();
        destinationPath = "Assets/Prefabs/";
        //Check if a prefabs folder exists. If not, create one.
        if (!AssetDatabase.IsValidFolder("Assets/" + "Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        Move("prefab");
    }

    [MenuItem("[Custom Tools]/Organize/Animator Controller", false, 52)]
    static void OrganizeAnimatorControllers()
    {
        allAssetPaths = AssetDatabase.GetAllAssetPaths();
        destinationPath = "Assets/Animator Controllers/";
        //Check if a animation controller folder exists. If not, create one.
        if (!AssetDatabase.IsValidFolder("Assets/" + "Animator Controllers"))
        {
            AssetDatabase.CreateFolder("Assets", "Animator Controllers");
        }
        Move("controller");
    }

    [MenuItem("[Custom Tools]/Organize/Sprites", false, 52)]
    static void OrganizeSprites()
    {
        allAssetPaths = AssetDatabase.GetAllAssetPaths();
        destinationPath = "Assets/Sprites/";
        //Check if a sprite folder exists. If not, create one.
        if (!AssetDatabase.IsValidFolder("Assets/" + "Sprites"))
        {
            AssetDatabase.CreateFolder("Assets", "Sprites");
        }
        Move(".png");
        Move(".jpg");
    }

    [MenuItem("[Custom Tools]/Organize/Audio", false, 52)]
    static void OrganizeAudio()
    {
        allAssetPaths = AssetDatabase.GetAllAssetPaths();
        destinationPath = "Assets/Audio/";
        //Check if a sprite folder exists. If not, create one.
        if (!AssetDatabase.IsValidFolder("Assets/" + "Audio"))
        {
            AssetDatabase.CreateFolder("Assets", "Audio");
        }
        Move(".mp3");
        Move(".wav");
        Move(".ogg");
    }

    static void Move(string _extension)
    {
        for (int i = 0; i < allAssetPaths.Length; i++)
        {
            //get the extension of all the assets
            //we'll check the extensions to find the type of the asset
            extension = allAssetPaths[i].Substring(allAssetPaths[i].LastIndexOf(".") + 1);
            //store the name of every asset
            assetName = allAssetPaths[i].Substring(allAssetPaths[i].LastIndexOf("/") + 1);
            //for scenes
            //if extension is .unity and the script is not the Tools scripts (this script) and the script is not already in the destination path.		
            if (extension.Equals(_extension) && assetName != "Tools.cs" && allAssetPaths[i] != destinationPath + assetName)
            {
                AssetDatabase.MoveAsset(allAssetPaths[i], destinationPath + assetName);
                Debug.Log("<color=green>" + assetName + "</color> moved from <color=red>" + allAssetPaths[i] + "</color> to <color=cyan>" + destinationPath + assetName + "</color>");
            }
        }
    }

    /// <summary>
    /// De-activate an active the selectedgameobject in the hierarchy vice-versa.
    /// Works with multiple gameobjects.
    /// </summary>
    [MenuItem("GameObject/[Custom Tools]/Mark", false, 0)]
    static void Mark()
    {
        if (Selection.gameObjects.Length == 1)
        {
            Debug.Log(Selection.activeGameObject.name);


        }

        else
        {
            Debug.LogWarning("Select ONLY one game object!");
        }
    }

    /// <summary>
    /// De-activate an active the selectedgameobject in the hierarchy vice-versa.
    /// Works with multiple gameobjects.
    /// </summary>
    [MenuItem("GameObject/[Custom Tools]/Activate - Deactivate #a", false, 0)]
    static void SwitchStatus()
    {
        if (Selection.gameObjects.Length != 0)
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                if (go.activeInHierarchy)
                {
                    go.SetActive(false);
                }
                else
                {
                    go.SetActive(true);
                }
            }
        }

        else
        {
            Debug.LogWarning("Select at least one game object!");
        }
    }

    /// <summary>
    /// Parents all the selected gameobjects to a parent gameobject
    /// </summary>
    [MenuItem("GameObject/[Custom Tools]/Create Parent", false, 0)]
    static void CreateParent()
    {
        if (Selection.gameObjects.Length != 0)
        {
            //create a new gameobject
            GameObject parent = new GameObject();
            parent.name = "_NewParent";
            //set all the selected gameobjects as a child of the newly created game object
            foreach (GameObject go in Selection.gameObjects)
            {
                go.transform.parent = parent.transform;
            }
            //workaround to delete extra parent gameobjects that are created for each selected gameobject
            foreach (GameObject allGo in Object.FindObjectsOfType(typeof(GameObject)))
            {
                if (allGo.name == "_NewParent" && allGo.transform.childCount == 0)
                {
                    DestroyImmediate(allGo);
                }
            }
        }

        else
        {
            Debug.LogWarning("Select at least one game object!");
        }
    }


    /// <summary>
    /// Delete all PlayerPrefs 
    /// </summary>
    [MenuItem("[Custom Tools]/Delete the PlayerPrefs", false, 51)]
    static void DeletePlayerPrefs()
    {
        if (EditorUtility.DisplayDialog("Delete All PlayerPrefs?", "Do you really want to delete all the PlayerPrefs?\nOnce deleted, they can't be restored!", "Yes", "No") == true)
        {
            PlayerPrefs.DeleteAll();
        }
    }

    /// <summary>
    /// Toggle Canvas 
    /// </summary>
    [MenuItem("[Custom Tools]/Toggle Canvas #c", false)]
    static void ToggleCanvas()
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            go.GetComponent<Canvas>().enabled = !go.GetComponent<Canvas>().enabled;

        }
        EditorApplication.RepaintHierarchyWindow();
    }



    [MenuItem("GameObject/[Custom Tools]/Collapse #`", false, 52)]
    static void Collapse()
    {
        GameObject go = Selection.gameObjects[0];
        var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
        var methodInfo = type.GetMethod("SetExpandedRecursive");
        // EditorApplication.ExecuteMenuItem("Window/Hierarchy");
        var window = EditorWindow.focusedWindow;
        methodInfo.Invoke(window, new object[] { go.GetInstanceID(), false });
    }

    [MenuItem("GameObject/[Custom Tools]/Select Child #1", false, 52)]
    static void SelectChildren()
    {
        if (Selection.gameObjects.Length != 0)
        {
            GameObject[] gArr = new GameObject[Selection.transforms.Length];

            for (int i = 0; i < gArr.Length; i++)
            {
                gArr[i] = Selection.transforms[i].GetChild(0).gameObject;
            }
            Selection.objects = gArr;
        }
        else
        {
            Debug.LogWarning("Select at least one game object!");
        }
    }

    [MenuItem("GameObject/[Custom Tools]/Select Next Sibling #2", false, 52)]
    static void SelectNextSibling()
    {
        if (Selection.gameObjects.Length != 0)
        {
            GameObject[] gArr = new GameObject[Selection.transforms.Length];
            for (int i = 0; i < gArr.Length; i++)
            {
                int index = Selection.transforms[i].GetSiblingIndex();
                if (index < Selection.transforms[i].parent.childCount)
                    gArr[i] = (Selection.transforms[i].parent.GetChild(index + 1)).gameObject;
            }
            Selection.objects = gArr;
        }
        else
        {
            Debug.LogWarning("Select at least one game object!");
        }
    }

    [MenuItem("GameObject/[Custom Tools]/Select Previous Sibling #3", false, 52)]
    static void SelectPreviousSibling()
    {
        if (Selection.gameObjects.Length != 0)
        {
            GameObject[] gArr = new GameObject[Selection.transforms.Length];
            for (int i = 0; i < gArr.Length; i++)
            {
                //get current selection index
                int index = Selection.transforms[i].GetSiblingIndex();

                //if not the first gameobject
                if (index > 0)
                {
                    gArr[i] = (Selection.transforms[i].parent.GetChild(index - 1)).gameObject;

                }

                //if the first child
                else
                {
                    gArr[i] = Selection.transforms[i].parent.transform.gameObject;

                }
            }
            Selection.objects = gArr;
        }
        else
        {
            Debug.LogWarning("Select at least one game object!");
        }
    }



    public static string fileName = "Editor Screenshot ";
    public static int startNumber = 1;

    [MenuItem("[Custom Tools]/Take Screenshot of Game View #l")]
    static void TakeScreenshot()
    {
        int number = startNumber;
        string name = "" + number;

        while (System.IO.File.Exists(fileName + name + ".png"))
        {
            number++;
            name = "" + number;
        }

        startNumber = number + 1;

        ScreenCapture.CaptureScreenshot(fileName + name + ".png");


    }
}