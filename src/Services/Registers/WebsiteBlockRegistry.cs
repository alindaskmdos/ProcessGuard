using System.Collections.Generic;

using blocker.Models;

namespace blocker.Services.Registers
{
    public class WebsiteBlockRegistry
    {
        private Dictionary<BlockTargetSite, BlockerTimer> _blockedWebSites = new Dictionary<BlockTargetSite, BlockerTimer>();

        public WebsiteBlockRegistry()
        {
            _blockedWebSites = new Dictionary<BlockTargetSite, BlockerTimer>();
        }

        public void AddEntry(BlockTargetSite entry, BlockerTimer timer)
        {
            if (!_blockedWebSites.ContainsKey(entry))
            {
                _blockedWebSites.Add(entry, timer);
                timer.BlockingStarted += () => OnBlockingStarted(entry);
                timer.BlockingEnded += () => OnBlockingEnded(entry);
            }

            Console.WriteLine($"Добавлена блокировка для домена: {entry.Domain}");
        }

        private void OnBlockingStarted(BlockTargetSite entry)
        {
            Console.WriteLine($"Блокировка началась для домена {entry.Domain} в {DateTime.Now:HH:mm:ss}");
        }

        private void OnBlockingEnded(BlockTargetSite entry)
        {
            Console.WriteLine($"Блокировка закончилась для домена {entry.Domain} в {DateTime.Now:HH:mm:ss}");
        }

        public void RemoveEntry(string entry)
        {
            var targetSite = _blockedWebSites.Keys.FirstOrDefault(site => site.Domain == entry);
            if (targetSite != null)
            {
                _blockedWebSites.Remove(targetSite);
                Console.WriteLine($"Удалена блокировка для домена: {entry}");
            }
            else
            {
                Console.WriteLine($"Блокировка для домена {entry} не найдена.");
            }
        }
    }
}