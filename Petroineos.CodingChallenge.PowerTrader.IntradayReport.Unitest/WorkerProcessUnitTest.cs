using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Petroineos.CodingChallenge.PowerTrader.IntradayReport.Core;
using Petroineos.CodingChallenge.PowerTrader.IntradayReport.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Petroineos.CodingChallenge.PowerTrader.IntradayReport.Unitest
{
    public class WorkerProcessUnitTest
    {
        public  WorkerProcessUnitTest()
        {
            IServiceCollection services = new ServiceCollection();

            var loggerMock = new Mock<ILogger<Worker>>();
            loggerMock.Setup(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            services.AddSingleton(typeof(ILogger), loggerMock);

            services.AddHostedService<Worker>();
            services.AddSingleton<Worker>();
            services.AddTransient<IntraReport>();
            services.AddTransient<ICSVReportService, CSVReportService>();
            //mock the dependencies for injection


            services.AddSingleton(Mock.Of<IOptions<AppInfraReportSettings>>(_ =>
                _.Value == Mock.Of<AppInfraReportSettings>(c =>
                    c.ReportFileFormat == "yyyyMMdd_HHmm"
                )
            ));

            #region "TO Do: Different approach to execute using IHostedService"
            //
            //services.AddSingleton(Mock.Of<Worker>(_ =>
            //    _.StartAsync(It.IsAny<CancellationToken>()) == Task.CompletedTask
            //));

            //var serviceProvider = services.BuildServiceProvider();
            //var hostedService = serviceProvider.GetService<IHostedService>();

            ////Act
            //hostedService.StartAsync(CancellationToken.None);
            //Task.Delay(1000);//Give some time to invoke the methods under test
            //hostedService.StopAsync(CancellationToken.None);

            ////Assert
            //var WorkerProcessService = serviceProvider
            //    .GetRequiredService<Worker>();
            ////extracting mock to do verifications
            //var mock = Mock.Get(WorkerProcessService);
            ////assert expected behavior
            //mock.Verify(_ => _.StartAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            #endregion
        }

        [Fact]
        public void IntraReport_WorkerProcess_Start_Should_Be_Started_Sucessfully()
        {

            //Arrange
            //Mocking logger object 
            var loggerMock = new Mock<ILogger<Worker>>();
            loggerMock.Setup(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
            
            //Mocking the applciation configuration values
            var appInfraReportSettings = new AppInfraReportSettings
            {
                ReportFileFormat = "yyyyMMdd_HHmm",
                ReportFileName = "PowerPosition_",
                ReportLocation = "./Reports/IntraReport/",
                ScheduledRunIntervalInMinutes = "2",
                IsRecordTradeData= true,
                TradeDataLocation="./Reports/TradeData/",
                VolumeFormat="00.00"
            };

            //Mocking the IIntraReport Object
            var intraReportMock = new Mock<IIntraReport>();

            //Act            
            var mockAppinfra = new Mock<IOptions<AppInfraReportSettings>>();            
            mockAppinfra.Setup(x=> x.Value).Returns(appInfraReportSettings);

            var backgroundService = new Worker(mockAppinfra.Object, loggerMock.Object, intraReportMock.Object);
            var res=backgroundService.StartAsync(CancellationToken.None);            

            //Assert
            Assert.True(res.IsCompleted);
            
        }

        [Fact]
        public void IntraReport_WorkerProcess_Start_Should_Be_Stopped_Sucessfully()
        {

            //Arrange
            //Mocking logger object 
            var loggerMock = new Mock<ILogger<Worker>>();
            loggerMock.Setup(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));

            //Mocking the applciation configuration values
            var appInfraReportSettings = new AppInfraReportSettings
            {
                ReportFileFormat = "yyyyMMdd_HHmm",
                ReportFileName = "PowerPosition_",
                ReportLocation = "./Reports/IntraReport/",
                ScheduledRunIntervalInMinutes = "2",
                IsRecordTradeData = true,
                TradeDataLocation = "./Reports/TradeData/"
            };

            //Mocking the IIntraReport Object
            var intraReportMock = new Mock<IIntraReport>();

            //Act            
            var mockAppinfra = new Mock<IOptions<AppInfraReportSettings>>();
            mockAppinfra.Setup(x => x.Value).Returns(appInfraReportSettings);

            var backgroundService = new Worker(mockAppinfra.Object, loggerMock.Object, intraReportMock.Object);
            var res = backgroundService.StartAsync(CancellationToken.None);
            Task.Delay(1000);
            backgroundService.StopAsync(CancellationToken.None);

            //Assert
            Assert.True(res.IsCompleted);

        }
        
    }
}