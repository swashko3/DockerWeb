using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DockerWeb.Models;

namespace DockerWeb.Interfaces
{
    public interface IDbService
    {
        Task<IEnumerable<WeatherForecast>> Get();
        Task<WeatherForecast> GetById(Guid id);
        Task<IEnumerable<WeatherForecast>> GetByDate(DateTime date);
        Task<WeatherForecast> Add(WeatherForecast weatherForecast);
        Task<HttpStatusCode> Delete(Guid id);
    }
}
