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
            try
            {

                Task.Factory.StartNew(() =>
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        //阻塞的队列使得一直在同一个线程运行
                        Process(15,heartBeatsChannel).Wait();
                    }

                }, TaskCreationOptions.LongRunning);

                Console.WriteLine("主线程 现在运行的线程id为：" + Thread.CurrentThread.ManagedThreadId);

                }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        /// <summary>
        /// 消费数据
        /// </summary>
        /// <param name="count">一次消费数量</param>
        /// <param name="heartBeatsChannel"></param>
        /// <returns></returns>
        private async Task Process(int count ,HeartBeatsChannel heartBeatsChannel)
        {
            Console.WriteLine("子线程_现在运行的线程id为：" + Thread.CurrentThread.ManagedThreadId);
            //每次消费count个
            if (heartBeatsChannel.IsHasContent)
            {
                //进行消费
                await heartBeatsChannel.ConsumeHeartBeatAsync(count, TimeSpan.FromSeconds(3));
            }           
            await Task.Delay(3000);
        }
    }
}
