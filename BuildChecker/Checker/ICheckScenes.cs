using System.Collections.Generic;
using BuildChecker.Level;
using UnityEngine;

namespace BuildChecker.Checker
{
    public interface ICheckScenes
    {
        public List<ValidationError> Validate(LevelSceneInfo info);
    
        public string Name { get; }
    }

    public class ValidationError
    {
        public string Message { get; set; }
        public LevelSceneInfo Scene { get; set; }
        public ICheckScenes Checker { get; set; }
    
        public GameObject GameObject { get; set; }
    }

    public class ValidationErrorBuilder
    {
        private readonly List<ValidationError> _errors = new();
        private ValidationError _currentError = new();

        public ValidationErrorBuilder Error(string message)
        {
            _currentError.Message = message;
            _errors.Add(_currentError);
            _currentError = new ValidationError
            {
                Scene = _currentError.Scene,
                Checker = _currentError.Checker
            };
            return this;
        }

        public ValidationErrorBuilder ForScene(LevelSceneInfo scene)
        {
            _currentError.Scene = scene;
            return this;
        }

        public ValidationErrorBuilder ByChecker(ICheckScenes checker)
        {
            _currentError.Checker = checker;
            return this;
        }

        public List<ValidationError> BuildAll()
        {
            if (_currentError.Message != null)
            {
                _errors.Add(_currentError);
            }
            return _errors;
        }
    }
}