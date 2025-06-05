namespace blocker.Services
{
    public class HostsFileManager
    {
        private const string HostsFilePath = @"C:\Windows\System32\drivers\etc\hosts";
        private const string LocalhostIP = "127.0.0.1";

        public void AddEntry(string entry)
        {
            string temp_entry = $"{LocalhostIP} {entry}";

            var existingLines = File.ReadAllLines(HostsFilePath);
            if (!existingLines.Any(line => line.Contains(temp_entry)))
            {
                File.AppendAllText(HostsFilePath, temp_entry + Environment.NewLine);
                Console.WriteLine($"Добавлена запись: {temp_entry}");
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

        public List<string> GetEntries()
        {
            return File.ReadAllLines(HostsFilePath).ToList();
        }
    }
}