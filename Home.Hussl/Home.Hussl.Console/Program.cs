using Azure.Messaging.ServiceBus;
using Home.Hussl.Pi;
using Home.Hussl.Contracts;

namespace Home.Hussl.Console
{
	internal class Program
	{
		private const string HomeServiceBusConnectionString = "Endpoint=sb://homesb01.servicebus.windows.net/;SharedAccessKeyName=ListenSendAccessKey;SharedAccessKey=sbHDrKEqox2ndt6MeKoakegNy5oBdOUY9+ASbJuPgDA=";
		private static ServiceBusClient _client = null!;
		private static ServiceBusProcessor _processor = null!;
		private static ServiceBusClientOptions _clientOptions = null!;

		static async Task Main(string[] args)
		{
			var result = RfWrapper.setupWiringPi();

			if (result == -1)
			{
				System.Console.WriteLine("WiringPi setup failed.");
				return;
			}

			_clientOptions = new ServiceBusClientOptions
			{
				TransportType = ServiceBusTransportType.AmqpWebSockets
			};
			_client = new ServiceBusClient(HomeServiceBusConnectionString, _clientOptions);
			_processor = _client.CreateProcessor("togglelight", new ServiceBusProcessorOptions());

			try
			{
				// add handler to process messages
				_processor.ProcessMessageAsync += ProcessToggleLightMessage;

				// add handler to process any errors
				_processor.ProcessErrorAsync += ProcessToggleLightMessageError;

				// start processing 
				await _processor.StartProcessingAsync();

				System.Console.WriteLine("Wait for a minute and then press any key to end the processing");
				System.Console.ReadKey();

				// stop processing 
				System.Console.WriteLine("\nStopping the receiver...");
				await _processor.StopProcessingAsync();
				System.Console.WriteLine("Stopped receiving messages");
			}
			finally
			{
				// Calling DisposeAsync on client types is required to ensure that network
				// resources and other unmanaged objects are properly cleaned up.
				await _processor.DisposeAsync();
				await _client.DisposeAsync();
			}
		}

		private static async Task ProcessToggleLightMessage(ProcessMessageEventArgs args)
		{
			try
			{
				var message = args.Message.Body.ToObjectFromJson<ToggleLightMessage>();

				if (message.Device.HasValue)
				{
					RfWrapper.sendUnitSignal(3, message.Address, message.Device.Value, message.TurnOn);
				}
				else
				{
					RfWrapper.sendGroupSignal(3, message.Address, message.TurnOn);
				}

				await using var client = new ServiceBusClient(HomeServiceBusConnectionString, _clientOptions);
				await using var sender = client.CreateSender("lighttoggled");

				var @event = new LightToggledEvent
				{
					Address = message.Address,
					Device = message.Device,
					TurnedOn = message.TurnOn
				};

				await sender.SendMessageAsync(new ServiceBusMessage(BinaryData.FromObjectAsJson(@event)));

				await args.CompleteMessageAsync(args.Message);
			}
			catch (Exception ex)
			{
				await args.DeadLetterMessageAsync(args.Message, "Exception during processing", ex.Message);
			}
		}

		private static Task ProcessToggleLightMessageError(ProcessErrorEventArgs args)
		{
			System.Console.WriteLine(args.Exception.ToString());
			return Task.CompletedTask;
		}
	}
}