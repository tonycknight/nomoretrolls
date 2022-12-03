namespace nomoretrolls.Statistics
{
    internal interface IUserStatisticsProvider
    {
        Task BumpUserStatisticAsync(ulong userId, string statName, TimeSpan expiry);
        Task<long> GetUserStatisticCountAsync(ulong userId, string statName, TimeSpan timeFrame);

    }
}
