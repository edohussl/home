using Azure.Messaging.ServiceBus;
using Home.Hussl.Contracts;
using Home.Hussl.Pi;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Home.Hussl.Console
{
	internal class RfReceiver : IAsyncDisposable, IHostedService
	{
		private readonly RfListener _listener;
		private readonly ILogger<RfReceiver> _logger;
		private readonly ServiceBusClient _client;
		private readonly ServiceBusSender _sender;

		public RfReceiver(
			IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
			RfListener listener,
			ILogger<RfReceiver> logger)
		{
			_listener = listener;
			_logger = logger;
			_client = serviceBusClientFactory.CreateClient("HomeSb01");
			_sender =  _client.CreateSender("lighttoggled");
		}

		private void TriggerLightToggledEvent(RfWrapper.NewRemoteCode code)
		{
			_logger.LogInformation($"New code ({code.switchType}) received on address {code.address} for device {code.unit} with group bit {code.groupBit}.");

			SendLightToggledEvent(
				code.address,
				code.groupBit ? null : code.unit,
				code.switchType == RfWrapper.NewRemoteCode.SwitchType.on).Wait();
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

		public async ValueTask DisposeAsync()
		{
			await _sender.DisposeAsync();
			await _client.DisposeAsync();
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			await _listener.StartAsync(TriggerLightToggledEvent, cancellationToken);
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			await _listener.StopAsync(cancellationToken);
		}
	}
}
