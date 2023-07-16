using Microsoft.Extensions.Options;

namespace Santander.StoryApplication.WebApplication.Options;

internal sealed class PeriodicTimerOptionsValidator :
    IValidateOptions<PeriodicTimerOptions> {
    public ValidateOptionsResult Validate(string? name, PeriodicTimerOptions options) {
        if (options.Period <= TimeSpan.Zero) {
            return ValidateOptionsResult.Fail(failureMessage: $"{nameof(options.Period)} must be greater than zero");
        }

        return ValidateOptionsResult.Success;
    }
}
