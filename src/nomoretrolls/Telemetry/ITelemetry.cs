﻿namespace nomoretrolls.Telemetry
{
    internal interface ITelemetry
    {
        void Event(TelemetryEvent evt);
    }
}
