using UnityEngine;

namespace UnityPerformancePortal
{
	[AddComponentMenu("")]
	[DefaultExecutionOrder(-1)]
	internal class ClientImpl : MonoBehaviour
	{
		public static ClientImpl Create()
		{
			GameObject obj = new GameObject("UnityPerformancePortal");
			DontDestroyOnLoad(obj);
			return obj.AddComponent<ClientImpl>();
		}

		public readonly Monitor Monitor = new Monitor();

		public Repoter Repoter;

		void Awake()
		{
			TimeInfo.UpdateFrame();
		}

		void Update()
		{
			TimeInfo.UpdateFrame();
		}

		void LateUpdate()
		{
			Monitor.Update();
			Repoter?.DoUpdate();
		}

		void OnDestroy()
		{
			Client.Abort();
			Monitor.Dispose();
			Repoter.Dispose();
		}

		public AsyncTask Initialize(IReporterSettings config)
		{
			Repoter = config.CreateRepoter();
			Repoter.Setup(this);
			var task = new AsyncTask();
			Repoter.Init(task.Success, task.Fail);
			return task;
		}

		public void Post()
		{
			Repoter.Post();
		}

		public AsyncTask Stop()
		{
			return AsyncTask.Succeed;
		}
	}
}