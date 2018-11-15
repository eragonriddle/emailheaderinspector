using System.Net;
using System.ComponentModel.DataAnnotations;

namespace EmailHeaderInspector.Models
{
	public class ValidIPAddress : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			IPGeolocationResult ipGeo = (IPGeolocationResult) validationContext.ObjectInstance;
			IPAddress ipAddr;
			if (IPAddress.TryParse(ipGeo.ip, out ipAddr))
			{
				return ValidationResult.Success;
			}
			return new ValidationResult("Invalid IP Address");
		}
	}
}
