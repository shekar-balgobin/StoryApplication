using AutoFixture.Xunit2;
using FluentAssertions;
using Santander.StoryApplication.WebApplication.Options;

namespace Santander.StoryApplication.WebApplication.TestProject.Options;

public sealed class PeriodicTimerOptionsValidatorTest {
    [InlineAutoData(false, -1)]
    [InlineAutoData(false, 0)]
    [InlineAutoData(true, 1)]
    [Theory]
    public void Validate(bool expected, long ticks, string name) {
        var periodicTimerOptions = new PeriodicTimerOptions {
            Period = TimeSpan.FromTicks(ticks)
        };

        var periodicTimerOptionsValidator = new PeriodicTimerOptionsValidator();
        var actual = periodicTimerOptionsValidator.Validate(name, periodicTimerOptions);

        actual.Succeeded.Should().Be(expected);
        actual.FailureMessage.Should().Be(actual.Succeeded ? null : "Period must be greater than zero");
    }
}
