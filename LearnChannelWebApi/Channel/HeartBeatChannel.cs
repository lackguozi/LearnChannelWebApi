using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace LearnChannelWebApi
{
    /// <summary>
    /// 心跳channel
    /// </summary>
    public class HeartBeatsChannel
    {
        private readonly Channel<string> channel = Channel.CreateBounded<string>(new BoundedChannelOptions(1500)
        {
            FullMode = BoundedChannelFullMode.Wait
        });

        public HeartBeatsChannel()
        {

        }
        public async Task ProduceHeartBeat(string message)
        {
            await channel.Writer.WriteAsync(message);
        }
        /// <summary>
        /// 一直不停的消费
        /// </summary>
        /// <returns></returns>
        public async Task ConsumeHeartBeat()
        {
            while (await channel.Reader.WaitToReadAsync())
            {
                if (channel.Reader.TryRead(out var number))
                {
                    Console.WriteLine(number);
                }
            }
        }
        
        /// <summary>
        /// 一次消费多少个数据
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<List<string>> ConsumeHeartBeatAsync(int count)
        {
            var result = new List<string>(count);
            int rcount = 0;
            while (rcount <= count)
            {
                while (await channel.Reader.WaitToReadAsync())
                {
                    if (channel.Reader.TryRead(out var number))
                    {
                        Console.WriteLine(number);
                        result.Add(number);
                    }
                    rcount++;
                }
            }
            return result;
        }
        /// <summary>
        /// timespan时间内消费多少数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public async Task<List<string>> ConsumeHeartBeatAsync(int count,TimeSpan timeSpan)
        {
            var result = new List<string>(count);
            CancellationTokenSource cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            cts.CancelAfter(timeSpan);
            int rcount = 0;
           //时间到了或者消费个数到了停止消费
            while ( !cancellationToken.IsCancellationRequested && rcount<count)
            {
                //await Task.Delay(2000);
                if (channel.Reader.TryRead(out var number))
                {
                    Console.WriteLine(number);
                    result.Add(number);
                    rcount++;
                }
                else //没有数据了直接退出
                {
                    break;
                }
                
            }  
            return result;
        }
        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task CancelTaskAsync(Task task)
        {
            var cts = new CancellationTokenSource();
            try
            {
                cts.Cancel();
                await task;
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("任务取消");
            }
        }
        public bool IsHasContent=> channel.Reader.Count > 0;
    }
    internal class Producer<T> where T : class
    {
        private readonly ChannelWriter<T> _writer;
        private readonly int _identifier;
        private readonly int _delay;

        public Producer(ChannelWriter<T> writer, int identifier, int delay)
        {
            _writer = writer;
            _identifier = identifier;
            _delay = delay;
        }

        public async Task BeginProducing(T message)
        {
            Console.WriteLine($"PRODUCER ({_identifier}): Starting");

            for (var i = 0; i < 10; i++)
            {
                await Task.Delay(_delay); // simulate fetching some data

                var msg = $"P{_identifier} - {DateTime.Now:G}";

                Console.WriteLine($"PRODUCER ({_identifier}): Creating {msg}");

                await _writer.WriteAsync(message);
            }

            Console.WriteLine($"PRODUCER ({_identifier}): Completed");
        }
    }
    internal class Consumer<T> where T: class
    {
        private readonly ChannelReader<T> _reader;
        private readonly int _identifier;
        private readonly int _delay;

        public Consumer(ChannelReader<T> reader, int identifier, int delay)
        {
            _reader = reader;
            _identifier = identifier;
            _delay = delay;
        }

        public async Task ConsumeData()
        {
            Console.WriteLine($"CONSUMER ({_identifier}): Starting");

            while (await _reader.WaitToReadAsync())
            {
                if (_reader.TryRead(out var timeString))
                {
                    await Task.Delay(_delay); // simulate processing time

                    Console.WriteLine($"CONSUMER ({_identifier}): Consuming {timeString}");
                }
            }

            Console.WriteLine($"CONSUMER ({_identifier}): Completed");
        }
    }
}
