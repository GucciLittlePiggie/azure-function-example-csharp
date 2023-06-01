using Microsoft.Extensions.Hosting;
using Function.Domain.Services;
using Function.Domain.Services.HttpClients;
using Function.Domain.Providers;
using Function.Domain.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Example.Function
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults(worker => worker.UseNewtonsoftJson())
                .ConfigureOpenApi()
                // This is a sample for adding configuration
                .ConfigureAppConfiguration((context, config) =>
                {
                    config
                        .AddJsonFile(Path.Combine(context.HostingEnvironment.ContentRootPath, "appsettings.json"), optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();
                })
                .ConfigureServices(s =>
                {
                    s.AddScoped<IFinhubDataMapper, FinhubDataMapper>();
                    s.AddScoped<IStockDataProvider, FinhubProvider>();
                    s.AddScoped<IHttpHelper, HttpHelper>();
                    s.AddHttpClient<FinhubHttpClient>();
                })
                .Build();

            host.Run();
        }
    }
}