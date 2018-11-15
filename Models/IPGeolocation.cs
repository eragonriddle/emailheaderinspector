using System;
using System.Net;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace EmailHeaderInspector.Models
{
	public class IPGeolocation : IPModel
	{
		private readonly IOptions<EHIConfig> _ehiConfig;
		private readonly ILogger _logger;

		public IPGeolocation(IOptions<EHIConfig> ehiConfig, ILogger<IPGeolocation> logger) : base(ehiConfig.Value.ip_city_db)
		{
			_ehiConfig = ehiConfig;
			_logger = logger;
		}

		public override void getData(IPGeolocationResult result)
		{
			IPAddress ipAddr = IPAddress.Parse(result.ip);
			_logger.LogInformation("Geolocation for ip = " + result.ip);
			Dictionary<string, object> data = dbReader.Find<Dictionary<string, object>>(ipAddr);
			if (data != null)
			{
				if (data.ContainsKey("city"))
				{
					Dictionary<string, object> city = (Dictionary<string, object>) data["city"];
					Dictionary<string, object> names = (Dictionary<string, object>)city["names"];
					if (names.ContainsKey("en"))
					{
						result.city = names["en"].ToString();
					}
				}
				if (data.ContainsKey("subdivisions"))
				{
					List<object> specifics = (List<object>)data["subdivisions"];
					Dictionary<string, object> names = (Dictionary<string, object>) specifics[0];
					names = (Dictionary<string, object>) names["names"];
					if (names.ContainsKey("en"))
					{
						result.state = names["en"].ToString();
					}
				}
				if (data.ContainsKey("country"))
				{
					Dictionary<string, object> country = (Dictionary<String, object>)data["country"];
					if (country.ContainsKey("iso_code"))
					{
						result.country = country["iso_code"].ToString();
					}
				}
			}
		}
	}
}
