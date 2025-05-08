using System.Threading.Channels;

namespace BPMaster.Common.Services.QueueBackgroundService
{
    public interface ITaskQueue
    {
       
    }

    public class TaskQueue : ITaskQueue
    {
        
    }

    public class MyBackgroundService : BackgroundService
    {
        private readonly ITaskQueue _taskQueue;
        private readonly IServiceProvider _service;

        public MyBackgroundService(ITaskQueue taskQueue, IServiceProvider service)
        {
            _taskQueue = taskQueue;
            _service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error dequeuing task: {ex.Message}");
            }
           
        }

       
    }
}
