using MCB.Core.Infra.CrossCutting.DependencyInjection.Abstractions.Interfaces;
using System.Diagnostics;

namespace MCB.Core.Infra.CrossCutting.Observability.OpenTelemetry.DependencyInjection;

public static class Bootstrapper
{
    // Public Methods
    public static void ConfigureDependencyInjection(
        IDependencyInjectionContainer dependencyInjectionContainer,
        string applicationName
    )
    {
        dependencyInjectionContainer.RegisterSingleton(d => new ActivitySource(applicationName));
    }
}