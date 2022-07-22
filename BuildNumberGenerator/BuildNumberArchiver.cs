using System.Collections.Concurrent;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using IvyTech.Utilities;

namespace BuildNumberGenerator
{
    class BuildNumberQueueItem
    {
        public DateTime Timestamp {get; set;}  // Always UTC
        public string? Identity {get; set;}
        public string? Branch {get;set;}
        public int Number {get; set;}
    }

    public class BuildNumberArchiver: IBuildNumberArchiver
    {
        private readonly ILogger<BuildNumberArchiver> _logger;
        private readonly BatchSender<BuildNumberQueueItem> _batchSender;
        private ElasticsearchClient _elasticsearchClient;
        public BuildNumberArchiver(ILogger<BuildNumberArchiver> logger, IBuildNumberArchiverConfig config)
        {
            if (config is null || 
                config.SendCount <= 0 ||
                config.SendDelay <= 0 ||
                config.ElasticSearchURL is null ||
                config.ApiKey is null)
            {
                throw new ArgumentException("BuildNumberArchiverConfig invalid");
            }

            logger.LogInformation("buildnumberarchiver starting");

            _logger = logger;
            _batchSender = new BatchSender<BuildNumberQueueItem>(config.SendCount, config.SendDelay);
            _batchSender.Send = Send;

            var apiKey = new ApiKey(config.ApiKey);
            var settings = new ElasticsearchClientSettings(new Uri(config.ElasticSearchURL))
                .Authentication(apiKey)
                .DefaultIndex("buildnumbers-2022"); // todo dynamic index

            _elasticsearchClient = new ElasticsearchClient(settings);
        }

        public void Queue(BuildNumber build)
        {
            _batchSender.Queue(new BuildNumberQueueItem()
            {
                Timestamp = build.BuildTime,
                Identity = build.Id,
                Branch = build.Branch,
                Number = build.Number
            });
        }

        public async Task StopAndWaitForFlush()
        {
            _batchSender.startShutdown();
            await _batchSender.WaitForShutdownComplete();
            _logger.LogInformation("BatchSender shutdown complete");
        }

        private async Task<bool> Send(IEnumerable<BuildNumberQueueItem> batch)
        { 
            _logger.LogInformation("indexing {count}", batch.ToArray().Length);

            // todo, look into errors this can have
            await _elasticsearchClient.IndexManyAsync<BuildNumberQueueItem>(batch);

            return true;
        }
    }
}