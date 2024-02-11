using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class BuildCheckerWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private Dictionary<string, HashSet<string>> levels;

    [MenuItem("Tools/Build Checker")]
    public static void ShowWindow()
    {
        GetWindow<SceneListerWindow>("/Build Checker");
    }

    void OnGUI()
    {
        GUILayout.Label("Build Checker", EditorStyles.boldLabel);
        if (GUILayout.Button("List Levels"))
        {
            levels = SceneLister.CheckAndListLevels("Assets/Scenes");
        }

        if (levels != null)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            foreach (KeyValuePair<string, HashSet<string>> level in levels)
            {
                string completeness = level.Value.Count == 3 ? "Complete" : "Incomplete";
                GUILayout.Label($"{level.Key}: {completeness}");

                foreach (string sceneType in level.Value)
                {
                    GUILayout.Label($"- {sceneType}");
                }
            }
            GUILayout.EndScrollView();
        }
    }
}
