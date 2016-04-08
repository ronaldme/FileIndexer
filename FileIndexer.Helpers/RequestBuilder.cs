using Messages;

namespace FileIndexer.Helpers
{
    public class RequestBuilder
    {
        public static SyncRequest GetSyncRequest(string input)
        {
            var results = input.Split(' ');
            return new SyncRequest
            {
                MainDirectory = results[0],
                SyncDirectory = results[1],
                SyncCommand = results.Length == 3 ? SyncCommands.WithDelete : SyncCommands.None
            };
        }
    }
}