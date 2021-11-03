namespace UnityPerformancePortal
{
	public enum DataUnit
	{
		None,
		Count,
		Bytes,
		Second,
		Millisecond,
		Nanosecond,
		Percentage,
		Rate,
	}

	public static class DataUnitExtension
	{
		public static string GetUnitId(this DataUnit self)
		{
			switch (self)
			{
				case DataUnit.Count:
					return "[COUNT]";
				case DataUnit.Bytes:
					return "[BYTES]";
				case DataUnit.Second:
					return "[SECOND]";
				case DataUnit.Millisecond:
					return "[MILISECOND]";
				case DataUnit.Nanosecond:
					return "[NANOSECOND]";
				case DataUnit.Percentage:
					return "[PERCENTAGE]";
				case DataUnit.Rate:
					return "[RATE]";
			}
			return "";
		}
	}

}