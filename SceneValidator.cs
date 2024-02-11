using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Test 
{
    public interface ISceneValidator
    {
        bool Validate();
        List<ValidationError> Errors { get; }
    }

    public class WayPointValidator : ISceneValidator
    {
        public List<ValidationError> Errors { get; private set; }

        public bool Validate()
        {
            Errors.Add(new ValidationError() { Message = "Waypoint is missing", GameObject = new GameObject() });

            return Errors.Count == 0;
        }
    }

   using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necessary for Scene management

namespace Test
{
    public class ColliderValidator : ISceneValidator
    {
        public Scene Scene { get; private set; }
        
        public List<ValidationError> Errors { get; private set; }

        public ColliderValidator(Scene scene)
        {
            Scene = scene;
            Errors = new List<ValidationError>();
        }

        public bool Validate()
        {
            foreach (var rootGameObject in Scene.GetRootGameObjects())
            {
                ValidateGameObject(rootGameObject); 
            }

            return Errors.Count == 0; 
        }

        private void ValidateGameObject(GameObject gameObject)
            var colliders = gameObject.GetComponents<Collider>();
            foreach (var collider in colliders)
            {
                if (!collider.enabled)
                {

                    collider.enabled = true;
                    Errors.Add(new ValidationError()
                    {
                        Message = $"GameObject '{gameObject.name}' has a disabled Collider.",
                        GameObject = gameObject
                    });
                }
            }

            foreach (Transform child in gameObject.transform)
            {
                ValidateGameObject(child.gameObject);
            }
        }
    }


    public class ValidationError
    {
        public string Message { get; set; }
        public GameObject GameObject { get; set; }
    }
}

