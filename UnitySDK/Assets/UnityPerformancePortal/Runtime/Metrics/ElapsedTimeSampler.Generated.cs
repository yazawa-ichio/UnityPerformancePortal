#if UPP_SAMPLER || UPP_SAMPLER_TRACE_OR_HIGHER
#define UPP_SAMPLER_TRACE
#endif
#if UPP_SAMPLER || UPP_SAMPLER_TRACE_OR_HIGHER || UPP_SAMPLER_DEBUG_OR_HIGHER
#define UPP_SAMPLER_DEBUG
#endif
#if UPP_SAMPLER || UPP_SAMPLER_TRACE_OR_HIGHER || UPP_SAMPLER_DEBUG_OR_HIGHER || UPP_SAMPLER_INFO_OR_HIGHER
#define UPP_SAMPLER_INFO
#endif
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using UnityPerformancePortal.Report;

namespace UnityPerformancePortal
{

	public class ElapsedTimeTraceSampler : IElapsedTimeSampler
	{

		public readonly string Category;

		string IElapsedTimeSampler.Category => Category;

		public bool Enabled
		{
			get => m_Enabled;
			set => m_Enabled = value;
		}

		bool m_Enabled;

#if UPP_SAMPLER_TRACE
		ConcurrentDictionary<string, AutoScope> m_Scope = new ConcurrentDictionary<string, AutoScope>();
#endif

		public ElapsedTimeTraceSampler(string category)
		{
			Category = category;
			m_Enabled = true;
			ElapsedTime.Add(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AutoScope Scope(string name)
		{
#if UPP_SAMPLER_TRACE
			if (Client.IsValid && m_Enabled)
			{
				return new AutoScope(Category, name, TimeInfo.Now);
			}
#endif
			return default;
		}

		[System.Diagnostics.Conditional("UPP_SAMPLER_TRACE")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Begin(string name)
		{
			if (!Client.IsValid || !m_Enabled)
			{
				return;
			}
			m_Scope[name] = Scope(name);
		}

		[System.Diagnostics.Conditional("UPP_SAMPLER_TRACE")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void End(string name)
		{
			if (!Client.IsValid || !m_Enabled)
			{
				return;
			}
			if (m_Scope.TryRemove(name, out var scope))
			{
				scope.Dispose();
			}
		}

		public readonly struct AutoScope : IDisposable
		{
#if UPP_SAMPLER_TRACE
			public readonly string Category;
			public readonly string Name;
			public readonly TimeInfo TimeAt;

			internal AutoScope(string category, string name, TimeInfo timeAt)
			{
				Category = category;
				Name = name;
				TimeAt = timeAt;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose()
			{
				if (!Client.IsValid || string.IsNullOrEmpty(Name)) return;
				Client.Impl.Repoter.Add(new ElapsedTimeReport(Category, Name, "TRACE", TimeAt, TimeInfo.Now));
			}
#else
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose() { }
#endif
		}
	}

	public class ElapsedTimeDebugSampler : IElapsedTimeSampler
	{

		public readonly string Category;

		string IElapsedTimeSampler.Category => Category;

		public bool Enabled
		{
			get => m_Enabled;
			set => m_Enabled = value;
		}

		bool m_Enabled;

#if UPP_SAMPLER_DEBUG
		ConcurrentDictionary<string, AutoScope> m_Scope = new ConcurrentDictionary<string, AutoScope>();
#endif

		public ElapsedTimeDebugSampler(string category)
		{
			Category = category;
			m_Enabled = true;
			ElapsedTime.Add(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AutoScope Scope(string name)
		{
#if UPP_SAMPLER_DEBUG
			if (Client.IsValid && m_Enabled)
			{
				return new AutoScope(Category, name, TimeInfo.Now);
			}
#endif
			return default;
		}

		[System.Diagnostics.Conditional("UPP_SAMPLER_DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Begin(string name)
		{
			if (!Client.IsValid || !m_Enabled)
			{
				return;
			}
			m_Scope[name] = Scope(name);
		}

		[System.Diagnostics.Conditional("UPP_SAMPLER_DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void End(string name)
		{
			if (!Client.IsValid || !m_Enabled)
			{
				return;
			}
			if (m_Scope.TryRemove(name, out var scope))
			{
				scope.Dispose();
			}
		}

		public readonly struct AutoScope : IDisposable
		{
#if UPP_SAMPLER_DEBUG
			public readonly string Category;
			public readonly string Name;
			public readonly TimeInfo TimeAt;

			internal AutoScope(string category, string name, TimeInfo timeAt)
			{
				Category = category;
				Name = name;
				TimeAt = timeAt;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose()
			{
				if (!Client.IsValid || string.IsNullOrEmpty(Name)) return;
				Client.Impl.Repoter.Add(new ElapsedTimeReport(Category, Name, "DEBUG", TimeAt, TimeInfo.Now));
			}
#else
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose() { }
#endif
		}
	}

	public class ElapsedTimeInfoSampler : IElapsedTimeSampler
	{

		public readonly string Category;

		string IElapsedTimeSampler.Category => Category;

		public bool Enabled
		{
			get => m_Enabled;
			set => m_Enabled = value;
		}

		bool m_Enabled;

#if UPP_SAMPLER_INFO
		ConcurrentDictionary<string, AutoScope> m_Scope = new ConcurrentDictionary<string, AutoScope>();
#endif

		public ElapsedTimeInfoSampler(string category)
		{
			Category = category;
			m_Enabled = true;
			ElapsedTime.Add(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AutoScope Scope(string name)
		{
#if UPP_SAMPLER_INFO
			if (Client.IsValid && m_Enabled)
			{
				return new AutoScope(Category, name, TimeInfo.Now);
			}
#endif
			return default;
		}

		[System.Diagnostics.Conditional("UPP_SAMPLER_INFO")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Begin(string name)
		{
			if (!Client.IsValid || !m_Enabled)
			{
				return;
			}
			m_Scope[name] = Scope(name);
		}

		[System.Diagnostics.Conditional("UPP_SAMPLER_INFO")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void End(string name)
		{
			if (!Client.IsValid || !m_Enabled)
			{
				return;
			}
			if (m_Scope.TryRemove(name, out var scope))
			{
				scope.Dispose();
			}
		}

		public readonly struct AutoScope : IDisposable
		{
#if UPP_SAMPLER_INFO
			public readonly string Category;
			public readonly string Name;
			public readonly TimeInfo TimeAt;

			internal AutoScope(string category, string name, TimeInfo timeAt)
			{
				Category = category;
				Name = name;
				TimeAt = timeAt;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose()
			{
				if (!Client.IsValid || string.IsNullOrEmpty(Name)) return;
				Client.Impl.Repoter.Add(new ElapsedTimeReport(Category, Name, "INFO", TimeAt, TimeInfo.Now));
			}
#else
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose() { }
#endif
		}
	}



}