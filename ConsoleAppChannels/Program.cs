using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ConsoleAppChannels
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const int max = 10;
            var c = Channel.CreateUnbounded<int>(new UnboundedChannelOptions { SingleReader = true });

            _ = Task.Run(async () =>
            {
                for (int i = 0; i < max; i++)
                {
                    await Task.Delay(100);
                    await c.Writer.WriteAsync(i);
                }
            });

            _ = Task.Run(async () =>
            {
                for (int i = 1000; i < 1000 + max; i++)
                {
                    await Task.Delay(100);
                    await c.Writer.WriteAsync(i);
                }
            });

            while (await c.Reader.WaitToReadAsync())
            {
                if (c.Reader.TryRead(out var item))
                {
                    Console.WriteLine(item);
                }
            }

            // or

            await foreach (var item in c.Reader.ReadAllAsync())
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("end");
        }
    }
}