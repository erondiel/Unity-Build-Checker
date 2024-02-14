using System.Collections.Generic;
using BuildChecker.Checker;

namespace BuildChecker.Level
{
    public interface IValidateLevels
    {
        List<ValidationError> Validate(Level level);
    }
}