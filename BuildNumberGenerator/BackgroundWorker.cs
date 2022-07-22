namespace BuildNumberGenerator
{
    public class BackgroundWorker : BackgroundService
    {
        private ILogger<BackgroundWorker> _logger;
        private IBuildNumberArchiver _archiver;

        public BackgroundWorker(ILogger<BackgroundWorker> logger, IBuildNumberArchiver archiver)
        {
            _logger = logger;
            _archiver = archiver;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // todo start something to purge periodically
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting shutdown");
           await _archiver.StopAndWaitForFlush();
        }
    }
}