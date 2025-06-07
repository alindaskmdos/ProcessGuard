using System;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Security.Principal;

using blocker.Models;
using blocker.Services;
using blocker.Services.Registers;

namespace blocker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // try
            // {
            //     WebsiteBlockRegistry hostExecutionContext = new WebsiteBlockRegistry();
            //     string blockEntry = "www.twitch.tv";

            //     BlockerTimer blockTimer = new BlockerTimer(DateTime.Now, DateTime.Now.AddMinutes(2));

            //     blockTimer.BlockingStarted += () =>
            //     {
            //         Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Блокировка началась");
            //         hostExecutionContext.AddEntry(blockEntry);
            //     };

            //     blockTimer.BlockingEnded += () =>
            //     {
            //         Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Блокировка началась");
            //         hostExecutionContext.RemoveEntry(blockEntry);
            //     };

            //     Console.WriteLine($"Блокиратор запущен! Время: {DateTime.Now:HH:mm:ss}");
            //     Console.WriteLine($"Блокировка будет с {DateTime.Now:HH:mm:ss} до {DateTime.Now.AddMinutes(2):HH:mm:ss}");
            //     Console.WriteLine("Нажмите 'q' для выхода или любую другую клавишу для проверки статуса...");

            //     ConsoleKeyInfo key;
            //     do
            //     {
            //         key = Console.ReadKey(true);
            //         if (key.KeyChar != 'q')
            //         {
            //             Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Программа работает... (нажмите 'q' для выхода)");
            //         }
            //     } while (key.KeyChar != 'q');

            //     blockTimer.Dispose();
            //     Console.WriteLine("Программа завершена.");
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine($"Ошибка: {ex.Message}");
            //     Console.ReadKey();
            // }
        }
    }
}