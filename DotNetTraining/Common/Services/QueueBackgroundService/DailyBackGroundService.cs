//using BPMaster.Utilities;
//using Services;

//public class DailyTaskService : BackgroundService
//{
//    private readonly IServiceProvider _serviceProvider;
//    private readonly string _logFilePath = "Logs/daily_task_log.txt";
//    private readonly TimeSpan _targetTime = new TimeSpan(10, 40, 0); // 9:00 AM

//    public DailyTaskService(IServiceProvider serviceProvider)
//    {
//        _serviceProvider = serviceProvider;
//        Directory.CreateDirectory("Logs");
//        // Define the target timezone (GMT+7)
//    }
//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        while (!stoppingToken.IsCancellationRequested)
//        {
//            // Calculate the next run time in GMT+7
//            var now = DateTimeConvertUtil.GetCurrentTimeInUtc7ForMac();

//            var nextRun = DateTime.Today.Add(_targetTime);
//            if (now.TimeOfDay >= _targetTime) //|| IsWeekend(now)
//            {
//                nextRun = nextRun.AddDays(1); // Skip to the next day
//            }

//            // Adjust to skip weekends
//            //while (IsWeekend(nextRun))
//            //{
//            //    nextRun = nextRun.AddDays(1);
//            //}

//            // Calculate the delay in UTC
//            var nextRunInUtc = DateTimeConvertUtil.ConvertTimeInUtc7ForMac(nextRun);
//            var delay = nextRunInUtc - DateTimeConvertUtil.GetCurrentTimeInUtc7ForMac();

//            Log($"Next run scheduled for: {nextRun} (UTC: {nextRunInUtc})");
//            Log($"Calculated delay: {delay.TotalSeconds} seconds");

//            // Wait until the next execution time
//            if (delay > TimeSpan.Zero)
//            {               
//               await Task.Delay(delay, stoppingToken);
//            }

//            // Perform the daily task
//            Console.WriteLine("Run logggggggggggggggggggg: ", DateTime.Now);
//            await RunDailyTaskAsync(stoppingToken);
//        }
//    }

//    private async Task RunDailyTaskAsync(CancellationToken stoppingToken)
//    {
//        using (var scope = _serviceProvider.CreateScope())
//        {
//            // Resolve your email service or other dependencies here
//           var emailService = scope.ServiceProvider.GetRequiredService<EmailLogService>();

//            try
//            {
//                // Call your email sending function
//                Log($"Task is running at {DateTime.Now}");
//                await emailService.SendReminderEmailAsync();
//                Log($"Task is stopping at {DateTime.Now}");
//            }
//            catch (Exception ex)
//            {
//               Console.WriteLine($"Error sending reminder email: {ex.Message}");
//                // Log the exception if logging is set up
//            }
//        }
//    }
//    private bool IsWeekend(DateTime date)
//    {
//        // Returns true if the date is Saturday (6) or Sunday (0)
//        var dayOfWeek = date.DayOfWeek;
//        return dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;
//    }

//    private void Log(string message)
//    {
//        var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
//        File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
//    }
//}
