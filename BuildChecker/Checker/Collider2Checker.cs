using System.Collections.Generic;
using BuildChecker.Level;

namespace BuildChecker.Checker
{
    public class Collider2Checker : ICheckScenes
    {
        public List<ValidationError> Validate(LevelSceneInfo info)
        {
            var builder = new ValidationErrorBuilder()
                .ForScene(info)
                .ByChecker(this);

            if (info.Level.Name == "ValidScene")
            {
                return builder.BuildAll();
            }

            return builder.Error("invalid blabla").BuildAll();
        }
    
        public string Name => "Collider Checker 2";
    }
}