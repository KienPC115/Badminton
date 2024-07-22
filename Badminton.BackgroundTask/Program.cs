using Badminton.BackgroundTask;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
    })
    .UseWindowsService();

var host = builder.Build();
host.Run();