namespace nomoretrolls.Telemetry
{
    internal record TelemetryEvent
    {
        public DateTime Time { get; init; } = DateTime.UtcNow;
        public string Message { get; init; } = default!;
    }

    internal record TelemetryHeadlineEvent : TelemetryEvent
    {

    }

    internal record TelemetryInfoEvent : TelemetryEvent
    {

    }

    internal record TelemetryTraceEvent : TelemetryEvent
    {

    }

    internal record TelemetryWarningEvent : TelemetryEvent
    {

    }

    internal record TelemetryErrorEvent : TelemetryEvent
    {
        public Exception Exception { get; init; } = default!;
    }
}
