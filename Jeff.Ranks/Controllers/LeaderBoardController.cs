using Jeff.Ranks.Common;
using Jeff.Ranks.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;

namespace Jeff.Ranks.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeaderBoardController : ControllerBase
    {
        private readonly ChannelWriter<RankQueueJob> _writer;
        private readonly ILogger<LeaderBoardController> _logger;

        public LeaderBoardController(ChannelWriter<RankQueueJob> writer, ILogger<LeaderBoardController> logger)
        {
            _writer = writer;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<dynamic>> Query([FromQuery]int start, [FromQuery] int end, CancellationToken ct)
        {
            _logger.LogInformation($"Method:Query start:{start} end:{end}");
            var tcs = new TaskCompletionSource<object>();
            var job = new RankQueueJob(RankQueueJobType.Query, new { start, end }, tcs);

            await _writer.WriteAsync(job);

            var result = await tcs.Task;
            return result;
        }


        [HttpGet("{customerId}")]
        public async Task<ActionResult<dynamic>> Range([FromRoute]long customerId, [FromQuery] int high, [FromQuery] int low, CancellationToken ct)
        {
            _logger.LogInformation($"Method:Range customerId:{customerId} high:{high} low:{low}");
            var tcs = new TaskCompletionSource<object>();
            var job = new RankQueueJob( RankQueueJobType.Range, new { customerId, high, low }, tcs);

            await _writer.WriteAsync(job);

            var result = await tcs.Task;
            return result;
        }
    }
}
