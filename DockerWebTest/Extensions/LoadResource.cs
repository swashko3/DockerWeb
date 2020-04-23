using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace DockerWebTest.Extensions
{
    public class LoadResource
    {
        public LoadResource()
        {
        }

        public static IList<T> LoadWeatherForecast<T>(IList<T> sourceModel) where T: class
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resource = "DockerWebTest.Resources.WeatherForecast.json";

            Stream stream = assembly.GetManifestResourceStream(resource);
            StreamReader reader = new StreamReader(stream);

            return JsonSerializer.Deserialize<IList<T>>(reader.ReadToEnd());
        }
    }
}
