namespace Messages
{
    public enum SyncCommands
    {
        None,
        /// <summary>
        /// Delets file from the sync location that are not in the main directory.
        /// </summary>
        WithDelete,
    }
}
