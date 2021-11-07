using System.Collections;
using UnityEngine;
using UnityPerformancePortal;

namespace UPP.Sample
{
	class SamplerMonitor : MonitorModule
	{
		Counter m_FrameCounter = new Counter("FrameCounter", DataUnit.Count);
		ElapsedTimeDebugSampler m_Sampler = new ElapsedTimeDebugSampler("SamplerMonitor");

		public override bool TryInit()
		{
			return true;
		}
		public override void Update()
		{
			m_FrameCounter.Inc();
		}

		public override void OnPostReport()
		{
			using (m_Sampler.Scope("FrameCounter.Sample"))
			{
				m_FrameCounter.Sample();
			}
		}
	}


	public class UPPSample : MonoBehaviour
	{
		[SerializeField]
		string m_Address = null;

		IEnumerator Start()
		{
			yield return Client.Initialize(new ReporterSettings
			{
				Config = new ReportConfig
				{
					UseDefaultMonitor = true,
					ReportUrl = m_Address,
					Interval = 5
				},
			}).Observe((task) =>
			{
				if (!task.IsSuccess)
				{
					Debug.LogException(task.Error);
				}
			});
			Client.Monitor.TryAdd<SamplerMonitor>();
		}

	}
}