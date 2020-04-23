using Microsoft.EntityFrameworkCore;

namespace DockerWebDbService.Models
{
    public class WeatherForecastContext : DbContext
    {
        public DbSet<WeatherForecast> WeatherForecast { get; set; }

        public WeatherForecastContext(DbContextOptions<WeatherForecastContext> options) : base(options) { }

        public WeatherForecastContext()
        {
        }
    }
}
