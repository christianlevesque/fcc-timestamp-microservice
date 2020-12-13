using System;
using System.Text.Json.Serialization;

namespace timestamp
{
	public class Timestamp
	{
		[JsonPropertyName("unix")]
		public long Unix { get; }
		
		[JsonPropertyName("utc")]
		public string Utc { get; set; }

		public Timestamp(DateTime date)
		{
			Unix = (long) (date - DateTime.UnixEpoch).TotalMilliseconds;
			Utc = date.ToString("R");
		}

		public Timestamp() : this(DateTime.Now) { }
	}
}