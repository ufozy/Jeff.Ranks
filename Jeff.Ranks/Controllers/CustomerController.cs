using Jeff.Ranks.Common;
using Jeff.Ranks.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;

namespace Jeff.Ranks.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ChannelWriter<RankQueueJob> _writer;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ChannelWriter<RankQueueJob> writer, ILogger<CustomerController> logger)
        {
            _writer = writer;
            _logger = logger;
        }

        [HttpPost("{customerId}/score/{score}")]
        public async Task<ActionResult<dynamic>> Score(long customerId, int score, CancellationToken ct)
        {
            _logger.LogInformation($"Method:Score customerId:{customerId} score:{score}");
            var tcs = new TaskCompletionSource<object>();
            var job = new RankQueueJob(RankQueueJobType.Score, new { customerId, score }, tcs);

            await _writer.WriteAsync(job);

            var result = await tcs.Task;
            return result;
        }
    }
}
