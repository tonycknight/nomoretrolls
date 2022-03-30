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

            r.Should().EndWith(evt.Message);
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

            var expected = evt.Time.ToString();

            r.Should().Contain(expected);
        }

        [Fact]
        public void Message_MessageSent()
        {
            string? r = null;
            var w = (string s) => { r = s; };

            var t = new ConsoleTelemetry(w);
            var msg = "test";

            t.Message(msg);

            r.Should().EndWith(msg);
        }

        

        [Fact]
        public void Error_MessageSent()
        {
            string? r = null;
            var w = (string s) => { r = s; };

            var t = new ConsoleTelemetry(w);
            var msg = "test";

            t.Error(msg);

            r.Should().EndWith(msg);
        }
    }
}
