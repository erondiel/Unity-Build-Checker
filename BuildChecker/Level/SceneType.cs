using System.Collections.Generic;
using BuildChecker.Checker;

namespace BuildChecker.Level
{
    public enum SceneType
    {
        Foreground,
        Background,
        Colliders,
    }
    
    public static class SceneTypeExtensions
    {
        public static List<ICheckScenes> GetSceneCheckers(this SceneType type)
        {
            return type switch
            {
                SceneType.Foreground => new List<ICheckScenes> { new Foreground1Checker() },
                SceneType.Background => new List<ICheckScenes> { new Background1Checker() },
                SceneType.Colliders => new List<ICheckScenes> { new Collider1Checker(), new Collider2Checker() },
                _ => new List<ICheckScenes>()
            };
        }
    }
}