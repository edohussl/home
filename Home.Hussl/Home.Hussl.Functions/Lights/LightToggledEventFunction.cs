using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Home.Hussl.Contracts;
using Home.Hussl.Data;
using Home.Hussl.Data.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.EntityFrameworkCore;

namespace Home.Hussl.Functions.Lights
{
	public class LightToggledEventFunction
	{
		private readonly HomeDbContext _context;

		public LightToggledEventFunction(HomeDbContext context)
		{
			_context = context;
		}

		[FunctionName("Lights_LightToggledEventFunction")]
		public async Task Run(
			[ServiceBusTrigger("lighttoggled", Connection = "ServiceBus_Home")] ServiceBusReceivedMessage message,
			ServiceBusMessageActions messageActions)
		{
			try
			{
				var @event = message.Body.ToObjectFromJson<LightToggledEvent>();

				var dbLight = await _context.Lights
					.TagWithCallSite()
					.FirstOrDefaultAsync(l => l.Address == @event.Address &&
					                          l.Device == @event.Device);

				if (dbLight == null)
				{
					dbLight = new DbLight
					{
						Address = @event.Address,
						Device = @event.Device,
						Name = "New light",
						Room = "Unknown",
					};

					await _context.AddAsync(dbLight);
				}

				dbLight.IsOn = @event.TurnedOn;
				await _context.SaveChangesAsync();

				await messageActions.CompleteMessageAsync(message);
			}
			catch (Exception ex)
			{
				await messageActions.DeadLetterMessageAsync(message, "Failed to process", ex.Message);
			}
		}
	}
}