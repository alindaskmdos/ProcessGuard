namespace ProcessGuard.Events
{
    public class BlockEventArgs : EventArgs
    {
        public string Identifier { get; }
        public DateTime Timestamp { get; }
        protected BlockEventArgs(string identifier)
        {
            Identifier = identifier;
            Timestamp = DateTime.Now;
        }
    }

    public class TargetBlockedEventArgs : BlockEventArgs
    {
        public TargetBlockedEventArgs(string identifier) : base(identifier) { }
    }

    public class TargetUnblockedEventArgs : BlockEventArgs
    {
        public TargetUnblockedEventArgs(string identifier) : base(identifier) { }
    }
}