namespace nomoretrolls.Commands.DiscordCommands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class CommandFormAttribute : Attribute
    {
        public CommandFormAttribute(string text)
        {
            Format = text;
        }
        public string Format { get; }
    }
}
