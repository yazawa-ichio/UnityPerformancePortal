using Plotly.Blazor;
using Plotly.Blazor.Traces;
using Plotly.Blazor.Traces.ScatterLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityPerformancePortal.Model;

namespace UnityPerformancePortal.Front
{
	public class AveragePlot
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
				Update();
			}
		}
		public bool Min
		{
			get => m_Min;
			set
			{
				m_Min = value;
				Update();
			}
		}
		public bool Max
		{
			get => m_Max;
			set
			{
				m_Max = value;
				Update();
			}
		}
		IEnumerable<string> m_Selects;
		bool m_Min;
		bool m_Max;
		Dictionary<string, List<AverageReport>> m_Dic = new();

		public void Set(ReportData[] datas)
		{
			m_Dic.Clear();
			foreach (var data in datas)
			{
				foreach (var average in data.Average)
				{
					if (!m_Dic.TryGetValue(average.Info.Name, out var list))
					{
						m_Dic[average.Info.Name] = list = new List<AverageReport>();
					}
					list.Add(average);
				}
			}
			Update();
		}

		void Update()
		{
			if (m_Selects == null)
			{
				return;
			}
			Data.Clear();
			foreach (var select in m_Selects)
			{
				if (!m_Dic.TryGetValue(select, out var list) || list.Count == 0)
				{
					continue;
				}
				var head = list[0];
				{
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
				if (Min)
				{
					var scatter = new Scatter
					{
						Name = head.Info.Name + "(min)",
						Mode = ModeFlag.Lines | ModeFlag.Markers,
						X = list.Select(x => (object)DateTimeOffset.FromUnixTimeMilliseconds(x.MinTimeAt.UnixMillseconds).LocalDateTime).ToList(),
						Y = list.Select(x => (object)x.MinValue).ToList(),
						HoverInfo = HoverInfoFlag.Y,
					};
					Data.Add(scatter);
				}
				if (Max)
				{
					var scatter = new Scatter
					{
						Name = head.Info.Name + "(max)",
						Mode = ModeFlag.Lines | ModeFlag.Markers,
						X = list.Select(x => (object)DateTimeOffset.FromUnixTimeMilliseconds(x.MaxTimeAt.UnixMillseconds).LocalDateTime).ToList(),
						Y = list.Select(x => (object)x.MaxValue).ToList(),
						HoverInfo = HoverInfoFlag.Y,
					};
					Data.Add(scatter);
				}
			}
			Chart?.Update();
		}

	}
}