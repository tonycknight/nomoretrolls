namespace nomoretrolls.Commands.DiscordCommands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class CommandFormAttribute : Attribute
    {
        public CommandFormAttribute(string text, string guidelines = null)
        {
            Format = text;
            Guidelines = guidelines;
        }
        public string Format { get; }
        public string Guidelines { get; }
    }
}
