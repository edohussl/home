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

				Task.Run(() => RfWrapper.initReceiver(2, TriggerLightToggledEvent));
				System.Console.WriteLine("Rf receiver initialized.");

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

				RfWrapper.deinitReceiver();
			}
		}

		private static async Task ProcessToggleLightMessage(ProcessMessageEventArgs args)
		{
			try
			{
				var message = args.Message.Body.ToObjectFromJson<ToggleLightMessage>();

				if (message.Device.HasValue)
				{
					System.Console.Write($"Sending unit signal for device {message.Device.Value} on address {message.Address} with code {message.TurnOn}.");
					RfWrapper.sendUnitSignal(3, message.Address, message.Device.Value, message.TurnOn);
				}
				else
				{
					System.Console.Write($"Sending group signal on address {message.Address} with code {message.TurnOn}.");
					RfWrapper.sendGroupSignal(3, message.Address, message.TurnOn);
				}

				await SendLightToggledEvent(message.Address, message.Device, message.TurnOn);

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

		private static void TriggerLightToggledEvent(RfWrapper.NewRemoteCode code)
		{
			System.Console.WriteLine($"New code ({code.switchType}) received on address {code.address} for device {code.unit} with group bit {code.groupBit}.");

			SendLightToggledEvent(
				(int)code.address,
				code.groupBit ? null : code.unit,
				code.switchType == RfWrapper.NewRemoteCode.SwitchType.on).Wait();
		}

		private static async Task SendLightToggledEvent(int address, int? device, bool turnedOn)
		{
			await using var client = new ServiceBusClient(HomeServiceBusConnectionString, _clientOptions);
			await using var sender = client.CreateSender("lighttoggled");

			var @event = new LightToggledEvent
			{
				Address = address,
				Device = device,
				TurnedOn = turnedOn
			};

			await sender.SendMessageAsync(new ServiceBusMessage(BinaryData.FromObjectAsJson(@event)));
		}
	}
}