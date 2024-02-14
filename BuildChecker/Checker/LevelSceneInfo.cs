using BuildChecker.Level;

namespace BuildChecker.Checker
{
    public class LevelSceneInfo
    {
        public UnityEngine.SceneManagement.Scene Scene { get; set; }
        public Level.Level Level { get; set; }
        public SceneType Type { get; set; }
    }
}