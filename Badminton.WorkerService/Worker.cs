using Badminton.Business;

namespace Badminton.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ICourtDetailBusiness _courtDetailBusiness;

        public Worker(ILogger<Worker> logger)
        {
            _courtDetailBusiness ??= new CourtDetailBusiness();
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            #region Refresh Status After Next 3s
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    var result = await _courtDetailBusiness.RefreshCourtDetailStatus();
                    _logger.LogInformation($"Result of Refresh Court Detail Status: {result.Status} - {result.Message}");

                }
                await Task.Delay(3000, stoppingToken);
            }
            #endregion
            #region Refresh Status When time at 00:00:00
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    var now = DateTime.Now;
            //    var nextRun = DateTime.Today.AddDays(1); //00:00:00 is tomorrow
            //    var delay = nextRun - now;

            //    _logger.LogInformation($"Current time: {now}, next run at: {nextRun}, delay: {delay}");

            //    if (delay.TotalMilliseconds > 0)
            //    {
            //        await Task.Delay(delay, stoppingToken);
            //    }
            //    try
            //    {
            //        var result = await _courtDetailBusiness.RefreshCourtDetailStatus();
            //        _logger.LogInformation($"Result of Refresh Court Detail Status: {result.Status} - {result.Message.ToString()}");
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex, "An error occurred while refreshing court detail status");
            //    }
            //    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            //}
            #endregion
        }
    }
}
