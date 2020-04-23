using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DockerWeb.Controllers;
using DockerWeb.Interfaces;
using DockerWeb.Models;
using DockerWebTest.Extensions;
using Moq;
using NUnit.Framework;

namespace DockerWebTest
{
    [TestFixture]
    public class DockerWebTest
    {
        private IList<WeatherForecast> _weatherForecast { get; set; }
        private WeatherForecastController _controller { get; set; }

        private IDbService _MockDbService;

        [SetUp]
        public void Setup()
        {
            System.Diagnostics.Debugger.Launch();

            _weatherForecast = LoadResource.LoadWeatherForecast(_weatherForecast);

            _MockDbService = InstantiateMockDbService();

            _controller = new WeatherForecastController(null, _MockDbService);
        }

        private IDbService InstantiateMockDbService()
        {
            Mock<IDbService> mockDbService = new Mock<IDbService>();

            // We have to mock out all the methods
            mockDbService.Setup(m => m.Get()).Returns(() => Task.FromResult(_weatherForecast as IEnumerable<WeatherForecast>));
            mockDbService.Setup(m => m.GetById(It.IsAny<Guid>())).Returns((Guid id) => Task.FromResult(_weatherForecast.Where(x => x.ID == id).FirstOrDefault()));
            mockDbService.Setup(m => m.GetByDate(It.IsAny<DateTime>())).Returns((DateTime dt) => Task.FromResult(_weatherForecast.Where(x => x.Date == dt).ToList() as IEnumerable<WeatherForecast>));
            mockDbService.Setup(m => m.Add(It.IsAny<WeatherForecast>())).Returns((WeatherForecast wf) => Task.FromResult(Mock_Add(wf)));
            mockDbService.Setup(m => m.Delete(It.IsAny<Guid>())).Returns((Guid id) => Task.FromResult(Mock_Delete(id)));

            return mockDbService.Object;
        }

        private WeatherForecast Mock_Add(WeatherForecast wf)
        {
            //Check to see if the model already exist, if so, just return it
            if (wf.ID == Guid.Empty)
            {
                Guid id = Guid.NewGuid();
                wf.ID = id;

                _weatherForecast.Add(wf);
            }

            return _weatherForecast.Where(x => x.ID == wf.ID).FirstOrDefault();
        }

        private HttpStatusCode Mock_Delete(Guid id)
        {
            // Check to see if the GUID exist
            var wf = _MockDbService.GetById(id);

            if (wf.Result == null)
            {
                return HttpStatusCode.NotFound;
            }
            else
            {
                return HttpStatusCode.OK;
            }
        }

        [Test]
        public async Task Test_GetaAllAsync()
        {

            IEnumerable<WeatherForecast> wf = await _controller.Get();

            Assert.IsNotNull(wf);
            Assert.AreEqual(5, wf.Count());
        }

        [Test]
        public async Task Test_GetById()
        {
            // Test a good GUID
            Guid guid;
            Guid.TryParse("725c4516-96af-4735-bfb3-a2a1579e8059", out guid);

            WeatherForecast wf = await _controller.GetById(guid);

            Assert.IsNotNull(wf);

            // Test for a Bad Guid
            Guid.TryParse("725c4516-96af-4735-bfb3-a2a1579f9160", out guid);

            wf = await _controller.GetById(guid);

            Assert.IsNull(wf);
        }

        [Test]
        public async Task Test_GetByDate()
        {
            DateTime d = DateTime.Parse("2020-04-07T18:22:28.118605");

            IEnumerable<WeatherForecast> wf = await _controller.GetByDate(d);

            Assert.IsNotNull(wf);
            Assert.GreaterOrEqual(1, wf.Count());
        }

        [Test]
        public async Task Test_Delete()
        {
            Guid guid;
            Guid.TryParse("725c4516-96af-4735-bfb3-a2a1579e8059", out guid);

            HttpStatusCode code = await _controller.Delete(guid);

            Assert.IsNotNull(code);
            Assert.AreEqual(HttpStatusCode.OK, code);

            // Test for a Bad Guid
            Guid.TryParse("725c4516-96af-4735-bfb3-a2a1579f9160", out guid);
            code = await _controller.Delete(guid);

            Assert.IsNotNull(code);
            Assert.AreEqual(HttpStatusCode.NotFound, code);
        }

        [Test]
        public async Task Test_Append()
        {
            var date = DateTime.Now;

            WeatherForecast wf = await _controller.Append(date, 17, "Test Test Test");

            Assert.IsNotNull(wf);
            Assert.AreEqual(date, wf.Date);
        }
    }
}
