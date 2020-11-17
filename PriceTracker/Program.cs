using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace PriceTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("https://localhost:3001;http://localhost:3000").UseStartup<Startup>();
                });
    }
}
