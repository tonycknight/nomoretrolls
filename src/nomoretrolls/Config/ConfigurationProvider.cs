namespace nomoretrolls.Config
{
    internal class ConfigurationProvider : IConfigurationProvider
    {
        private readonly FileConfigurationProvider _fileProvider;
        private readonly EnvVarConfigurationProvider _envVarProvider;
        private string _filePath;

        public ConfigurationProvider(FileConfigurationProvider fileProvider, EnvVarConfigurationProvider envVarProvider)
        {
            _fileProvider = fileProvider;
            _envVarProvider = envVarProvider;
        }

        public AppConfiguration GetAppConfiguration()
        {
            if (_filePath != null)
            {
                return _fileProvider.SetFilePath(_filePath)
                                    .GetAppConfiguration();
            }
            return _envVarProvider.GetAppConfiguration();
        }

        public IConfigurationProvider SetFilePath(string filePath)
        {
            _filePath = filePath;
            return this;
        }
    }
}
