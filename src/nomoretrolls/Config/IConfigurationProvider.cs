namespace nomoretrolls.Config
{
    public interface IConfigurationProvider
    {
        public IConfigurationProvider SetFilePath(string filePath);

        public AppConfiguration GetAppConfiguration();
    }
}
