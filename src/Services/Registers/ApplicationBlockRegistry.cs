using System.Collections.Generic;

using blocker.Models;

namespace blocker.Services.Registers
{
    public class ApplicationBlockRegistry
    {
        private Dictionary<BlockTargetApplication, BlockerTimer> _blockedApplications = new Dictionary<BlockTargetApplication, BlockerTimer>();

        public ApplicationBlockRegistry()
        {
            _blockedApplications = new Dictionary<BlockTargetApplication, BlockerTimer>();
        }

        public void AddBlockedApplication(BlockTargetApplication application, BlockerTimer timer)
        {
            if (!_blockedApplications.ContainsKey(application))
            {
                _blockedApplications.Add(application, timer);
                timer.BlockingStarted += () => OnBlockingStarted(application);
                timer.BlockingEnded += () => OnBlockingEnded(application);
            }

            Console.WriteLine($"Добавлена блокировка для {application.ProcessName}");
        }

        public void RemoveBlockedApplication(BlockTargetApplication application)
        {
            if (_blockedApplications.ContainsKey(application))
            {
                var timer = _blockedApplications[application];
                timer.BlockingStarted -= () => OnBlockingStarted(application);
                timer.BlockingEnded -= () => OnBlockingEnded(application);
                _blockedApplications.Remove(application);
            }

            Console.WriteLine($"Удалена блокировка для {application.ProcessName}");
        }

        private void OnBlockingStarted(BlockTargetApplication application)
        {
            Console.WriteLine($"Блокировка началась для {application.ProcessName} в {DateTime.Now:HH:mm:ss}");
        }

        private void OnBlockingEnded(BlockTargetApplication application)
        {
            Console.WriteLine($"Блокировка закончилась для {application.ProcessName} в {DateTime.Now:HH:mm:ss}");
        }
    }
}
