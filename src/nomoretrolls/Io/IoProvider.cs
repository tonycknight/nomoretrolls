using System.Diagnostics.CodeAnalysis;

namespace nomoretrolls.Io
{
    [ExcludeFromCodeCoverage]
    public class IoProvider : IIoProvider
    {
        public StreamReader OpenFileReader(string filePath) => File.OpenText(filePath);
    }
}
