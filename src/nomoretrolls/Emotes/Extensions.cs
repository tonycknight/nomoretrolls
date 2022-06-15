﻿namespace nomoretrolls.Emotes
{
    internal static class Extensions
    {
        public static UserEmoteAnnotationEntry CreateUserEmoteAnnotationEntry(this Discord.IUser user, DateTime now, TimeSpan duration, string emoteList)
        {
            duration = duration > TimeSpan.Zero ? duration : TimeSpan.FromDays(365);

            return new UserEmoteAnnotationEntry()
            {
                UserId = user.Id,
                Start = now,
                Expiry = now.Add(duration),
                EmoteListName = emoteList,
            };
        }
    }
}