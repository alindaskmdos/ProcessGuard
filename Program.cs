using ProcessGuard.Core;
using ProcessGuard.Core.Models;
using ProcessGuard.Services.BlockEventArgs;
using ProcessGuard.Services;

namespace ProcessGuard
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BlockManager blockManager = new BlockManager();
            ApplicationBlocker applicationBlocker = new ApplicationBlocker();
            NetworkBlocker networkBlocker = new NetworkBlocker(); string command = String.Empty;
            while (true)
            {
                Console.WriteLine("\nДоступные команды:");
                Console.WriteLine("  block <app/site> <название> [минуты]  - заблокировать приложение/сайт");
                Console.WriteLine("  unblock <app/site> <название>         - разблокировать приложение/сайт");
                Console.WriteLine("  exit                                   - выход из программы\n");
                Console.Write("Введите команду: ");

                command = Console.ReadLine() ?? "";

                string[] parts = command?.Split(' ') ?? new string[0];

                switch (parts.Length > 0 ? parts[0].ToLower() : "")
                {
                    case "block":
                        if (parts.Length >= 3)
                        {
                            string target = parts[1].ToLower();
                            if (target == "site")
                            {
                                string siteName = parts[2];
                                int minutes = parts.Length > 3 && int.TryParse(parts[3], out int min) ? min : 60;

                                DateTime startTime = DateTime.Now;
                                DateTime endTime = startTime.AddMinutes(minutes);

                                var siteTarget = new BlockTargetSite
                                {
                                    Identifier = siteName,
                                    BlockerTimer = new BlockerTimer(startTime, endTime)
                                }; networkBlocker.Subscribe(blockManager);
                                blockManager.BlockTarget(siteTarget);
                                Console.WriteLine($"\n  Сайт '{siteName}' успешно заблокирован на {minutes} минут");
                                Console.WriteLine($"  Время блокировки: {startTime:HH:mm} - {endTime:HH:mm}");
                            }
                            else if (target == "app")
                            {
                                string appName = parts[2];
                                int minutes = parts.Length > 3 && int.TryParse(parts[3], out int min) ? min : 60;

                                DateTime startTime = DateTime.Now;
                                DateTime endTime = startTime.AddMinutes(minutes);

                                var appTarget = new BlockTargetProcess
                                {
                                    Identifier = appName.EndsWith(".exe") ? appName : appName + ".exe",
                                    BlockerTimer = new BlockerTimer(startTime, endTime)
                                }; applicationBlocker.Subscribe(blockManager);
                                blockManager.BlockTarget(appTarget);
                                Console.WriteLine($"\n  Приложение '{appName}' успешно заблокировано на {minutes} минут");
                                Console.WriteLine($"  Время блокировки: {startTime:HH:mm} - {endTime:HH:mm}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("\n  Ошибка: Неверный формат команды");
                            Console.WriteLine("  Используйте: block <app/site> <название> [минуты]");
                            Console.WriteLine("  Пример: block app notepad 30");
                        }
                        break;
                    case "unblock":
                        if (parts.Length >= 3)
                        {
                            string target = parts[1].ToLower(); if (target == "site")
                            {
                                string siteName = parts[2];
                                var siteTarget = new BlockTargetSite { Identifier = siteName };
                                blockManager.UnblockTarget(siteTarget);
                                Console.WriteLine($"\n  Сайт '{siteName}' успешно разблокирован");
                            }
                            else if (target == "app")
                            {
                                string appName = parts[2];
                                var appTarget = new BlockTargetProcess
                                {
                                    Identifier = appName.EndsWith(".exe") ? appName : appName + ".exe"
                                };
                                blockManager.UnblockTarget(appTarget);
                                Console.WriteLine($"\n  Приложение '{appName}' успешно разблокировано");
                            }
                        }
                        else
                        {
                            Console.WriteLine("\n  Ошибка: Неверный формат команды");
                            Console.WriteLine("  Используйте: unblock <app/site> <название>");
                            Console.WriteLine("  Пример: unblock app notepad");
                        }
                        break;
                    case "exit":
                        Console.WriteLine();
                        return;

                    default:
                        Console.WriteLine("\n  Неизвестная команда\n");
                        break;
                }
            }
        }
    }
}