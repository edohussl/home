using Home.Hussl.Data;
using Home.Hussl.Functions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Home.Hussl.Functions
{
	public class Startup : FunctionsStartup
	{
		public override void Configure(IFunctionsHostBuilder builder)
		{
			builder.Services.AddDbContext<HomeDbContext>(b =>
				b.UseCosmos(
					"https://homest01.documents.azure.com:443/",
					"J4xEEbWzfWgQZWULdbXaBh5OHfNe1wrneI8y8gb6wKC3kXHAkE7iXPZeJVosYyWd5PPTjdLcFW5IACDbOC7nGA==",
					"Home"));
		}
	}
}