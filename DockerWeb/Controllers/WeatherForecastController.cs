using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DockerWeb.Models;
using System.Net.Http;
using DockerWeb.Services;
using System.Net;

namespace DockerWeb.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly DBService _dbService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, DBService dbService)
        {
            _logger = logger;
            _dbService = dbService;
        }


        /// <summary>
        /// Return all Weather Forcast
        /// </summary>
        /// <returns>
        /// array of WeatherForecast objects
        /// </returns>
        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            try
            {
                return await _dbService.Get();
            }
            catch (HttpRequestException)
            {
                return Array.Empty<WeatherForecast>();
            }
        }

        [HttpGet]
        [Route("ById/{id}")]
        public async Task<WeatherForecast> GetById(Guid id)
        {
            try
            {
                return await _dbService.GetById(id);
            }
            catch (HttpRequestException)
            {
                return new WeatherForecast();
            }
        }

        [HttpGet]
        [Route("GetByDate/{date}")]
        public async Task<IEnumerable<WeatherForecast>> GetByDate(DateTime date)
        {
            try
            {
                return await _dbService.GetByDate(date);
            }
            catch (HttpRequestException)
            {
                return Array.Empty<WeatherForecast>();
            }
        }

        [HttpPost]
        [Route("Add/{date}/{temperatureC}/{summary}")]
        public async Task<WeatherForecast> Append(DateTime date, int temperatureC, String summary)
        {
            try
            {
                var weatherForecast = new WeatherForecast
                {
                    Date = date,
                    TemperatureC = temperatureC,
                    Summary = summary
                };

                return await _dbService.Add(weatherForecast);
            }
            catch (HttpRequestException)
            {
                return new WeatherForecast();
            }
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<HttpStatusCode> Delete(Guid id)
        {
            return await _dbService.Delete(id);
        }
    }
}
