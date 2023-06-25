using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnChannelWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HeartBeatController : ControllerBase
    {
        private readonly HeartBeatsChannel heartBeatsChannel;

        //private readonly 
        public HeartBeatController(HeartBeatsChannel heartBeatsChannel)
        {
            this.heartBeatsChannel = heartBeatsChannel;
        }
        [HttpGet]
        public async Task ProduceHeartBeatAsync(int count)
        {
            int rcount = 0;
            while(rcount < count)
            {
                await heartBeatsChannel.ProduceHeartBeat(rcount.ToString());  
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
            var task1 =  heartBeatsChannel.ConsumeHeartBeatAsync(count,TimeSpan.FromSeconds(10));
            var  task2 = heartBeatsChannel.ConsumeHeartBeatAsync(count, TimeSpan.FromSeconds(10));
            var task3 = heartBeatsChannel.ConsumeHeartBeatAsync(count, TimeSpan.FromSeconds(10));
            await Task.WhenAll(task1, task2, task3);
            /*foreach(var item in list)
            {   
                Console.WriteLine(item);
            }*/

        }
    }
}
