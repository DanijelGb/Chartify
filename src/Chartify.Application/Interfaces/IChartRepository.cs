using Chartify.Domain.Entities;

namespace Chartify.Application.Interfaces;


public interface IChartRepository
{
    Task<Chart?> GetLatestAsync(string location);
}