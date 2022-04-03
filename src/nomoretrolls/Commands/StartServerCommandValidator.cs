using System.Diagnostics.CodeAnalysis;
using nomoretrolls.Io;

namespace nomoretrolls.Commands
{
    [ExcludeFromCodeCoverage]
    internal class StartServerCommandValidator
    {
        public StartServerCommand Validate(StartServerCommand command)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            command.ConfigurationFile = command.ConfigurationFile?
                .InvalidOpArg(string.IsNullOrWhiteSpace, $"The {nameof(command.ConfigurationFile)} parameter is missing.")
                .ResolveWorkingPath()
                .AssertFileExists();
#pragma warning restore CS8601 // Possible null reference assignment.

            return command;
        }

        public StartServerCommand Validate(StartServerCommand command, Config.AppConfiguration config)
        {
            config.Discord
                    .InvalidOpArg(c => c == null, "The Discord configuration is missing.")
                    .InvalidOpArg(c => string.IsNullOrWhiteSpace(c.DiscordClientId), "The Discord Client ID is missing.")
                    .InvalidOpArg(c => string.IsNullOrWhiteSpace(c.DiscordClientToken), "The Discord Client token is missing.");

            return command;
        }
    }
}
