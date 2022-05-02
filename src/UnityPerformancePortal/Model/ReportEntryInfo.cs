using System;
using System.Collections.Generic;

namespace UnityPerformancePortal.Model
{
	public class ReportEntryInfo : IEquatable<ReportEntryInfo>
	{
		public string Name { get; set; }
		public string Unit { get; set; }

		public override bool Equals(object obj)
		{
			return Equals(obj as ReportEntryInfo);
		}

		public bool Equals(ReportEntryInfo other)
		{
			return other != null &&
				   Name == other.Name &&
				   Unit == other.Unit;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Name, Unit);
		}

		public static bool operator ==(ReportEntryInfo left, ReportEntryInfo right)
		{
			return EqualityComparer<ReportEntryInfo>.Default.Equals(left, right);
		}

		public static bool operator !=(ReportEntryInfo left, ReportEntryInfo right)
		{
			return !(left == right);
		}
	}
}