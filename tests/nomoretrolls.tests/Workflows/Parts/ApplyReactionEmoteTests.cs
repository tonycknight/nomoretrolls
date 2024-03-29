﻿using System.Threading.Tasks;
using FsCheck;
using FsCheck.Xunit;
using nomoretrolls.Emotes;
using nomoretrolls.Messaging;
using nomoretrolls.Workflows;
using nomoretrolls.Workflows.Parts;
using NSubstitute;

namespace nomoretrolls.tests.Workflows.Parts
{
    public class ApplyReactionEmoteTests
    {

        [Property(Verbose = true)]
        public async Task<bool> ExecuteAsync_EmoteApplied(NonEmptyString value)
        {
            var gen = Substitute.For<IEmoteGenerator>();
            gen.PickEmoteAsync(Arg.Any<string>()).Returns(value.Get);

            var p = new ApplyReactionEmote(gen, null);

            var msg = Substitute.For<IDiscordMessageContext>();
            var context = new MessageWorkflowContext(msg);

            var r = await p.ExecuteAsync(context);

            return r.EmoteCode() == value.Get;
        }
    }
}
