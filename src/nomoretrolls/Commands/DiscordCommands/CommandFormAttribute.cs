namespace nomoretrolls.Commands.DiscordCommands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class CommandFormAttribute : Attribute
    {
        public CommandFormAttribute(string parameters, string guidelines = null, string example = null, string exampleExplanation = null)
        {
            Parameters = parameters;
            Guidelines = guidelines;
            Example = example;
            ExampleExplanation = exampleExplanation;
        }
        public string Parameters { get; }
        public string Guidelines { get; }
        public string Example { get; }
        public string ExampleExplanation { get; }
    }
}
