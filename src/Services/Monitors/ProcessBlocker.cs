using System.Diagnostics;
using System.Management;

using ProcessGuard.Models;

namespace ProcessGuard.Services.Monitors
{
    public class ProcessBlocker
    {
        private ManagementEventWatcher _processStartWatcher;
        public HashSet<string> _blockedProcessNames;

        public ProcessBlocker()
        {
            _processStartWatcher = new ManagementEventWatcher();
            _blockedProcessNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            SetupProcessStartWatcher();
        }
        public void BlockApplicationOnStart(BlockTargetApplication application)
        {
            _blockedProcessNames.Add(application.ProcessName);

            Process[] processes = Process.GetProcessesByName(application.ProcessName);
            if (processes.Length > 0)
            {
                foreach (var process in processes)
                {
                    try
                    {
                        process.Kill();
                        Console.WriteLine($"Завершен существующий процесс: {process.ProcessName} (ID: {process.Id})");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Не удалось завершить процесс {process.ProcessName}: {ex.Message}");
                    }
                }
            }

            Console.WriteLine($"Процесс '{application.ProcessName}' добавлен в список блокировки");
        }

        private void SetupProcessStartWatcher()
        {
            var query = new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace");
            _processStartWatcher = new ManagementEventWatcher(query);
            _processStartWatcher.EventArrived += ProcessStarted;
            _processStartWatcher.Start();
        }
        private void ProcessStarted(object sender, EventArrivedEventArgs e)
        {
            try
            {
                int processId = Convert.ToInt32(e.NewEvent["ProcessID"]);
                string processName = e.NewEvent["ProcessName"]?.ToString();

                if (processId <= 0)
                {
                    return;
                }

                Console.WriteLine($"{processId} {processName}");

                Process? process = null;
                try
                {
                    process = Process.GetProcessById(processId);
                    if (process == null || string.IsNullOrEmpty(process.ProcessName))
                    {
                        return;
                    }
                }
                catch (ArgumentException)
                {
                    return;
                }

                if (_blockedProcessNames.Contains(process.ProcessName))
                {
                    try
                    {
                        process.Kill();
                        Console.WriteLine($"Заблокирован процесс: {process.ProcessName} (ID: {processId})");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при блокировке процесса {process.ProcessName}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в ProcessStarted: {ex.Message}");
            }
        }

        public void UnblockApplication(string processName)
        {
            _blockedProcessNames.Remove(processName);
            Console.WriteLine($"Процесс '{processName}' удален из списка блокировки");
        }


        public void Dispose()
        {
            _processStartWatcher?.Stop();
            _processStartWatcher?.Dispose();
        }
    }
}