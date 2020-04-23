using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DockerWeb.Interfaces;
using DockerWeb.Models;

namespace DockerWeb.Services
{
    public class DBService: IDbService
    {
        public HttpClient Client { get; }

        public DBService(HttpClient client)
        {
            //TODO: this needs to be configured for docker ip once image is built
            //client.BaseAddress = new Uri("http://localhost:5002/");
            //client.BaseAddress = new Uri("http://172.17.0.5:5002/");

            //attempt to get the asyncreference from the docker-compose, otherwise default to localhost
            var host = Environment.GetEnvironmentVariable("ServiceHost");

            if (host == null)
            {
                host = "localhost";
            }

            client.BaseAddress = new Uri($"http://{host}:5002/");

            Client = client;

        }

        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var response = await Client.GetAsync("api/WeatherForecast");

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(responseStream);
        }

        public async Task<WeatherForecast> GetById(Guid id)
        {

            var response = await Client.GetAsync($"api/WeatherForecast/ById/{id}");

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<WeatherForecast>(responseStream);
        }

        public async Task<IEnumerable<WeatherForecast>> GetByDate(DateTime date)
        {
            var formattedDate = date.ToString("MM-dd-yyyy HH:mm:ss");

            var response = await Client.GetAsync($"api/WeatherForecast/ByDate/{formattedDate}");

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(responseStream);
        }

        public async Task<WeatherForecast> Add(WeatherForecast weatherForecast)
        {
            var requestUri = "api/WeatherForecast/Add";

            var json = JsonSerializer.Serialize(weatherForecast);

            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync(requestUri, content);

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<WeatherForecast>(responseStream);
        }

        public async Task<HttpStatusCode> Delete(Guid id)
        {
            HttpResponseMessage response = await Client.DeleteAsync($"api/WeatherForecast/Delete/{id}");

            return response.StatusCode;
        }
    }
}
