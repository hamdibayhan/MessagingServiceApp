using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MessagingServiceApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                 .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                 .UseSerilog((hostingContext, services, loggerConfiguration) => loggerConfiguration
                     .ReadFrom.Configuration(hostingContext.Configuration)
                     .Enrich.FromLogContext()
                     .WriteTo.Console(outputTemplate:
                         "[{Timestamp:yyyy-MM-ddTHH:mm:ss.fffffffK} {Level:u3}] {Message:lj}{NewLine}{Properties:j}{NewLine}{Exception}"));
    }
}
