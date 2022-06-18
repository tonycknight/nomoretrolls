using Tk.Extensions.Guards;

namespace nomoretrolls.Emotes
{
    internal static class Extensions
    {
        public static UserEmoteAnnotationEntry CreateUserEmoteAnnotationEntry(this Discord.IUser user, DateTime now, TimeSpan duration, string emoteList)
        {
            user.ArgNotNull(nameof(user));
            duration = duration > TimeSpan.Zero ? duration : TimeSpan.FromDays(365);

            return new UserEmoteAnnotationEntry()
            {
                UserId = user.Id,
                Start = now,
                Expiry = now.Add(duration),
                EmoteListName = emoteList,
            };
        }

        public static UserEmoteAnnotationEntryDto ToDto(this UserEmoteAnnotationEntry value) 
            => new UserEmoteAnnotationEntryDto()
            {
                UserId = value.UserId,
                Start = value.Start,
                Expiry = value.Expiry,
                EmoteListName = value.EmoteListName,
            };

        public static UserEmoteAnnotationEntry FromDto(this UserEmoteAnnotationEntryDto value) 
            => new UserEmoteAnnotationEntry()
            {
                UserId = value.UserId,
                Start = value.Start,
                Expiry = value.Expiry,
                EmoteListName = value.EmoteListName,
            };

        public static IList<EmoteInfo> ToEmotes(this IEnumerable<string> emotes)
            => emotes.Select(s => new[] { s }).ToEmotes();

        private static IList<EmoteInfo> ToEmotes(this IEnumerable<string[]> emotes)
            => emotes.Select(e => new EmoteInfo(e)).ToArray();
    }
}
