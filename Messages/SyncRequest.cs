namespace Messages
{
    public class SyncRequest
    {
        /// <summary>
        /// The directory which is leading. Files within this location are
        /// copied to the sync directory.
        /// </summary>
        public string MainDirectory { get; set; }
        /// <summary>
        /// Files that are in the main directory but not here are copied to this directory.
        /// </summary>
        public string SyncDirectory { get; set; }
        public SyncCommands SyncCommand { get; set; }
    }
}
