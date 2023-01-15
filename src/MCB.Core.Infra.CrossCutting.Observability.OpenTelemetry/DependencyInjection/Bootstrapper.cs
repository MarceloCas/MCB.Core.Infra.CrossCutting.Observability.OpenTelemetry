using MCB.Core.Infra.CrossCutting.DependencyInjection.Abstractions.Interfaces;
using MCB.Core.Infra.CrossCutting.Observability.Abstractions;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace MCB.Core.Infra.CrossCutting.Observability.OpenTelemetry.DependencyInjection;

public static class Bootstrapper
{
    // Public Methods
    public static void ConfigureDependencyInjection(
        IDependencyInjectionContainer dependencyInjectionContainer,
        string applicationName,
        string? applicationVersion
    )
    {
        dependencyInjectionContainer.RegisterSingleton(d => new ActivitySource(applicationName));
        dependencyInjectionContainer.RegisterSingleton<ITraceManager, TraceManager>();
        dependencyInjectionContainer.RegisterSingleton(d => new Meter(applicationName, applicationVersion));
        dependencyInjectionContainer.RegisterSingleton<IMetricsManager>(d => new MetricsManager(d.Resolve<Meter>()!));
    }
}