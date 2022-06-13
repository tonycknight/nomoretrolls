using System.ComponentModel;
using Discord.Commands;
using Tk.Extensions;

namespace nomoretrolls.Commands.DiscordCommands
{
    internal static class HelpExtensions
    {
        public const char CommandPrefix = '!';

        public static IEnumerable<Type> GetDiscordCommandTypes(this Type type) => type.Assembly.GetTypes()
                            .Where(t => t.IsAssignableTo(typeof(ModuleBase<SocketCommandContext>)));

        public static IEnumerable<(string, string, string[], string)> GetCommandHelp(this IEnumerable<Type> discordCommands)
        {
            var methods = discordCommands.SelectMany(t => t.GetMethods())
                                         .Select(mi => new
                                         {
                                             method = mi,
                                             cmdAttr = mi.GetCustomAttributes(false).OfType<CommandAttribute>().FirstOrDefault(),
                                             descAttr = mi.GetCustomAttributes(false).OfType<DescriptionAttribute>().FirstOrDefault(),
                                             aliasAttr = mi.GetCustomAttributes(false).OfType<AliasAttribute>().FirstOrDefault(),
                                             exampleAttr = mi.GetCustomAttributes(false).OfType<CommandFormAttribute>().FirstOrDefault()
                                         })
                                         .Where(a => !string.IsNullOrWhiteSpace(a.cmdAttr?.Text));

            return methods.Select(a => (a.cmdAttr.Text, a.descAttr?.Description, a.aliasAttr?.Aliases, a.exampleAttr?.Format))
                .OrderBy(t => t.Text);
        }

        public static IEnumerable<string> FormatCommandHelp(this IEnumerable<(string, string, string[], string)> cmds)
        {            
            Func<string[], string> joinAliases = xs => xs.Select(s => $"``{s}``").Join(", ");
            Func<string[], string> aliases = xs => xs != null ? $" Aliases: {joinAliases(xs)}" : "";
                        
            foreach(var cmd in cmds)
            {
                yield return $"``{HelpExtensions.CommandPrefix}{cmd.Item1}``";
                
                if (cmd.Item4 != null)
                {
                    yield return $"{cmd.Item2}{aliases(cmd.Item3)} Form: ``{HelpExtensions.CommandPrefix}{cmd.Item1} {cmd.Item4}``";
                }
                else
                {
                    yield return $"{cmd.Item2}{aliases(cmd.Item3)}";
                }
            }
        }
    }
}
