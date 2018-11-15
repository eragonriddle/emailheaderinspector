using MaxMind.Db;

namespace EmailHeaderInspector.Models
{
	public abstract class IPModel : IIPModel
	{
		private readonly Reader reader;

		public IPModel(string dbName)
		{
			reader = new Reader(dbName);
		}

		public Reader dbReader
		{
			get
			{
				return reader;
			}
		}

		public abstract void getData(IPGeolocationResult result);
	}
}
