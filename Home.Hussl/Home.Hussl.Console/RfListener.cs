using Home.Hussl.Pi;

namespace Home.Hussl.Console
{
	internal class RfListener
	{
		private bool _enabled;

		public void Enable()
		{
			RfWrapper.enableReceiver();
			_enabled = true;
		}

		public void Disable()
		{
			RfWrapper.disableReceiver();
			_enabled = false;
		}

		public bool IsEnabled()
		{
			return _enabled;
		}

		public async Task StartAsync(
			RfWrapper.NewRemoteReceiverCallBack triggerLightToggledEvent,
			CancellationToken cancellationToken)
		{
#pragma warning disable CS4014 // listener should run on the background
			Task.Run(() => RfWrapper.initReceiver(2, triggerLightToggledEvent), cancellationToken);
#pragma warning restore CS4014

			await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);

			Disable();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			RfWrapper.deinitReceiver();
			return Task.CompletedTask;
		}
	}
}
