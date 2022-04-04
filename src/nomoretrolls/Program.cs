using System.Diagnostics.CodeAnalysis;
using McMaster.Extensions.CommandLineUtils;

namespace nomoretrolls
{
    [ExcludeFromCodeCoverage]
    [Subcommand(typeof(Commands.StartServerCommand))]
    [Subcommand(typeof(Commands.VersionCommand))]
    public class Program
    {
        public static int Main(string[] args)
        {
            using var app = new CommandLineApplication<Program>();

            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(ProgramBootstrap.CreateServiceCollection());

            try
            {
                return app.Execute(args);                
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return false.ToReturnCode();
            }
        }

        private int OnExecute(CommandLineApplication app)
        {
            app.Description = "nomoretrolls";
            app.ShowHelp();
            return true.ToReturnCode();
        }

    }
}