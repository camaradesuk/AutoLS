using System;
using Microsoft.Extensions.Hosting;
using SyRF.WebHostConfig.Common.Extensions;

namespace SyRF.BiorxivParser.Endpoint
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.Out.WriteLineAsync("Biorxiv Search Service is starting...");
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureSyrfHostSettings(args)
                .ConfigureWebHostDefaults(webHostBuilder => webHostBuilder
                    .ConfigureSyrfWebHost<Startup>(args)
                );
    }
}