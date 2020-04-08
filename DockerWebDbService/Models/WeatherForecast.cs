using System;

namespace DockerWebDbService.Models
{
    public class WeatherForecast
    {
        public Guid ID { get; set; }

        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF { get; set; }

        public string Summary { get; set; }
    }
}
