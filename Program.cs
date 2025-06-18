using System;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Management;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ProcessGuard.Models;
using ProcessGuard.Services;
using ProcessGuard.Services.Registers;
using ProcessGuard.Services.Monitors;

namespace ProcessGuard
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddSingleton<NetworkBlocker>();
            services.AddSingleton<ApplicationBlockRegistry>();
            services.AddSingleton<WebsiteBlockRegistry>();
            services.AddSingleton<ProcessBlocker>();

            var serviceProvider = services.BuildServiceProvider();

            var processBlocker = serviceProvider.GetService<ProcessBlocker>();

            var notebookApp = new BlockTargetApplication
            {
                ProcessName = "notepad",
                IsEnabled = true,
                IsBlockedPermanently = true
            };

            processBlocker.BlockApplicationOnStart(notebookApp);

            Console.ReadKey();

            processBlocker.Dispose();
        }
    }
}