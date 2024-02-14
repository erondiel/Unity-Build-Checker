using System.Collections.Generic;

namespace BuildChecker.Checker
{
    public class Foreground1Checker : ICheckScenes
    {
        public List<ValidationError> Validate(LevelSceneInfo info)
        {
            var builder = new ValidationErrorBuilder()
                .ForScene(info)
                .ByChecker(this);
        
            if (info.Level.Name != "ValidScene")
            {
                builder
                    .Error("invalid sbrables")
                    .Error("invalid foobar")
                    .Error("XXXXXXX");
            }

            return builder.BuildAll();
        }

        public string Name => "Foreground Checker 1";
    }
}