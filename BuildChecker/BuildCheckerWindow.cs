using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildChecker.Level;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BuildChecker
{
    public class BuildCheckerWindow : EditorWindow
    {
        private readonly List<Level.Level> _levels = new();
        private Vector2 _scrollPosition;
        private readonly Dictionary<string, bool> _foldoutStates = new();
        private const string BaseFolderPath = "Assets/Scenes/";

        [MenuItem("Tools/Build Checker")]
        public static void ShowWindow()
        {
            var window = GetWindow<BuildCheckerWindow>("Build Checker");
            window.PopulateLevels();
            window.minSize = new Vector2(400, 300);
            window.maxSize = new Vector2(400, 600); 
        }
        
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); 
            if (GUILayout.Button("Refresh", GUILayout.Width(100), GUILayout.Height(20)))
            {
                Refresh();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
    
            GUILayout.Space(10);
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(400), GUILayout.Height(600));

            GUILayout.BeginHorizontal();
            GUILayout.Space(20); 
    
            EditorGUILayout.BeginVertical();
    
            foreach (var level in _levels)
            {
                level.Validate();
                DrawLevelFoldout(level);
            }

            EditorGUILayout.EndVertical();
            GUILayout.EndHorizontal();
            
            GUILayout.EndScrollView();
        }

        private void DrawLevelFoldout(Level.Level level)
        {
            _foldoutStates.TryAdd(level.Name, false);
            var foldoutStyle = GetFoldoutStyle(level.IsValid ? Color.green : Color.red, FontStyle.Bold, 14);
            
            _foldoutStates[level.Name] = EditorGUILayout.Foldout(_foldoutStates[level.Name], level.Name, true, foldoutStyle);

            if (!_foldoutStates[level.Name]) return;
            
            EditorGUI.indentLevel++;
            
            if (level.ErrorsByChecker.TryGetValue("Level Checks", out var generalErrors))
            {
                foreach (var error in generalErrors)
                {
                    DrawError(error.Message);
                }
            }
            
            DrawSceneTypesFoldout(level);
            EditorGUI.indentLevel--;
        }

        private void DrawSceneTypesFoldout(Level.Level level)
        {
            foreach (var sceneType in Enum.GetValues(typeof(SceneType)).Cast<SceneType>())
            {
                if (!level.HasScene(sceneType)) continue;

                var sceneTypeKey = $"{level.Name}_{sceneType}";
                _foldoutStates.TryAdd(sceneTypeKey, false);

                var hasErrors = level.ErrorsByChecker.Any(e => e.Value.Any(err => err.Scene?.Type == sceneType));
                var sceneTypeStyle = GetFoldoutStyle(hasErrors ? Color.red : Color.green, FontStyle.Bold, 14);

                _foldoutStates[sceneTypeKey] = EditorGUILayout.Foldout(_foldoutStates[sceneTypeKey], sceneType.ToString(), true, sceneTypeStyle);

                if (!_foldoutStates[sceneTypeKey]) continue;

                EditorGUI.indentLevel++;
                DrawCheckerErrors(level, sceneType);
                EditorGUI.indentLevel--;
            }
        }

        private void DrawCheckerErrors(Level.Level level, SceneType sceneType)
        {
            foreach (var checkerEntry in level.ErrorsByChecker)
            {
                var checkerErrors = checkerEntry.Value.Where(e => e.Scene?.Type == sceneType).ToList();
                if (!checkerErrors.Any()) continue;

                var checkerKey = $"{level.Name}_{sceneType}_{checkerEntry.Key}";
                _foldoutStates.TryAdd(checkerKey, false);

                EditorGUILayout.BeginHorizontal();

                var checkerStyle = GetFoldoutStyle(Color.red, FontStyle.BoldAndItalic, 12);
                _foldoutStates[checkerKey] = EditorGUILayout.Foldout(_foldoutStates[checkerKey], checkerEntry.Key, true, checkerStyle);

                if (GUILayout.Button("Fix", GUILayout.Width(50), GUILayout.Height(18)))
                {
                    FixCheckerErrors(level, sceneType, checkerEntry.Key);
                }

                EditorGUILayout.EndHorizontal();

                if (_foldoutStates[checkerKey])
                {
                    EditorGUI.indentLevel++;
                    foreach (var error in checkerErrors)
                    {
                        DrawError(error.Message);
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }


        private static void FixCheckerErrors(Level.Level level, SceneType sceneType, string checkerKey)
        {
            Debug.Log($"Fixing errors for checker: {checkerKey} in {level.Name} - {sceneType}");
        }

        private static void DrawError(string message)
        {
            var errorStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Normal,
                fontSize = 12,
                normal = { textColor = Color.white }
            };
            EditorGUILayout.LabelField(message, errorStyle);
        }

        private GUIStyle GetFoldoutStyle(Color textColor, FontStyle fontStyle, int fontSize)
        {
            return new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = fontStyle,
                fontSize = fontSize,
                normal = { textColor = textColor },
                onNormal = { textColor = textColor },
                focused = { textColor = textColor },
                onFocused = { textColor = textColor },
                active = { textColor = textColor },
                onActive = { textColor = textColor }
            };
        }

        private void Refresh()
        {
            PopulateLevels();
            foreach (var level in _levels)
            {
                level.Validate();
            }
        }

        private void PopulateLevels()
        {
            _levels.Clear();
            var scenePaths = ScenePaths();
            var levelMap = new Dictionary<string, Level.Level>();
            
            foreach (var scenePath in scenePaths)
            {
                var levelName = GetLevelNameFromScene(scenePath);
                if (!levelMap.ContainsKey(levelName))
                {
                    levelMap[levelName] = new Level.Level(levelName);
                }
                
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

                if (scenePath.Contains("Foreground")) levelMap[levelName].AddScene(SceneType.Foreground, scene);
                if (scenePath.Contains("Background")) levelMap[levelName].AddScene(SceneType.Background, scene);
                if (scenePath.Contains("Colliders")) levelMap[levelName].AddScene(SceneType.Colliders, scene);
            }
            
            _levels.AddRange(levelMap.Values);
        }

        private static IEnumerable<string> ScenePaths()
        {
            return Directory.GetFiles(BaseFolderPath, "*.unity", SearchOption.AllDirectories);
        }

        private static string GetLevelNameFromScene(string scenePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(scenePath);
            return fileName.Replace("Foreground", "").Replace("Background", "").Replace("Colliders", "");
        }
    }
}