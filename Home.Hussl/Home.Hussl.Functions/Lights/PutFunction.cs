using System.IO;
using System.Net;
using System.Threading.Tasks;
using Home.Hussl.Contracts;
using Home.Hussl.Data;
using Home.Hussl.Functions.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;

namespace Home.Hussl.Functions.Lights
{
	public class PutFunction
	{
		private readonly HomeDbContext _context;

		public PutFunction(HomeDbContext context)
		{
			_context = context;
		}

		[FunctionName("Lights_PutFunction")]
		[OpenApiOperation("put-lights", "lights")]
		//[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
		[OpenApiRequestBody("application/json", typeof(ApiLightBody))]
		[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(ApiLight))]
		public async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "PUT", Route = "lights")] HttpRequest req)
		{
			string requestBody; 
			using (var streamReader = new StreamReader(req.Body))
			{
				requestBody = await streamReader.ReadToEndAsync();
			}
			var light = JsonConvert.DeserializeObject<ApiLightBody>(requestBody);

			var dbLight = light.ToDb();
			await _context.Lights.AddAsync(dbLight, req.HttpContext.RequestAborted);
			await _context.SaveChangesAsync(req.HttpContext.RequestAborted);

			return new OkObjectResult(dbLight.ToApi());
		}
	}
}