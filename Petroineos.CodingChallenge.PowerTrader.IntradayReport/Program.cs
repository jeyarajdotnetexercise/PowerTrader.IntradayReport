using Petroineos.CodingChallenge.PowerTrader.IntradayReport;
using Petroineos.CodingChallenge.PowerTrader.IntradayReport.Core;
using Petroineos.CodingChallenge.PowerTrader.IntradayReport.Services;

//Read application configuration values from appsettings
IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true)
   .Build();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        //DI dependant objects configured in DI container
        services.AddHostedService<Worker>();
        services.AddSingleton<Worker>();
        services.AddTransient<IIntraReport,IntraReport>();
        services.AddTransient<ICSVReportService, CSVReportService>();

        //Read Appsettings application configuration values to read across the application 
        var IntraReportConfig = configuration.GetSection("IntraReportConfig");
        services.Configure<AppInfraReportSettings>(IntraReportConfig);

    })
    .Build();
    //TODO: To deploy the windows service in server, we need to make slight changes in the above configuration
await host.RunAsync();



