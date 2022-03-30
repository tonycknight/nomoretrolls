using System.Diagnostics.CodeAnalysis;
using nomoretrolls.Io;

namespace nomoretrolls.Commands
{
    [ExcludeFromCodeCoverage]
    internal class StartServerCommandValidator
    {
        public StartServerCommand Validate(StartServerCommand command)
        {
            command.ConfigurationFile = command.ConfigurationFile?
                .InvalidOpArg(string.IsNullOrWhiteSpace, $"The {nameof(command.ConfigurationFile)} parameter is missing.")
                .ResolveWorkingPath()
                .AssertFileExists();

            return command;
        }
    }
}
