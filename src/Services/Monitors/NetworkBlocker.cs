using System.Collections.Generic;

using ProcessGuard.Models;
using ProcessGuard.Services;

namespace ProcessGuard.Services.Monitors
{
    public class NetworkBlocker
    {
        private const string HostsFilePath = @"C:\Windows\System32\drivers\etc\hosts";
        private const string LocalhostIP = "127.0.0.1";

        public void AddEntry(BlockTargetSite entry)
        {
            string entryWithIp = $"{LocalhostIP} {entry.Domain}";

            var existingLines = File.ReadAllLines(HostsFilePath);
            if (!existingLines.Any(line => line.Contains(entryWithIp)))
            {
                File.AppendAllText(HostsFilePath, entryWithIp + Environment.NewLine);
                Console.WriteLine($"Добавлена запись: {entryWithIp}");
            }
            else
            {
                Console.WriteLine($"Запись для домена {entry} уже существует.");
            }
        }

        public void RemoveEntry(string entry)
        {
            var lines = File.ReadAllLines(HostsFilePath)
                .Where(line => !line.Contains(entry) || line.TrimStart().StartsWith("#"))
                .ToArray();
            File.WriteAllLines(HostsFilePath, lines);
            Console.WriteLine($"Удалена запись для домена: {entry}");
        }
    }
}