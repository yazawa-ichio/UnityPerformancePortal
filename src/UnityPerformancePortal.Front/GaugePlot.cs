using Plotly.Blazor;
using Plotly.Blazor.Traces;
using Plotly.Blazor.Traces.ScatterLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityPerformancePortal.Model;

namespace UnityPerformancePortal.Front
{
	public class GaugePlot
	{
		public PlotlyChart Chart;
		public Config Config = new Config();
		public Layout Layout = new Layout();
		public IList<ITrace> Data = new List<ITrace>();
		public IEnumerable<string> Selects
		{
			get => m_Selects;
			set
			{
				m_Selects = value;
				Update(value);
			}
		}

		IEnumerable<string> m_Selects;
		Dictionary<string, List<GaugeReport>> m_Dic = new();

		public void Set(ReportData[] datas)
		{
			m_Dic.Clear();
			foreach (var data in datas)
			{
				foreach (var gauge in data.Gauge)
				{
					if (!m_Dic.TryGetValue(gauge.Info.Name, out var list))
					{
						m_Dic[gauge.Info.Name] = list = new List<GaugeReport>();
					}
					list.Add(gauge);
				}
			}
			if (m_Selects != null)
			{
				Update(m_Selects);
			}
		}

		void Update(IEnumerable<string> selects)
		{
			Data.Clear();
			foreach (var select in selects)
			{
				if (!m_Dic.TryGetValue(select, out var list) || list.Count == 0)
				{
					continue;
				}
				var head = list[0];
				var scatter = new Scatter
				{
					Name = head.Info.Name,
					Mode = ModeFlag.Lines | ModeFlag.Markers,
					X = list.Select(x => (object)DateTimeOffset.FromUnixTimeMilliseconds(x.TimeAt.UnixMillseconds).LocalDateTime).ToList(),
					Y = list.Select(x => (object)x.Value).ToList(),
					HoverInfo = HoverInfoFlag.Y,
				};
				Data.Add(scatter);
			}
			Chart?.Update();
		}

	}
}