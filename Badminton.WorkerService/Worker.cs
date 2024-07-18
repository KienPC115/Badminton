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
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                        var result = await _courtDetailBusiness.RefreshCourtDetailStatus();
                        var text = $"{DateTime.Now} Result of Refresh Court Detail Status: {result.Status} - {result.Message}";
                        _logger.LogInformation(text);
                        await File.AppendAllTextAsync(FilePath, text + "\n\n");
                    }
                    await Task.Delay(3000, stoppingToken);
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
