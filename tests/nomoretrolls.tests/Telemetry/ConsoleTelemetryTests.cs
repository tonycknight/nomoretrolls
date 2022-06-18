﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using nomoretrolls.Telemetry;
using Xunit;

namespace nomoretrolls.tests.Telemetry
{
    public class ConsoleTelemetryTests
    {
        [Fact]
        public void Event_MessageSent()
        {
            string? r = null;
            var w = (string s) => { r = s; };

            var t = new ConsoleTelemetry(w);
            var evt = new TelemetryEvent()
            {
                Message = "test"
            };

            t.Event(evt);

            r.Should().Contain(evt.Message);
        }

        [Fact]
        public void Event_MessageSent_WithTimestamp()
        {
            string? r = null;
            var w = (string s) => { r = s; };

            var t = new ConsoleTelemetry(w);
            var evt = new TelemetryEvent()
            {
                Message = "test",
                Time = DateTime.UtcNow,
            };

            t.Event(evt);

            var expected = evt.Time.ToString("yyyy-MM-dd HH:mm:ss.fff");

            r.Should().Contain(expected);
        }

        [Fact]
        public void Message_MessageSent()
        {
            string? r = null;
            var w = (string s) => { r = s; };

            var t = new ConsoleTelemetry(w);
            var msg = "test";

            t.Event(new TelemetryEvent() { Message = msg } );

            r.Should().Contain(msg);
        }

        

        [Fact]
        public void Error_MessageSent()
        {
            string? r = null;
            var w = (string s) => { r = s; };

            var t = new ConsoleTelemetry(w);
            var msg = "test";

            t.Event(new TelemetryErrorEvent() { Message = msg } );

            r.Should().Contain(msg);
        }
    }
}
