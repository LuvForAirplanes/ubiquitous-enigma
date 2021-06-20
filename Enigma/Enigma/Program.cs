using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
namespace Enigma
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args);
            var isService = !(Debugger.IsAttached || args.Contains("--console") || args.Contains("-c"));

            if (isService && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                builder.UseWindowsService();
            else
                builder.Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
