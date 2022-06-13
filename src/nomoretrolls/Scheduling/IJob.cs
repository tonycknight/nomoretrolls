namespace nomoretrolls.Scheduling
{
    internal interface IJob
    {
        TimeSpan Frequency { get; }
        Task ExecuteAsync();
    }
}
