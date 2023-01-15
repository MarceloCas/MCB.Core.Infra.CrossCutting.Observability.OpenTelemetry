using MCB.Core.Infra.CrossCutting.Observability.Abstractions;
using MCB.Core.Infra.CrossCutting.Observability.Abstractions.Models;

namespace MCB.Core.Infra.CrossCutting.Observability.OpenTelemetry;
public class MetricsManager
    : IMetricsManager
{
    // Constants
    public const string COUNTER_ALREADY_EXISTS_ERROR_MESSAGE = "Counter already exists [name:{0}]";
    public const string COUNTER_NOT_FOUND_ERROR_MESSAGE = "Counter not found [name:{0}]";

    public const string HISTOGRAM_ALREADY_EXISTS_ERROR_MESSAGE = "Histogram already exists [name:{0}]";
    public const string HISTOGRAM_NOT_FOUND_ERROR_MESSAGE = "Histogram not found [name:{0}]";

    public const string OBSERVABLE_GAUGE_ALREADY_EXISTS_ERROR_MESSAGE = "Observable gauge already exists [name:{0}]";
    public const string OBSERVABLE_GAUGE_NOT_FOUND_ERROR_MESSAGE = "Observable gauge not found [name:{0}]";

    // Fields
    private readonly System.Diagnostics.Metrics.Meter _meter = null!;
    private readonly Dictionary<string, object> _counterDictionary;
    private readonly List<Counter> _counterColection;

    private readonly Dictionary<string, object> _histogramDictionary;
    private readonly List<Histogram> _histogramColection;

    private readonly Dictionary<string, object> _observableGaugeDictionary;
    private readonly List<ObservableGauge> _observableGaugeColection;

    // Constructors
    public MetricsManager(System.Diagnostics.Metrics.Meter meter)
    {
        _meter = meter;

        _counterDictionary = new Dictionary<string, object>();
        _counterColection = new List<Counter>();

        _histogramDictionary = new Dictionary<string, object>();
        _histogramColection = new List<Histogram>();

        _observableGaugeDictionary = new Dictionary<string, object>();
        _observableGaugeColection = new List<ObservableGauge>();
    }

    // Public Methods
    public void CreateCounter<T>(string name, string? unit = null, string? description = null) where T : struct
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        if(_counterDictionary.ContainsKey(name)) 
            throw new ArgumentException(message: string.Format(COUNTER_ALREADY_EXISTS_ERROR_MESSAGE, name), paramName: nameof(name));

        _counterDictionary.Add(name, _meter.CreateCounter<T>(name, unit, description));
        _counterColection.Add(new Counter(name, unit, description));
    }
    public void IncrementCounter<T>(string name, T delta) where T : struct
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        if (!_counterDictionary.ContainsKey(name))
            throw new ArgumentOutOfRangeException(message: string.Format(COUNTER_NOT_FOUND_ERROR_MESSAGE, name), paramName: nameof(name));

        var counter = (System.Diagnostics.Metrics.Counter<T>) _counterDictionary[name];
        counter.Add(delta);
    }
    public void IncrementCounter<T>(string name, T delta, params KeyValuePair<string, object?>[] tags) where T : struct
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        if (!_counterDictionary.ContainsKey(name))
            throw new ArgumentOutOfRangeException(message: string.Format(COUNTER_NOT_FOUND_ERROR_MESSAGE, name), paramName: nameof(name));

        var counter = (System.Diagnostics.Metrics.Counter<T>)_counterDictionary[name];
        counter.Add(delta, tags);
    }
    public IEnumerable<Counter> GetCounterCollection()
    {
        return _counterColection.AsReadOnly();
    }

    public void CreateHistogram<T>(string name, string? unit = null, string? description = null) where T : struct
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        if (_histogramDictionary.ContainsKey(name))
            throw new ArgumentException(message: string.Format(HISTOGRAM_ALREADY_EXISTS_ERROR_MESSAGE, name), paramName: nameof(name));

        _histogramDictionary.Add(name, _meter.CreateHistogram<T>(name, unit, description));
        _histogramColection.Add(new Histogram(name, unit, description));
    }
    public void RecordHistogram<T>(string name, T value, KeyValuePair<string, object?> tag) where T : struct
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        if (!_counterDictionary.ContainsKey(name))
            throw new ArgumentOutOfRangeException(message: string.Format(HISTOGRAM_NOT_FOUND_ERROR_MESSAGE, name), paramName: nameof(name));

        var histogram = (System.Diagnostics.Metrics.Histogram<T>)_histogramDictionary[name];

        histogram.Record(value, tag);
    }
    public IEnumerable<Histogram> GetHistogramCollection()
    {
        return _histogramColection.AsReadOnly();
    }

    public void CreateObservableGauge<T>(string name, Func<IEnumerable<System.Diagnostics.Metrics.Measurement<T>>> observeValues, string? unit = null, string? description = null) where T : struct
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        if (_observableGaugeDictionary.ContainsKey(name))
            throw new ArgumentException(message: string.Format(OBSERVABLE_GAUGE_ALREADY_EXISTS_ERROR_MESSAGE, name), paramName: nameof(name));

        _observableGaugeDictionary.Add(name, _meter.CreateObservableGauge(name, observeValues, unit, description));
        _observableGaugeColection.Add(new ObservableGauge(name, unit, description));
    }
    public IEnumerable<ObservableGauge> GetObservableGaugeCollection()
    {
        return _observableGaugeColection.AsReadOnly();
    }
}
