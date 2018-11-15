using System.Net;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace EmailHeaderInspector.Models
{
	public class IPASN : IPModel
	{
		private readonly IOptions<EHIConfig> _ehiConfig;
		private readonly ILogger _logger;

		public IPASN(IOptions<EHIConfig> ehiConfig, ILogger<IPASN> logger) : base(ehiConfig.Value.ip_asn_db)
		{
			_ehiConfig = ehiConfig;
			_logger = logger;
		}

		public override void getData(IPGeolocationResult result)
		{
			IPAddress ipAddr = IPAddress.Parse(result.ip);
			_logger.LogInformation("ASN check for ip = " + result.ip);
			var data = dbReader.Find<Dictionary<string, object>>(ipAddr);
			if (data != null)
			{
				if (data.ContainsKey("autonomous_system_organization"))
				{
					result.asn = data["autonomous_system_organization"].ToString();
				}
			}
		}
	}
}
