using System.Configuration;
using EasyNetQ;
using Topshelf;

namespace FileIndexer.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IBus bus = RabbitHutch.CreateBus(ConfigurationManager.AppSettings["easynetq"]);

            HostFactory.Run(x =>
            {
                x.Service<FileIndexerService>(s =>
                {
                    s.ConstructUsing(name => new FileIndexerService(bus));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();
                 
                x.SetDescription("Manage files indexing.");
                x.SetDisplayName("FileIndexer");
                x.SetServiceName("FileIndexer");
            });
        }
    }
}