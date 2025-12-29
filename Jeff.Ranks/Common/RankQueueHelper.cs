using Jeff.Ranks.Models;
using System.Threading.Channels;

namespace Jeff.Ranks.Common
{
    public enum RankQueueJobType
    {
        None = 0,
        Query = 1,
        Range = 2,
        Score = 3,
    }

    public record RankQueueJob(
         RankQueueJobType JobType,
        dynamic Model,
         TaskCompletionSource<dynamic> Completion);

    /// <summary>
    /// Queue consumers
    /// </summary>
    public class BackgroundJobProcessor : BackgroundService
    {
        private readonly ChannelReader<RankQueueJob> _reader;

        public BackgroundJobProcessor(ChannelReader<RankQueueJob> reader)
        {
            _reader = reader;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var job in _reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    object result = job.JobType switch
                    {
                        RankQueueJobType.Query => RankReport.RankSection(job.Model.start, job.Model.end),
                        RankQueueJobType.Range => RankReport.RankSection(job.Model.customerId, job.Model.high, job.Model.low),
                        RankQueueJobType.Score => new CustomerRankHandle(job.Model.customerId).SetScore(job.Model.score) ,
                        _ => new { Error = "Unknown job type" }
                    };

                    job.Completion.SetResult(result);
                }
                catch (Exception ex)
                {
                    job.Completion.SetException(ex);
                }
            }
        }
    }
}
