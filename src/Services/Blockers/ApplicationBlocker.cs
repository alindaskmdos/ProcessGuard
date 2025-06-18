using System.Diagnostics;
using Microsoft.Win32;

using ProcessGuard.Core;
using ProcessGuard.Events;

namespace ProcessGuard.Services.BlockEventArgs
{
    public class ApplicationBlocker
    {
        public void Subscribe(BlockManager manager)
        {
            manager.TargetBlocked += HandleTargetBlocked;
            manager.TargetUnblocked += HandleTargetUnblocked;
        }
        private async void HandleTargetBlocked(object? sender, TargetBlockedEventArgs e)
        {
            if (e.Identifier.EndsWith(".exe"))
            {
                await BlockProcessAsync(e.Identifier);
            }
        }

        private async void HandleTargetUnblocked(object? sender, TargetUnblockedEventArgs e)
        {
            if (e.Identifier.EndsWith(".exe"))
            {
                await UnblockProcessAsync(e.Identifier);
            }
        }

        public async Task BlockProcessAsync(string processName)
        {
            try
            {
                var paths = FindAllExecutablePaths(processName);

                if (!paths.Any())
                {
                    Console.WriteLine($"Исполняемый файл {processName} не найден!");
                    return;
                }

                foreach (string path in paths)
                {
                    await BlockExecutable(path);
                    Console.WriteLine($"Заблокирован {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка {processName} {ex.Message}");
            }
        }

        public async Task UnblockProcessAsync(string processName)
        {
            try
            {
                var paths = FindAllExecutablePaths(processName);

                foreach (string path in paths)
                {
                    await UnblockExecutable(path);
                    Console.WriteLine($"{path} разблокировн");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка {processName} {ex.Message}");
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------

        private List<string> FindAllExecutablePaths(string processName)
        {
            var allPaths = new HashSet<string>();

            allPaths.UnionWith(FindInRunningProcesses(processName));
            allPaths.UnionWith(FindAllPaths(processName));

            var registryPath = FindInRegistry(processName);
            if (!string.IsNullOrEmpty(registryPath))
                allPaths.Add(registryPath);

            return allPaths.Where(File.Exists).ToList();
        }

        private IEnumerable<string> FindInRunningProcesses(string processName)
        {
            try
            {
                string nameWithoutExtension = Path.GetFileNameWithoutExtension(processName);

                return Process.GetProcessesByName(nameWithoutExtension)
                             .Where(p => !string.IsNullOrEmpty(p.MainModule?.FileName))
                             .Select(p => p.MainModule.FileName)
                             .Distinct();
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        private List<string> FindAllPaths(string processName)
        {
            var allPaths = new HashSet<string>();

            allPaths.UnionWith(FindInRunningProcesses(processName));

            var registryPath = FindInRegistry(processName);
            if (!string.IsNullOrEmpty(registryPath))
                allPaths.Add(registryPath);

            allPaths.UnionWith(FindInSystemFolders(processName));

            return allPaths.Where(File.Exists).ToList();
        }

        private IEnumerable<string> FindInSystemFolders(string processName)
        {
            var paths = new List<string>();

            var systemFolders = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.System),
                Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)),
                @"C:\Windows\SysWOW64"
            };
            foreach (var folder in systemFolders)
            {
                if (Directory.Exists(folder))
                {
                    var fullPath = Path.Combine(folder, processName);
                    if (File.Exists(fullPath))
                    {
                        paths.Add(fullPath);
                    }
                }
            }

            return paths;
        }

        private string FindInRegistry(string processName)
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey($@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\{processName}");
                return key?.GetValue("")?.ToString();
            }
            catch
            {
                return null;
            }
        }
        private async Task BlockExecutable(string exePath)
        {
            await RunIcaclsCommand($"\"{exePath}\" /deny *S-1-1-0:(X)");
        }

        private async Task UnblockExecutable(string exePath)
        {
            await RunIcaclsCommand($"\"{exePath}\" /grant *S-1-1-0:(RX)");
        }
        private async Task RunIcaclsCommand(string arguments)
        {
            try
            {
                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "icacls",
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                });

                if (process != null)
                {
                    await process.WaitForExitAsync();

                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();

                    if (process.ExitCode == 5)
                    {
                        if (arguments.Contains("Windows\\System") || arguments.Contains("Windows\\SysWOW64"))
                        {
                            return;
                        }
                    }

                    if (process.ExitCode == 0 || process.ExitCode == 1)
                    {
                        return;
                    }

                    throw new Exception($"icacls ошибка с кодом {process.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка {ex.Message}");
                throw;
            }
        }
    }
}