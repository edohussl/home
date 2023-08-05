using System.Linq;
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
using Microsoft.EntityFrameworkCore;

namespace Home.Hussl.Functions.Lights
{
	public class GetFunction
	{
		private readonly HomeDbContext _context;

		public GetFunction(HomeDbContext context)
		{
			_context = context;
		}

		[FunctionName("Lights_GetFunction")]
		[OpenApiOperation("get-lights", "lights")]
		//[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
		[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(ApiLight[]))]
		public async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "GET", Route = "lights")] HttpRequest req)
		{
			var lights = await _context.Lights.TagWithCallSite().ToArrayAsync(req.HttpContext.RequestAborted);

			return new OkObjectResult(lights.Select(l => l.ToApi()));
		}
	}
}