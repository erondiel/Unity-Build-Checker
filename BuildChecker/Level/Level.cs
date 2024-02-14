using System.Collections.Generic;
using System.Linq;
using BuildChecker.Checker;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace BuildChecker.Level
{
    public class Level
    {
        public bool IsValid => !ErrorsByChecker.Values.SelectMany(v => v).Any();

        public string Name { get; set; }

        public readonly Dictionary<SceneType, LevelSceneInfo> Scenes = new();
        
        public readonly Dictionary<string, List<ValidationError>> ErrorsByChecker = new();

        public Level(string name)
        {
            Name = name;
        }

        public void AddScene(SceneType type, Scene scene)
        {
            Scenes[type] = new LevelSceneInfo
            {
                Scene = scene,
                Level = this,
                Type = type
            };
        }
        
        public void Validate()
        {
            IsValidated = true;
            ErrorsByChecker.Clear();
            
            foreach (var sceneType in Scenes.Keys)
            {
                var checkers = sceneType.GetSceneCheckers();
                foreach (var checker in checkers)
                {
                    var errors = checker.Validate(Scenes[sceneType]);
                    if (!errors.Any()) 
                        continue;
                    
                    if (!ErrorsByChecker.ContainsKey(checker.Name))
                        ErrorsByChecker[checker.Name] = new List<ValidationError>();

                    ErrorsByChecker[checker.Name].AddRange(errors);
                }
            }

            var checkerName = "Level Checks";
            var validator = new MissingScenesLevelValidator();
            var missingSceneErrors = validator.Validate(this);
            if (missingSceneErrors.Any())
            {
                if (!ErrorsByChecker.ContainsKey(checkerName))
                    ErrorsByChecker[checkerName] = new List<ValidationError>();
                ErrorsByChecker[checkerName].AddRange(missingSceneErrors);
            }
        }
        
        private void CheckLocalizationKey()
        {
            var folderPath = "Assets/Game/Data/Localization/Maps";

            var localizationFiles = AssetDatabase.FindAssets("t:LocalizationKey", new[] { folderPath });

            const string checkerName = "Level Checks";

            if (localizationFiles.Length == 0)
            {
                var error = new ValidationError
                {
                    Message = $"No localization data found"
                };

                if (!ErrorsByChecker.ContainsKey(checkerName))
                {
                    ErrorsByChecker[checkerName] = new List<ValidationError>();
                }
                ErrorsByChecker[checkerName].Add(error);
            }
        }

        private void CheckMapSceneData()
        {
            var folderPath = "Assets/Game/Data/Maps";

            var mapDataFiles = AssetDatabase.FindAssets("t:MapSceneData", new[] { folderPath });

            const string checkerName = "Level Checks";

            if (mapDataFiles.Length == 0)
            {
                var error = new ValidationError
                {
                    Message = $"No map scene data found"
                };

                if (!ErrorsByChecker.ContainsKey(checkerName))
                {
                    ErrorsByChecker[checkerName] = new List<ValidationError>();
                }
                ErrorsByChecker[checkerName].Add(error);
            }
        }

        public bool IsComplete => HasScene(SceneType.Foreground) && HasScene(SceneType.Background) && HasScene(SceneType.Colliders);
       
        public bool IsValidated { get; private set; }

        public bool HasScene(SceneType sceneType)
        {
            return Scenes.ContainsKey(sceneType);
        }
    }
}