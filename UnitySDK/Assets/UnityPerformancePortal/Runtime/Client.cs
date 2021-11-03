using System.Runtime.CompilerServices;

namespace UnityPerformancePortal
{

	public static class Client
	{
		internal static ClientImpl Impl;

		public static bool IsValid { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }

		public static Monitor Monitor => Impl.Monitor;

		public static AsyncTask Initialize(ReporterSettings settings)
		{
			if (IsValid)
			{
				throw new System.InvalidOperationException("already Initialized");
			}
			IsValid = true;
			Impl = ClientImpl.Create();
			return Impl.Initialize(settings);
		}

		public static AsyncTask Stop()
		{
			if (!IsValid)
			{
				var task = new AsyncTask();
				task.Success();
				return task;
			}
			var impl = Impl;
			IsValid = false;
			return impl.Stop().Observe((_) =>
			{
				if (Impl == impl)
				{
					Impl = null;
				}
				else if (Impl == null)
				{
					Impl = null;
				}
			});
		}

		internal static void Abort()
		{
			Impl = null;
			IsValid = false;
		}

		public static void PostImmediate()
		{
			Impl.Post();
		}

	}


}