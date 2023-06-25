namespace LearnChannelWebApi.BackServices
{
    public class HeartBeatService : BackgroundService
    {
        private readonly HeartBeatsChannel heartBeatsChannel;

        public HeartBeatService(HeartBeatsChannel heartBeatsChannel)
        {
            this.heartBeatsChannel = heartBeatsChannel;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if(heartBeatsChannel .IsHasContent)
                    {
                        Console.WriteLine("sss");
                        Task.Factory.StartNew(() =>
                        {
                            Process(heartBeatsChannel).Wait();
                        }, TaskCreationOptions.LongRunning);
                        
                    }
                    await Task.Delay(3000, stoppingToken);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                
                
            }
        }
        /// <summary>
        /// 消费数据
        /// </summary>
        /// <param name="heartBeatsChannel"></param>
        /// <returns></returns>
        private async Task Process(HeartBeatsChannel heartBeatsChannel)
        {
            //每次消费三十个
            int count = 30;
            //进行消费
            await heartBeatsChannel.ConsumeHeartBeatAsync(count, TimeSpan.FromSeconds(3));

        }
    }
}
