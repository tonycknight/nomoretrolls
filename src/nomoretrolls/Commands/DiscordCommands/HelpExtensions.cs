using System.ComponentModel;
using System.Reflection;
using Discord.Commands;
using Tk.Extensions;
using Tk.Extensions.Linq;

namespace nomoretrolls.Commands.DiscordCommands
{
    internal class CommandHelpInfo
    {
        public MethodInfo Method { get; init; }
        public CommandAttribute? Command { get; init; }
        public DescriptionAttribute? Description { get; init; }
        public AliasAttribute? Alias { get; init; }
        public CommandFormAttribute? Form { get; init; }
    }

    internal static class HelpExtensions
    {
        public const char CommandPrefix = '!';

        public static IEnumerable<Type> GetDiscordCommandTypes(this Type type) => type.Assembly.GetTypes()
                            .Where(t => t.IsAssignableTo(typeof(ModuleBase<SocketCommandContext>)));

        public static IEnumerable<CommandHelpInfo> GetCommandHelpInfos(this IEnumerable<Type> discordCommands)
        {
            return discordCommands.SelectMany(t => t.GetMethods())
                                         .Select(mi => new CommandHelpInfo()
                                         {
                                             Method = mi,
                                             Command = mi.GetCustomAttributes(false).OfType<CommandAttribute>().FirstOrDefault(),
                                             Description = mi.GetCustomAttributes(false).OfType<DescriptionAttribute>().FirstOrDefault(),
                                             Alias = mi.GetCustomAttributes(false).OfType<AliasAttribute>().FirstOrDefault(),
                                             Form = mi.GetCustomAttributes(false).OfType<CommandFormAttribute>().FirstOrDefault()
                                         })
                                         .Where(i => !string.IsNullOrWhiteSpace(i.Command?.Text));
        }

        public static IEnumerable<string> FormatCommandHelp(this IEnumerable<CommandHelpInfo> helpInfos)
        {
            Func<IEnumerable<string>, string> joinAliases = xs => xs.Select(s => $"``{HelpExtensions.CommandPrefix}{s}``").Join(", ");

            foreach (var helpInfo in helpInfos.OrderBy(i => i.Command.Text))
            {
                var cmd = $"{HelpExtensions.CommandPrefix}{helpInfo.Command.Text}";
                var aliases = (helpInfo.Alias?.Aliases).NullToEmpty().Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                yield return helpInfo.Description.Description != null
                    ? $"``{cmd}``: {helpInfo.Description.Description}"
                    : $"``{cmd}``";


                if (helpInfo.Form != null)
                {
                    yield return $"Form: ``{cmd} {helpInfo.Form.Parameters}``";


                    if (helpInfo.Form.Example != null)
                    {
                        yield return $"E.g.: ``{cmd} {helpInfo.Form.Example}`` {helpInfo.Form.ExampleExplanation}";
                    }

                    if (helpInfo.Form.Guidelines != null)
                    {
                        yield return helpInfo.Form.Guidelines;
                    }
                }

                yield return "";
            }
        }
    }
}
