namespace nomoretrolls.Io
{
    public interface IIoProvider
    {
        public StreamReader OpenFileReader(string filePath);
    }
}
