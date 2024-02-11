using System.IO;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR

public static class SceneLister
{
    public static Dictionary<string, HashSet<string>> CheckAndListLevels(string searchRootFolder)
    {
        Dictionary<string, HashSet<string>> levelScenes = new Dictionary<string, HashSet<string>>();
        RecursivelyCollectScenes(new DirectoryInfo(searchRootFolder), ref levelScenes);
        return levelScenes;
    }

    private static void RecursivelyCollectScenes(DirectoryInfo directoryInfo, ref Dictionary<string, HashSet<string>> levelScenes)
    {
        foreach (FileInfo file in directoryInfo.GetFiles("*.unity"))
        {
            string sceneName = Path.GetFileNameWithoutExtension(file.Name);
            string levelName = ExtractLevelName(sceneName);
            string sceneType = ExtractSceneType(sceneName);

            if (!string.IsNullOrEmpty(levelName) && !string.IsNullOrEmpty(sceneType))
            {
                if (!levelScenes.ContainsKey(levelName))
                {
                    levelScenes[levelName] = new HashSet<string>();
                }
                levelScenes[levelName].Add(sceneType);
            }
        }

        foreach (DirectoryInfo subDirectory in directoryInfo.GetDirectories())
        {
            RecursivelyCollectScenes(subDirectory, ref levelScenes);
        }
    }

    private static string ExtractLevelName(string sceneName)
    {
        if (sceneName.EndsWith("Foreground") || sceneName.EndsWith("Background") || sceneName.EndsWith("Collider"))
        {
            int lastIndex = sceneName.LastIndexOf("Foreground");
            if (lastIndex == -1) lastIndex = sceneName.LastIndexOf("Background");
            if (lastIndex == -1) lastIndex = sceneName.LastIndexOf("Collider");
            if (lastIndex > -1)
            {
                return sceneName.Substring(0, lastIndex);
            }
        }
        return null;
    }

    private static string ExtractSceneType(string sceneName)
    {
        if (sceneName.EndsWith("Foreground"))
        {
            return "Foreground";
        }
        else if (sceneName.EndsWith("Background"))
        {
            return "Background";
        }
        else if (sceneName.EndsWith("Collider"))
        {
            return "Collider";
        }
        return null;
    }
}
#endif
