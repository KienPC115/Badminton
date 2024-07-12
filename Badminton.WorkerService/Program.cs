using Badminton.Business;
using Badminton.WorkerService;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
    })
    .UseWindowsService();

var host = builder.Build();
await host.RunAsync();