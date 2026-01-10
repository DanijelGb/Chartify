public interface IChartService
{
    Task<ChartDto> GetGlobalTop100Async();
}