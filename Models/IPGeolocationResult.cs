namespace EmailHeaderInspector.Models
{
	public class IPGeolocationResult
	{
		[ValidIPAddress]
		public string ip { get; set; }
		public string city { get; set; }
		public string state { get; set; }
		public string country { get; set; }
		public string asn { get; set; }
	}
}
