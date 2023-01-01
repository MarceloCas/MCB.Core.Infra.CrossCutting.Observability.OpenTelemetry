using MCB.Core.Infra.CrossCutting.Observability.Abstractions;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace MCB.Core.Infra.CrossCutting.Observability.OpenTelemetry;
public class TraceManager
    : ITraceManager
{
    // Fields
    private readonly ActivitySource _activitySource;

    // Constructors
    public TraceManager(ActivitySource activitySource)
    {
        _activitySource = activitySource;
    }

    // Public Methods
    public void StartActivity(string name, ActivityKind kind, Guid correlationId, Guid tenantId, string? executionUser, string? sourcePlatform, Action<Activity> handler)
    {
        using var activity = _activitySource.StartActivity(name, kind);

        if (activity is null)
            return;

        SetDefaultActivityTags(activity, correlationId, tenantId, executionUser, sourcePlatform);

        try
        {
            activity.SetStatus(Status.Ok);
            handler(activity);
        }
        catch (Exception ex)
        {
            activity.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity.RecordException(ex);
            throw;
        }
    }
    public void StartActivity<TInput>(string name, ActivityKind kind, Guid correlationId, Guid tenantId, string? executionUser, string? sourcePlatform, TInput? input, Action<TInput?, Activity> handler)
    {
        using var activity = _activitySource.StartActivity(name, kind);

        if (activity is null)
            return;

        SetDefaultActivityTags(activity, correlationId, tenantId, executionUser, sourcePlatform);

        try
        {
            activity.SetStatus(Status.Ok);
            handler(input, activity);
        }
        catch (Exception ex)
        {
            activity.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity.RecordException(ex);
            throw;
        }
    }
    public TOutput? StartActivity<TInput, TOutput>(string name, ActivityKind kind, Guid correlationId, Guid tenantId, string? executionUser, string? sourcePlatform, TInput? input, Func<TInput?, Activity, TOutput?> handler)
    {
        using var activity = _activitySource.StartActivity(name, kind);

        if (activity is null)
            return default;

        SetDefaultActivityTags(activity, correlationId, tenantId, executionUser, sourcePlatform);

        try
        {
            activity.SetStatus(Status.Ok);
            return handler(input, activity);
        }
        catch (Exception ex)
        {
            activity.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity.RecordException(ex);
            throw;
        }
    }

    public Task StartActivityAsync(string name, ActivityKind kind, Guid correlationId, Guid tenantId, string? executionUser, string? sourcePlatform, Func<Activity, CancellationToken, Task> handler, CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity(name, kind);

        if (activity is null)
            return Task.CompletedTask;

        SetDefaultActivityTags(activity, correlationId, tenantId, executionUser, sourcePlatform);

        try
        {
            activity.SetStatus(Status.Ok);
            return handler(activity, cancellationToken);
        }
        catch (Exception ex)
        {
            activity.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity.RecordException(ex);
            throw;
        }
    }
    public Task StartActivityAsync<TInput>(string name, ActivityKind kind, Guid correlationId, Guid tenantId, string? executionUser, string? sourcePlatform, TInput? input, Func<TInput?, Activity, CancellationToken, Task> handler, CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity(name, kind);

        if (activity is null)
            return Task.CompletedTask;

        SetDefaultActivityTags(activity, correlationId, tenantId, executionUser, sourcePlatform);

        try
        {
            activity.SetStatus(Status.Ok);
            return handler(input, activity, cancellationToken);
        }
        catch (Exception ex)
        {
            activity.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity.RecordException(ex);
            throw;
        }
    }
    public Task<TOutput?> StartActivityAsync<TInput, TOutput>(string name, ActivityKind kind, Guid correlationId, Guid tenantId, string? executionUser, string? sourcePlatform, TInput? input, Func<TInput?, Activity, CancellationToken, Task<TOutput?>> handler, CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity(name, kind);

        if (activity is null)
            return Task.FromResult(default(TOutput));

        SetDefaultActivityTags(activity, correlationId, tenantId, executionUser, sourcePlatform);

        try
        {
            activity.SetStatus(Status.Ok);
            return handler(input, activity, cancellationToken);
        }
        catch (Exception ex)
        {
            activity.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity.RecordException(ex);
            throw;
        }
    }

    // Private Methods
    private static void SetDefaultActivityTags(Activity activity, Guid correlationId, Guid tenantId, string? executionUser, string? sourcePlatform)
    {
        activity.SetTag(ITraceManager.CORRELATION_ID_TAG_NAME, correlationId);
        activity.SetTag(ITraceManager.TENANT_ID_TAG_NAME, tenantId);
        activity.SetTag(ITraceManager.EXECUTION_USER_TAG_NAME, executionUser);
        activity.SetTag(ITraceManager.SOURCE_PLATFORM_TAG_NAME, sourcePlatform);
    }
}
