using Home.Hussl.Contracts;
using Home.Hussl.Data.Entities;

namespace Home.Hussl.Functions.Mappers
{
	internal static class LightMapper
	{
		internal static DbLight ToDb(this ApiLightBody apiLightBody)
		{
			return new DbLight
			{
				Address = apiLightBody.Address,
				Device = apiLightBody.Device,
				Name = apiLightBody.Name,
				Room = apiLightBody.Room,
				IsOn = apiLightBody.IsOn
			};
		}

		internal static ApiLight ToApi(this DbLight dbLight)
		{
			return new ApiLight
			{
				Id = dbLight.Id,
				Address = dbLight.Address,
				Device = dbLight.Device,
				Name = dbLight.Name,
				Room = dbLight.Room,
				IsOn = dbLight.IsOn
			};
		}
	}
}
