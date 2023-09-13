using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnChannelWebApi.Controllers
{   
    
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TwoHeartBeatChannelController : ControllerBase
    {
        private readonly HeartBeatsChannel heartBeatsChannel;

        public TwoHeartBeatChannelController(HeartBeatsChannel heartBeatsChannel)
        {
            this.heartBeatsChannel = heartBeatsChannel;
        }
        //不同的请求增加消息
        [HttpGet]
        public async Task TwoProduceHeartBeatAsync(int count)
        {
            int rcount = 0;
            while (rcount < count)
            {
                await heartBeatsChannel.ProduceHeartBeat("two " + rcount.ToString());
                rcount++;
            }
        }
        /// <summary>
        /// 消费数据
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task ConsumeHeartBeatAsync(int count)
        {
            //开启多个消费者进行处理查看数据是否正确
            var task1 = heartBeatsChannel.ConsumeHeartBeatAsync(count, TimeSpan.FromSeconds(10));
            var task2 = heartBeatsChannel.ConsumeHeartBeatAsync(count, TimeSpan.FromSeconds(10));
            //var task3 = heartBeatsChannel.ConsumeHeartBeatAsync(count, TimeSpan.FromSeconds(10));
            await Task.WhenAll(task1, task2);

        }
    }
}
