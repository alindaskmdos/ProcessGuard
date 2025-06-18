using ProcessGuard.Core;
using ProcessGuard.Events;

namespace ProcessGuard.Services.BlockEventArgs
{
    public class NetworkBlocker
    {
        private const string HostsFilePath = @"C:\Windows\System32\drivers\etc\hosts";
        private const string LocalhostIP = "127.0.0.1";

        public void Subscribe(BlockManager manager)
        {
            manager.TargetBlocked += HandleTargetBlocked;
            manager.TargetUnblocked += HandleTargetUnblocked;
        }

        private void HandleTargetBlocked(object sender, TargetBlockedEventArgs e)
        {
            if (!e.Identifier.EndsWith(".exe"))
            {
                string entryWithIp = $"{LocalhostIP} {e.Identifier}";

                var existingLines = File.ReadAllLines(HostsFilePath);
                if (!existingLines.Any(line => line.Contains(entryWithIp)))
                {
                    File.AppendAllText(HostsFilePath, entryWithIp + Environment.NewLine);
                    Console.WriteLine($"Добавлена запись: {entryWithIp}");
                }
                else
                {
                    Console.WriteLine($"Запись для домена {e.Identifier} уже существует.");
                }
            }
            else
            {
                return;
            }
        }

        private void HandleTargetUnblocked(object sender, TargetUnblockedEventArgs e)
        {
            var lines = File.ReadAllLines(HostsFilePath)
                            .Where(line => !line.Contains(e.Identifier) || line.TrimStart().StartsWith("#"))
                            .ToArray();
            File.WriteAllLines(HostsFilePath, lines);
            Console.WriteLine($"Удалена запись для домена: {e.Identifier}");
        }
    }
}