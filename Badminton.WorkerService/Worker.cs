using Badminton.Business;
using System.Runtime.Loader;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Extensions.Logging;

namespace Badminton.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ICourtDetailBusiness _courtDetailBusiness;
        private readonly string FilePath = "D:\\now_semester\\PRN221\\Net1710_221_8_Goodminton\\Source\\Badminton\\Badminton.WorkerService\\Log.txt";

        public Worker(ILogger<Worker> logger)
        {
            _courtDetailBusiness ??= new CourtDetailBusiness();
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                #region Refresh Status After Next 3s
                //while (!stoppingToken.IsCancellationRequested)
                //{
                //    if (_logger.IsEnabled(LogLevel.Information))
                //    {
                //        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //        var result = await _courtDetailBusiness.RefreshCourtDetailStatus();
                //        var text = $"{DateTime.Now} Result of Refresh Court Detail Status: {result.Status} - {result.Message}";
                //        _logger.LogInformation(text);
                //        await File.AppendAllTextAsync(FilePath, text + "\n\n");
                //    }
                //    await Task.Delay(3000, stoppingToken);
                //}
                #endregion
                #region Refresh Status When time at 00:00:00
                while (!stoppingToken.IsCancellationRequested)
                {
                    var now = DateTime.Now;
                    var nextRun = DateTime.Today.AddDays(1); //00:00:00 is tomorrow
                    var delay = nextRun - now;

                    _logger.LogInformation($"Current time: {now}, next run at: {nextRun}, delay: {delay}");

                    if (delay.TotalMilliseconds > 0)
                    {
                        await Task.Delay(delay, stoppingToken);
                    }
                    try
                    {
                        var result = await _courtDetailBusiness.RefreshCourtDetailStatus();
                        _logger.LogInformation($"Result of Refresh Court Detail Status: {result.Status} - {result.Message.ToString()}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while refreshing court detail status");
                    }
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
                #endregion
            }
            catch (Exception e)
            {
                File.AppendAllText(FilePath, e.Message);
            }
        }
    }
}
