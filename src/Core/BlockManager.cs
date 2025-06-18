using ProcessGuard.Core.Models;
using ProcessGuard.Events;

namespace ProcessGuard.Core
{
    public class BlockManager
    {
        public event EventHandler<TargetBlockedEventArgs> TargetBlocked;
        public event EventHandler<TargetUnblockedEventArgs> TargetUnblocked;

        private HashSet<BlockTargetType> _blockEvents;

        public BlockManager()
        {
            _blockEvents = new HashSet<BlockTargetType>();
        }

        public void BlockTarget(BlockTargetType target)
        {
            if (_blockEvents.Contains(target))
            {
                throw new InvalidOperationException($"Таргет {target.Identifier} уже заблокирован");
            }
            _blockEvents.Add(target);

            target.BlockerTimer.BlockingStarted += () =>
            {
                TargetBlocked?.Invoke(this, new TargetBlockedEventArgs(target.Identifier));
            };

            target.BlockerTimer.BlockingEnded += () =>
            {
                TargetUnblocked?.Invoke(this, new TargetUnblockedEventArgs(target.Identifier));
            };
        }

        public void UnblockTarget(BlockTargetType target)
        {
            if (!_blockEvents.Contains(target))
            {
                throw new InvalidOperationException($"Таргет {target.Identifier} не найден.");
            }

            _blockEvents.Remove(target);

            TargetUnblocked?.Invoke(this, new TargetUnblockedEventArgs(target.Identifier));

            target.BlockerTimer.Dispose();
        }
    }
}
