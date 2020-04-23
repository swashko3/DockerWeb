using NUnit.Framework;
using DockerWeb.Services;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using DockerWebDbService.Models;
using System.Linq;
using DockerWebTest.Extensions;
using DockerWebDbService.Controllers;
using System;

namespace DockerWebTest
{
    [TestFixture]
    public class DockerWebDbServiceTest
    {
        private IList<WeatherForecast> _weatherForecast { get; set; }
        private WeatherForecastContext _context { get; set; }
        private WeatherForecastController _controller { get; set; }

        [SetUp]
        public void Setup()
        {
            System.Diagnostics.Debugger.Launch();

            _weatherForecast = LoadResource.LoadWeatherForecast(_weatherForecast);

            _context = new WeatherForecastContext();

            _context.WeatherForecast = DbSetExtension.GetQueryableMockDbSet(_weatherForecast);

            _controller = new WeatherForecastController(_context);
        }

        [Test]
        public void Test_GetaAll()
        {
            Assert.IsNotEmpty(_controller.GetAll().ToString());
        }

        [Test]
        public void Test_GetById()
        {
            Guid guid;
            Guid.TryParse("725c4516-96af-4735-bfb3-a2a1579e8059", out guid);

            var wf = _controller.GetById(guid).ToString();

            Assert.IsNotEmpty(wf);

        }

        [Test]
        public void Test_GetByDate()
        {
            DateTime date = DateTime.Parse("2020-04-06T18:22:47.385");

            var wf = _controller.GetByDate(date).ToString();

            Assert.IsNotEmpty(wf);
        }
    }
}