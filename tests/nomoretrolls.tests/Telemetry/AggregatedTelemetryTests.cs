using System.Linq;
using nomoretrolls.Config;
using nomoretrolls.Telemetry;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Telemetry
{
    public class AggregatedTelemetryTests
    {
        [Fact]
        public void Event_EventMessagePropagated()
        {            
            var cp = CreateMockConfigurationProvider();
            var telemetries = Enumerable.Range(1, 3)
                .Select(i => Substitute.For<ITelemetry>())
                .ToArray();

            var aggTelemetry = new AggregatedTelemetry(telemetries, cp);

            var evt = new TelemetryEvent();

            aggTelemetry.Event(evt);

            foreach(var t in telemetries)
            {
                t.Received(1).Event(evt);
            }
        }

        [Fact]
        public void Message_EventMessagePropagated()
        {
            var cp = CreateMockConfigurationProvider();
            var telemetries = Enumerable.Range(1, 3)
                .Select(i => Substitute.For<ITelemetry>())
                .ToArray();

            var aggTelemetry = new AggregatedTelemetry(telemetries, cp);

            var evt = new TelemetryEvent() { Message = "test" };

            aggTelemetry.Event(evt);

            foreach (var t in telemetries)
            {
                t.Received(1).Event(Arg.Is<TelemetryEvent>(e => e.Message == evt.Message));
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(true, nameof(TelemetryDependencyEvent))]
        [InlineData(true, nameof(TelemetryInfoEvent), nameof(TelemetryDependencyEvent))]
        [InlineData(false, nameof(TelemetryInfoEvent) )]
        [InlineData(false, nameof(TelemetryInfoEvent), nameof(TelemetryWarningEvent))]

        public void Message_EventMessageFiltered_Propagated(bool received, params string[] eventTypes)
        {
            var telemetry = new TelemetryConfiguration()
            {
                LogTelemetryTypes = eventTypes,
            };
            var cp = CreateMockConfigurationProvider(telemetry: telemetry);
            var telemetries = new[] { Substitute.For<ITelemetry>() };
            var aggTelemetry = new AggregatedTelemetry(telemetries, cp);

            var evt = new TelemetryDependencyEvent() { Message = "test" };

            aggTelemetry.Event(evt);

            foreach (var t in telemetries)
            {
                if (received)
                {
                    t.Received(1).Event(evt);
                }
                else
                {
                    t.Received(0).Event(evt);
                }
            }
            
        }

        [Fact]
        public void Error_EventMessagePropagated()
        {
            var cp = CreateMockConfigurationProvider();
            var telemetries = Enumerable.Range(1, 3)
                .Select(i => Substitute.For<ITelemetry>())
                .ToArray();

            var aggTelemetry = new AggregatedTelemetry(telemetries, cp);

            var evt = new TelemetryEvent() { Message = "test" };

            aggTelemetry.Event(evt);

            foreach (var t in telemetries)
            {
                t.Received(1).Event(Arg.Is<TelemetryEvent>(e => e.Message == evt.Message));
            }
        }

        private IConfigurationProvider CreateMockConfigurationProvider(AppConfiguration appConfig = null, TelemetryConfiguration telemetry = null)
        {
            var config = appConfig ??new AppConfiguration();
            config.Telemetry = telemetry ?? new TelemetryConfiguration();
            var cp = Substitute.For<IConfigurationProvider>();
            cp.GetAppConfiguration().Returns(config);
            return cp;
        }
    }
}
