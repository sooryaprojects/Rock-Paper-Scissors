using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SceneSwitcher : EditorWindow
{
	private Vector2 scrollPos;

	[MenuItem("[Custom Tools]/Scene Switcher", false, 102)]
	internal static void Init()
	{

		var window = (SceneSwitcher)GetWindow(typeof(SceneSwitcher), false, "Scene Switcher");
		window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 100f);
	}

	void OnGUI()
	{
		EditorGUILayout.BeginVertical(GUI.skin.box);
		this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos, false, false);

		GUILayout.Label("Scenes In Build", EditorStyles.boldLabel);
		for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
		{
			var scene = EditorBuildSettings.scenes[i];
			if (scene.enabled)
			{
				var sceneName = Path.GetFileNameWithoutExtension(scene.path);
				var pressed = GUILayout.Button(i + ": " + sceneName, new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleLeft });
				if (pressed)
				{
					if (EditorApplication.SaveCurrentSceneIfUserWantsTo())
					{
						EditorApplication.OpenScene(scene.path);
					}
				}
			}
		}

		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}
}
