using Newtonsoft.Json;
using nomoretrolls.Io;
using Tk.Extensions.Guards;

namespace nomoretrolls.Config
{
    internal class FileConfigurationProvider : IConfigurationProvider
    {
        private readonly IIoProvider _ioProvider;
        private string? _filePath;

        public FileConfigurationProvider(Io.IIoProvider ioProvider)
        {
            _ioProvider = ioProvider;
        }

        public AppConfiguration GetAppConfiguration()
        {
            if (_filePath == null)
            {
                throw new InvalidOperationException("File path not set.");
            }

            return GetAppConfiguration(_filePath);
        }

        public IConfigurationProvider SetFilePath(string filePath)
        {
            _filePath = filePath.ArgNotNull(nameof(filePath));
            return this;
        }

        private AppConfiguration GetAppConfiguration(string filePath)
        {
            try
            {
                using var sRdr = _ioProvider.OpenFileReader(filePath);
                using var jRdr = new JsonTextReader(sRdr);

                var s = JsonSerializer.Create();

                return s.Deserialize<AppConfiguration>(jRdr);
            }
            catch (JsonSerializationException)
            {
                throw new InvalidOperationException("The config file is not valid JSON.");
            }
            catch (JsonReaderException)
            {
                throw new InvalidOperationException("The config file is not valid JSON.");
            }
        }

    }
}
