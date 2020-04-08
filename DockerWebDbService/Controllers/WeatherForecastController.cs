using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DockerWebDbService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DockerWebDbService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherForecastContext _context;

        public WeatherForecastController(WeatherForecastContext context)
        {
            _context = context;

            if (_context.WeatherForecast.Count() == 0)
            {
                _context.WeatherForecast.Add(new WeatherForecast
                {
                    Date = DateTime.Now,
                    TemperatureC = 15,
                    TemperatureF = 59,
                    Summary = "Initial"
                });

                _context.SaveChanges();
            }
        }

        [HttpGet]
        public ActionResult<string> GetAll()
        {
            return JsonSerializer.Serialize(_context.WeatherForecast.ToList());
        }

        [HttpGet]
        [Route("ById/{id}")]
        public ActionResult<string> GetById(Guid id)
        {
            var item = _context.WeatherForecast.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            return JsonSerializer.Serialize(item);
        }

        [HttpGet]
        [Route("ByDate/{date}")]
        public ActionResult<string> GetByDate(DateTime date)
        {
            var items = _context.WeatherForecast.Where(w => w.Date == date).ToList();

            if (items.Count == 0)
            {
                return NotFound();
            }

            return JsonSerializer.Serialize(items);
        }

        [HttpPost]
        [Route("Add")]
        public ActionResult<string> Add([FromBody] WeatherForecast weatherForecast)
        {

            _context.Add(weatherForecast);
            _context.SaveChanges();

            return JsonSerializer.Serialize(weatherForecast);
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public ActionResult<string> Delete(Guid id)
        {
            var item = _context.WeatherForecast.Find(id);

            if (item == null)
            {
                return NotFound();
            }

            _context.Remove(item);
            _context.SaveChanges();

            return JsonSerializer.Serialize(item);
        }

    }
}
