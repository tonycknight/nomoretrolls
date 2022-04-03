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
                    .InvalidOpArg(c => string.IsNullOrWhiteSpace(c?.DiscordClientId), "The Discord Client ID is missing.")
                    .InvalidOpArg(c => string.IsNullOrWhiteSpace(c?.DiscordClientToken), "The Discord Client token is missing.");

            config.MongoDb
                    .InvalidOpArg(c => string.IsNullOrWhiteSpace(c?.Connection), "The Mongo connection is missing.")
                    .InvalidOpArg(c => string.IsNullOrWhiteSpace(c?.DatabaseName), "The Mongo DB name is missing.")
                    .InvalidOpArg(c => string.IsNullOrWhiteSpace(c?.WorkflowConfigCollectionName), "The Workflow config collection name is missing.")
                    .InvalidOpArg(c => string.IsNullOrWhiteSpace(c?.UserStatsCollectionName), "The User stats collection name is missing.")
                    .InvalidOpArg(c => string.IsNullOrWhiteSpace(c?.UserBlacklistCollectionName), "The User blacklist collection name is missing.");

            return command;
        }
    }
}
