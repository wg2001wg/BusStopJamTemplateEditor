﻿using UnityEngine.SceneManagement;

namespace Watermelon
{
    public static class SceneUtils
    {
        /// <summary>
        /// Returns true if the scene 'name' exists and is in your Build settings, false otherwise
        /// </summary>
        public static bool DoesSceneExist(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                var lastSlash = scenePath.LastIndexOf("/");
                var sceneName = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);

                if (string.Compare(name, sceneName, true) == 0)
                    return true;
            }

            return false;
        }
    }
}