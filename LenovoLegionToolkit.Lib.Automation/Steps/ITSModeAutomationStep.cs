using System;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Features;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

[method: JsonConstructor]
public class ITSModeAutomationStep(ITSMode state)
    : AbstractFeatureAutomationStep<ITSMode>(state)
{
    public override IAutomationStep DeepCopy() => new ITSModeAutomationStep(State);

    public override async Task RunAsync(AutomationContext context, AutomationEnvironment environment, CancellationToken token)
    {
        await WaitForStableModeAsync(token).ConfigureAwait(false);
        await base.RunAsync(context, environment, token).ConfigureAwait(false);
    }

    private static async Task WaitForStableModeAsync(CancellationToken token)
    {
        var stableFor = TimeSpan.FromMilliseconds(500);
        var minimumWait = TimeSpan.FromSeconds(5);
        var timeout = TimeSpan.FromSeconds(8);
        var deadline = DateTimeOffset.UtcNow.Add(timeout);
        var startTime = DateTimeOffset.UtcNow;

        var lastMode = await ITSModeFeature.GetITSModeEx().ConfigureAwait(false);
        var lastChanged = DateTimeOffset.UtcNow;

        while (DateTimeOffset.UtcNow < deadline && !token.IsCancellationRequested)
        {
            await Task.Delay(200, token).ConfigureAwait(false);

            var currentMode = await ITSModeFeature.GetITSModeEx().ConfigureAwait(false);
            if (currentMode != lastMode)
            {
                lastMode = currentMode;
                lastChanged = DateTimeOffset.UtcNow;
            }
            else if (DateTimeOffset.UtcNow - lastChanged >= stableFor
                  && DateTimeOffset.UtcNow - startTime >= minimumWait)
            {
                return;
            }
        }
    }
}
