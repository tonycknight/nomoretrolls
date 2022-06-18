using McMaster.Extensions.CommandLineUtils;
using Tk.Extensions;

namespace nomoretrolls.Commands
{
    [Command("version", Description = "Get the bot's version info.")]
    internal class VersionCommand
    {        
        public Task<int> OnExecuteAsync()
        {
            var line = GetDescriptionLines();

            Console.Out.WriteLine(line);

            return Task.FromResult(true.ToReturnCode());
        }


        public string GetDescriptionLines() =>
            ProgramBootstrap.GetProductDescription().Select(s => Crayon.Output.Bright.Cyan(s))
                .Concat(ProgramBootstrap.GetVersionDescription().Select(s => Crayon.Output.Bright.Yellow(s)))
                .Concat(ProgramBootstrap.GetCopyrightDescriptions().Select(s => Crayon.Output.Bright.White(s)))
                .Join(Environment.NewLine);
    }
}
