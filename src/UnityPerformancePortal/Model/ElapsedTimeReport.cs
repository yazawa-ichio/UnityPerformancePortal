namespace UnityPerformancePortal.Model
{
	public class ElapsedTimeReport
	{
		public string Category { get; set; }
		public string Name { get; set; }
		public string Lavel { get; set; }
		public TimeInfo StartAt { get; set; }
		public TimeInfo EndAt { get; set; }
	}
}