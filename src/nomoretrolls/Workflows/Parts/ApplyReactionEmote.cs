﻿using nomoretrolls.Emotes;

namespace nomoretrolls.Workflows.Parts
{
    internal class ApplyReactionEmote : IMessageWorkflowPart
    {
        private readonly IEmoteGenerator _emoteGenerator;
        private readonly string _emotesName;

        public ApplyReactionEmote(IEmoteGenerator emoteGenerator, string emotesName)
        {
            _emoteGenerator = emoteGenerator;
            _emotesName = emotesName ?? "blacklist";
        }

        public Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var result = context.DeepClone()
                        .EmoteCode(_emoteGenerator.PickEmoteAsync(_emotesName).GetAwaiter().GetResult());

            return Task.FromResult(result);
        }
    }
}
