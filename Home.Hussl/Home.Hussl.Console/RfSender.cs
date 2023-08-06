using Azure.Messaging.ServiceBus;
using Home.Hussl.Contracts;
using Home.Hussl.Pi;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Home.Hussl.Console
{
	internal class RfSender : IAsyncDisposable, IHostedService
	{
		private readonly RfListener _listener;
		private readonly ILogger<RfSender> _logger;
		private readonly ServiceBusClient _client;
		private readonly ServiceBusSender _sender;
		private readonly ServiceBusProcessor _processor;

		public RfSender(
			IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
			RfListener listener,
			ILogger<RfSender> logger)
		{
			_listener = listener;
			_logger = logger;
			_client = serviceBusClientFactory.CreateClient("HomeSb01");
			_sender = _client.CreateSender("lighttoggled");
			_processor =  _client.CreateProcessor("togglelight");

			_processor.ProcessMessageAsync += ProcessToggleLightMessage;
			_processor.ProcessErrorAsync += ProcessToggleLightMessageError;
		}

		private async Task ProcessToggleLightMessage(ProcessMessageEventArgs args)
		{
			try
			{
				var message = args.Message.Body.ToObjectFromJson<ToggleLightMessage>();

				ExecuteWithDisabledListener(() =>
				{
					if (message.Device.HasValue)
					{
						_logger.LogInformation($"Sending unit signal for device {message.Device.Value} on address {message.Address} with code {message.TurnOn}.");
						RfWrapper.sendUnitSignal(3, message.Address, message.Device.Value, message.TurnOn);
					}
					else
					{
						_logger.LogInformation($"Sending group signal on address {message.Address} with code {message.TurnOn}.");
						RfWrapper.sendGroupSignal(3, message.Address, message.TurnOn);
					}
				});

				await SendLightToggledEvent(message.Address, message.Device, message.TurnOn);

				await args.CompleteMessageAsync(args.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception during processing");
				await args.DeadLetterMessageAsync(args.Message, "Exception during processing", ex.Message);
			}
		}

		private void ExecuteWithDisabledListener(Action action)
		{
			if (!_listener.IsEnabled())
			{
				action();
				return;
			}

			_logger.LogDebug("Disabling receiver...");
			_listener.Disable();

			try
			{
				action();
			}
			finally
			{
				_logger.LogDebug("Enabling receiver...");
				_listener.Enable();
			}
		}

		private Task ProcessToggleLightMessageError(ProcessErrorEventArgs args)
		{
			_logger.LogError(args.Exception, "Error reading from queue");
			return Task.CompletedTask;
		}

		private async Task SendLightToggledEvent(long address, short? device, bool turnedOn)
		{
			var @event = new LightToggledEvent
			{
				Address = address,
				Device = device,
				TurnedOn = turnedOn
			};

			await _sender.SendMessageAsync(new ServiceBusMessage(BinaryData.FromObjectAsJson(@event)));
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			await _processor.StartProcessingAsync(cancellationToken);
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			await _processor.StopProcessingAsync(cancellationToken);
		}

		public async ValueTask DisposeAsync()
		{
			await _processor.DisposeAsync();
			await _sender.DisposeAsync();
			await _client.DisposeAsync();
		}
	}
}
