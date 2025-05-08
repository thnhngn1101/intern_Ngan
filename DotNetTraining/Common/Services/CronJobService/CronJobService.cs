using Cronos;

namespace BPMaster.Common.Services.CronJobService
{
    public class CronJobService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _cronExpression;
        private Timer _timer;

        public CronJobService(IServiceProvider serviceProvider, string cronExpression)
        {
            _serviceProvider = serviceProvider;
            _cronExpression = cronExpression;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ScheduleTask(stoppingToken);
            return Task.CompletedTask;
        }

        private void ScheduleTask(CancellationToken stoppingToken)
        {
            var cronSchedule = CronExpression.Parse(_cronExpression);
            var nextOccurrence = cronSchedule.GetNextOccurrence(DateTime.UtcNow);

            if (nextOccurrence.HasValue)
            {
                var delay = nextOccurrence.Value - DateTime.UtcNow;
                _timer = new Timer(async _ =>
                {
                    _timer?.Change(Timeout.Infinite, 0); // Stop the timer while executing
                    await ExecuteScheduledTask(stoppingToken);
                    ScheduleTask(stoppingToken); // Schedule the next task
                }, null, delay, Timeout.InfiniteTimeSpan);
            }
        }

        private async Task ExecuteScheduledTask(CancellationToken stoppingToken)
        {
            
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}
