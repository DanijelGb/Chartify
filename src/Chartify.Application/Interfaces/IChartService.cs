using Chartify.Domain.Entities;

namespace Chartify.Application.Interfaces;

public interface IChartService
{
    Task<Chart> GetGlobalTop100Async();

    //Task<Chart> GetTop100ByCountryAsync(string country);
}