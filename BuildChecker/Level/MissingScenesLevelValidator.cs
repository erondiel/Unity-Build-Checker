using System;
using System.Collections.Generic;
using System.Linq;
using BuildChecker.Checker;

namespace BuildChecker.Level
{
    public class MissingScenesLevelValidator : IValidateLevels
    {
        public List<ValidationError> Validate(BuildChecker.Level.Level level)
        {
            if (level.IsComplete) 
                return new List<ValidationError>();
            
            var errors = new List<ValidationError>();

            var missingScenes = Enum.GetValues(typeof(SceneType))
                .Cast<SceneType>()
                .Where(st => !level.HasScene(st))
                .Select(st => st.ToString());

            var missingScenesString = string.Join(" | ", missingScenes);
            var generalError = new ValidationError
            {
                Message = $"missing: {missingScenesString}",
            };

            errors.Add(generalError);

            return errors;
        }
    }
}