using System.Collections.Generic;
using BuildChecker.Level;

namespace BuildChecker.Checker
{
    public class Background1Checker : ICheckScenes
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

            if (info.Level.Name == "TestScene")
            {
                return builder
                    .Error("invalid xxx")
                    .Error("invalid yyy")
                    .Error("invalid zzz")
                    .Error("invalid xxx")
                    .Error("invalid yyy")
                    .Error("invalid zzz")
                    .Error("invalid xxx")
                    .BuildAll();
            }

            return builder
                .Error("invalid object")
                .BuildAll();
        }

        public string Name => "Background Checker";
    }
}