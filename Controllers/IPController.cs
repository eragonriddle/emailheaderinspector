using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EmailHeaderInspector.Models;

namespace EmailHeaderInspector.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class IPController : ControllerBase
	{
		private readonly IPGeolocation _ipGeo;
		private readonly IPASN _ipASN;
		private readonly ILogger _logger;

		public IPController(IPGeolocation ipGeo, IPASN ipASN, ILogger<IPController> logger)
		{
			_ipGeo = ipGeo;
			_ipASN = ipASN;
			_logger = logger;
		}

		[HttpGet]
		public ActionResult Index()
		{
			return File("index.html", "text/html");
		}

		public IPGeolocationResult GetByIP([FromBody] [Bind("ip")] IPGeolocationResult ip)
		{
			_logger.LogInformation("Incoming ip = " + ip.ip);
			try
			{	_logger.LogInformation("Getting geo for " + ip.ip);
				_ipGeo.getData(ip);
				_logger.LogInformation("Getting asn for " + ip.ip);
				_ipASN.getData(ip);
				_logger.LogInformation("Completed for ip " + ip.ip);
			}
			catch (Exception e)
			{
				_logger.LogInformation("Exception occurred!!!!!!!!!!!!!");
				_logger.LogError(e.StackTrace);
			}
			return ip;
		}
	}
}
