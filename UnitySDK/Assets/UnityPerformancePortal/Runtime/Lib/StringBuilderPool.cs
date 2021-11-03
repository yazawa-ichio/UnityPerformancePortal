using System.Collections.Concurrent;
using System.Text;
namespace UnityPerformancePortal
{

	internal static class StringBuilderPool
	{
		static ConcurrentQueue<StringBuilder> s_Pool = new ConcurrentQueue<StringBuilder>();

		public static StringBuilder Pop()
		{
			if (s_Pool.TryDequeue(out var ret))
			{
				return ret;
			}
			return new StringBuilder(256);
		}

		public static void Push(StringBuilder value)
		{
			value.Clear();
			s_Pool.Enqueue(value);
		}

	}

}