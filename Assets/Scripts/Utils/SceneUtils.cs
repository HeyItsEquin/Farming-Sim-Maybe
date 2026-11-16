using System;
using UnityEngine.SceneManagement;

public static class SceneUtils
{
    /// <summary>
    /// Returns true if the scene 'name' exists and is in your Build settings, false otherwise
    /// </summary>
    public static bool SceneExists(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;

        for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            var lastSlash = scenePath.LastIndexOf("/");
            var sceneName = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);

            if (name.Equals(sceneName,StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}