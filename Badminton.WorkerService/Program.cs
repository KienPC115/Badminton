using Badminton.Business;
using Badminton.Data.Models;
using Badminton.WorkerService;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddDbContext<Net1710_221_8_BadmintonContext>(options =>
            options.UseSqlServer(
                hostContext.Configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5, // Maximum number of retry attempts (default is 6)
                        maxRetryDelay: TimeSpan.FromSeconds(30), // Maximum delay between retries
                        errorNumbersToAdd: null // List of additional error numbers to retry on
                    );
                }));
    })
    .UseWindowsService();

var host = builder.Build();
await host.RunAsync();