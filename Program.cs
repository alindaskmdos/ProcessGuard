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
            if (!PasswordManager.IsPasswordSet())
            {
                PasswordManager.SetPassword();
            }

            Console.Clear();
            Console.WriteLine("ProcessGuard - Родительский контроль");
            Console.WriteLine("\nКоманды:");
            Console.WriteLine("  block app notepad 30    - заблокировать приложение на 30 мин");
            Console.WriteLine("  block site google.com   - заблокировать сайт на 60 мин");
            Console.WriteLine("  unblock app notepad     - разблокировать приложение");
            Console.WriteLine("  unblock site google.com - разблокировать сайт");
            Console.WriteLine("  exit                    - выход\n");

            BlockManager blockManager = new BlockManager();
            ApplicationBlocker applicationBlocker = new ApplicationBlocker();
            NetworkBlocker networkBlocker = new NetworkBlocker();

            while (true)
            {
                Console.Write("> ");
                string command = Console.ReadLine() ?? "";
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
                                Console.WriteLine($"Заблокирован сайт '{siteName}' на {minutes} мин");
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
                                };
                                applicationBlocker.Subscribe(blockManager);
                                blockManager.BlockTarget(appTarget);
                                Console.WriteLine($"Заблокировано приложение '{appName}' на {minutes} мин");
                            }
                            else
                            {
                                Console.WriteLine("Используйте: block app notepad 30 или block site google.com 60");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Используйте: block app notepad 30 или block site google.com 60");
                        }
                        break;
                    case "unblock":
                        if (PasswordManager.CheckPassword())
                        {
                            if (parts.Length >= 3)
                            {
                                string target = parts[1].ToLower();
                                if (target == "site")
                                {
                                    string siteName = parts[2];
                                    var siteTarget = new BlockTargetSite { Identifier = siteName };
                                    blockManager.UnblockTarget(siteTarget);
                                    Console.WriteLine($"Разблокирован сайт '{siteName}'");
                                }
                                else if (target == "app")
                                {
                                    string appName = parts[2];
                                    var appTarget = new BlockTargetProcess
                                    {
                                        Identifier = appName.EndsWith(".exe") ? appName : appName + ".exe"
                                    };
                                    blockManager.UnblockTarget(appTarget);
                                    Console.WriteLine($"Разблокировано приложение '{appName}'");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Используйте: unblock app notepad или unblock site google.com");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Неверный пароль!");
                        }
                        break;
                    case "exit":
                        return;

                    default:
                        if (!string.IsNullOrEmpty(command))
                            Console.WriteLine("Неизвестная команда.");
                        break;
                }
            }
        }
    }
}