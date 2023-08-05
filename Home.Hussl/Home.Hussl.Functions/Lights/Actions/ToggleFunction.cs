using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Home.Hussl.Contracts;
using Home.Hussl.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Home.Hussl.Functions.Lights.Actions
{
	public class ToggleFunction
	{
		private const string HomeServiceBusConnectionString = "Endpoint=sb://homesb01.servicebus.windows.net/;SharedAccessKeyName=ListenSendAccessKey;SharedAccessKey=sbHDrKEqox2ndt6MeKoakegNy5oBdOUY9+ASbJuPgDA=";

		private readonly HomeDbContext _context;

		public ToggleFunction(HomeDbContext context)
		{
			_context = context;
		}

		[FunctionName("Light_ToggleFunction")]
		[OpenApiOperation("action-light-toggle", "lights")]
		//[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
		[OpenApiRequestBody("application/json", typeof(ApiLightState))]
		[OpenApiParameter("id", Type = typeof(Guid), Required = true)]
		[OpenApiResponseWithoutBody(HttpStatusCode.Accepted)]
		public async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "PUT", Route = "lights/{id}/actions/toggle")] HttpRequest req,
			Guid id)
		{
			string requestBody; 
			using (var streamReader = new StreamReader(req.Body))
			{
				requestBody = await streamReader.ReadToEndAsync();
			}
			var lightState = JsonConvert.DeserializeObject<ApiLightState>(requestBody);

			var dbLight = await _context.Lights.TagWithCallSite().FirstOrDefaultAsync(l => l.Id == id);
			if (dbLight == null)
			{
				return new NotFoundResult();
			}

			var message = new ToggleLightMessage
			{
				Address = dbLight.Address,
				Device = dbLight.Device,
				TurnOn = lightState.TurnOn
			};

			var clientOptions = new ServiceBusClientOptions
			{
				TransportType = ServiceBusTransportType.AmqpWebSockets
			};

			await using var client = new ServiceBusClient(HomeServiceBusConnectionString, clientOptions);
			await using var sender = client.CreateSender("togglelight");

			await sender.SendMessageAsync(new ServiceBusMessage(BinaryData.FromObjectAsJson(message)));

			return new AcceptedResult();
		}
	}
}