using Badminton.BackgroundTask.Models;

namespace Badminton.BackgroundTask
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private Net17102218BadmintonContext _context;
        private readonly string LogPath = "D:\\now_semester\\PRN221\\Net1710_221_8_Goodminton\\Source\\Badminton\\Badminton.BackgroundTask\\Logging.txt";
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var text = string.Empty;
            try
            {
                #region Refresh now
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        text += ("Worker running at: {time}\n", DateTimeOffset.Now).ToString();
                        _logger.LogInformation("Worker running at: {time}\n", DateTimeOffset.Now);
                        var result = RefreshStatusOfCourtDetail();
                        if (result <= 0)
                        {
                            text += "Refresh failed\n";
                        }
                        else
                        {
                            text += "Refresh successfully\n";
                        }
                    }
                    await Task.Delay(1000, stoppingToken);
                }
                #endregion

                #region Refresh Status When time at 00:00:00
                //while (!stoppingToken.IsCancellationRequested)
                //{
                //    var now = DateTime.Now;
                //    var nextRun = DateTime.Today.AddDays(1); //00:00:00 is tomorrow
                //    var delay = nextRun - now;

                //    text = $"Current time: {now}, next run at: {nextRun}, delay: {delay}";
                //    File.AppendAllText(LogPath, text);

                //    _logger.LogInformation(text);

                //    if (delay.TotalMilliseconds > 0)
                //    {
                //        await Task.Delay(delay, stoppingToken);
                //    }
                //    var result = RefreshStatusOfCourtDetail();
                //    if (result <= 0)
                //    {
                //        text = "Refresh failed\n";
                //    }
                //    else
                //    {
                //        text = "Refresh successfully\n";
                //    }
                //    File.AppendAllText(LogPath, text);
                //    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                //}
                #endregion
            }
            catch (Exception ex)
            {
                text = ex.ToString() + "\n";
                File.AppendAllText(LogPath, text);
            }
        }

        private int RefreshStatusOfCourtDetail()
        {
            _context = new Net17102218BadmintonContext();
            var list = _context.CourtDetails.ToList().Where(c => c.Status.Equals("Booked")).ToList();
            list.ForEach(x =>
            {
                x.Status = "Available";
                _context.CourtDetails.Update(x);
            });
            return _context.SaveChanges();
        }
    }
}
