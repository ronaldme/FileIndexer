using System.Configuration;
using EasyNetQ;
using FileIndexer.Helpers;
using Messages;

namespace FileIndexer.Console
{
    public class Program
    {
        private static readonly IBus Bus = RabbitHutch.CreateBus(ConfigurationManager.AppSettings["easynetq"]);

        public static void Main(string[] args)
        {
            string format = @"Format: C:\Testing D:\Testing -d (delete)";
            System.Console.WriteLine($"Commands: 'Exit' or 'Help'.. \n{format}");
            
            string input = null;

            while (input != "Exit")
            {
                input = System.Console.ReadLine();

                if (input != null)
                {
                    if (input.ToLower() == "help")
                    {
                        System.Console.WriteLine(format);
                    }
                    else if (UserInputValidation.ValidInputFormat(input))
                    {
                        var request = RequestBuilder.GetSyncRequest(input);

                        var response = Bus.Request<SyncRequest, SyncResponse>(request);
                        System.Console.WriteLine($"Files synced from {request.MainDirectory}, to: " +
                                                 $"{request.SyncDirectory}, in {response.SyncTime} ms.");
                    }
                }
            }
        }
    }
}