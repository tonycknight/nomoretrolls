using Discord;
using Discord.Commands;
using nomoretrolls.Parsing;

namespace nomoretrolls.Commands
{
    internal static class CommandContextExtensions
    {
        public static Task<IUser?> GetUserAsync(this ICommandContext context, string userName) 
        {
            var (un, discrim) = userName.DeconstructDiscordName();

            return context.GetUserAsync(un, discrim);
        }

        public static async Task<IUser?> GetUserAsync(this ICommandContext context, string userName, string discriminator)
        {
            if (userName != null && discriminator != null) 
            {
                return await context.Client.GetUserAsync(userName, discriminator);
            }
            return null;
        }

        public static Task<IUser?> GetUserAsync(this ICommandContext context, ulong userId) => context.Client.GetUserAsync(userId);
    }
}
