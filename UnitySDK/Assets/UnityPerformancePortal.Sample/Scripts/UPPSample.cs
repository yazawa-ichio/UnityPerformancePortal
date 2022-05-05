using System.Collections;
using UnityEngine;
using UnityPerformancePortal;

namespace UPP.Sample
{

	public class UPPSample : MonoBehaviour
	{
		[SerializeField]
		string m_Address = null;
		bool m_Running = false;

		IEnumerator Run()
		{
			m_Running = true;
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

		void OnGUI()
		{
			float scale = (Screen.width > Screen.height) ? Screen.width / 720f : Screen.height / 720f;
			GUIUtility.ScaleAroundPivot(new Vector2(scale, scale), Vector2.zero);
			if (!m_Running)
			{
				InitView();
			}
			if (Client.IsValid)
			{
				MainView();
			}
		}

		void InitView()
		{
			GUILayout.Label("Address");
			m_Address = GUILayout.TextField(m_Address);
			if (GUILayout.Button("Run"))
			{
				StartCoroutine(Run());
			}
		}

		void MainView()
		{
			GUILayout.Label("ReporterId");
			GUILayout.TextField(Client.ReporterId);
			GUILayout.Label("SessionId");
			GUILayout.TextField(Client.SessionId);

			var target = Application.targetFrameRate == 30 ? 90 : 30;
			if (GUILayout.Button($"Change FPS {target}"))
			{
				Application.targetFrameRate = target;
			}

			GUILayout.Space(100);
			if (GUILayout.Button("Reset"))
			{
				Client.Stop().Observe(x =>
				{
					m_Running = false;
				});
			}
			if (GUILayout.Button("Clear User"))
			{
				PlayerPrefs.DeleteAll();
				Client.Stop().Observe(x =>
				{
					m_Running = false;
				});
			}
		}

	}
}