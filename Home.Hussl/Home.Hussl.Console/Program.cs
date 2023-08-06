using Azure.Messaging.ServiceBus;
using Home.Hussl.Pi;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Home.Hussl.Console
{
	internal class Program
	{
		private const string HomeServiceBusConnectionString = "Endpoint=sb://homesb01.servicebus.windows.net/;SharedAccessKeyName=ListenSendAccessKey;SharedAccessKey=sbHDrKEqox2ndt6MeKoakegNy5oBdOUY9+ASbJuPgDA=";

		static async Task Main(string[] args)
		{
			var result = RfWrapper.setupWiringPi();

			if (result == -1)
			{
				System.Console.WriteLine("WiringPi setup failed.");
				return;
			}

			var builder = new HostBuilder()
				.ConfigureAppConfiguration((hostingContext, config) =>
				{
					config.AddEnvironmentVariables();
					config.AddCommandLine(args);
				})
				.ConfigureServices((hostContext, services) =>
				{
					services.AddOptions();

					services.AddAzureClients(cfg =>
					{
						cfg.AddServiceBusClient(HomeServiceBusConnectionString)
							.WithName("HomeSb01")
							.ConfigureOptions(o => o.TransportType = ServiceBusTransportType.AmqpWebSockets);
					});

					services.AddHostedService<RfReceiver>();
					services.AddHostedService<RfSender>();

					services.AddSingleton<RfListener>();
				})
				.ConfigureLogging((hostingContext, logging) =>
				{
					logging.AddConsole();
				});

			await builder.RunConsoleAsync();
		}
	}
}