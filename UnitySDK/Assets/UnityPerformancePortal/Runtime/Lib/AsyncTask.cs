using System;
using System.Threading.Tasks;

namespace UnityPerformancePortal
{
	public class AsyncTask : UnityEngine.CustomYieldInstruction
	{
		public readonly static AsyncTask Succeed = new AsyncTask(success: true);

		public readonly static AsyncTask Failed = new AsyncTask(success: false);

		public static AsyncTask Run(Task task)
		{
			return Run(() => task);
		}

		public static AsyncTask Run(Func<Task> task)
		{
			var ret = new AsyncTask();
			ret.RunImpl(task);
			return ret;
		}

		public static AsyncTask<T> Run<T>(Task<T> task)
		{
			return Run(() => task);
		}

		public static AsyncTask<T> Run<T>(Func<Task<T>> task)
		{
			var ret = new AsyncTask<T>();
			ret.RunImpl(task);
			return ret;
		}

		public override bool keepWaiting => !IsDone;

		public bool IsDone { get; private set; }

		public bool IsSuccess { get; private set; }

		public Exception Error { get; private set; }

		public event Action<AsyncTask> OnComplete
		{
			add
			{
				if (IsDone)
				{
					value?.Invoke(this);
				}
				else
				{
					m_OnComplete += value;
				}
			}
			remove
			{
				m_OnComplete -= value;
			}
		}

		Action<AsyncTask> m_OnComplete;
		TaskCompletionSource<bool> m_Future = new TaskCompletionSource<bool>();

		public AsyncTask() { }

		internal AsyncTask(bool success)
		{
			if (success)
			{
				Success();
			}
			else
			{
				Fail(new Exception("Failed"));
			}
		}

		public AsyncTask Observe(Action<AsyncTask> action)
		{
			OnComplete += action;
			return this;
		}

		async void RunImpl(Func<Task> task)
		{
			try
			{
				await task();
				Success();
			}
			catch (Exception error)
			{
				Fail(error);
			}
		}

		internal void Success()
		{
			if (IsDone) return;
			IsDone = true;
			IsSuccess = true;
			var action = m_OnComplete;
			m_OnComplete = null;
			action?.Invoke(this);
			m_Future.TrySetResult(true);
		}

		internal void Fail(Exception error)
		{
			if (IsDone) return;
			IsDone = true;
			IsSuccess = false;
			Error = error;
			var action = m_OnComplete;
			m_OnComplete = null;
			action?.Invoke(this);
			m_Future.TrySetException(error);
		}

		public Task ToTask()
		{
			return m_Future.Task;
		}
	}

	public class AsyncTask<T> : UnityEngine.CustomYieldInstruction
	{

		public override bool keepWaiting => false;

		public bool IsDone { get; private set; }

		public bool IsSuccess { get; private set; }

		public T Value { get; private set; }

		public Exception Error { get; private set; }

		public event Action<AsyncTask<T>> OnComplete
		{
			add
			{
				if (IsDone)
				{
					value?.Invoke(this);
				}
				else
				{
					m_OnComplete += value;
				}
			}
			remove
			{
				m_OnComplete -= value;
			}
		}

		Action<AsyncTask<T>> m_OnComplete;

		TaskCompletionSource<T> m_Future = new TaskCompletionSource<T>();

		public AsyncTask() { }

		public AsyncTask(T value)
		{
			Success(value);
		}

		public AsyncTask(Exception error)
		{
			Fail(error);
		}


		internal async void RunImpl(Func<Task<T>> task)
		{
			try
			{
				Success(await task());
			}
			catch (Exception error)
			{
				Fail(error);
			}
		}

		public AsyncTask<T> Observe(Action<AsyncTask<T>> action)
		{
			OnComplete += action;
			return this;
		}

		internal void Success(T value)
		{
			if (IsDone) return;
			IsDone = true;
			Value = value;
			IsSuccess = true;
			var action = m_OnComplete;
			m_OnComplete = null;
			action?.Invoke(this);
			m_Future.TrySetResult(value);
		}

		internal void Fail(Exception error)
		{
			if (IsDone) return;
			IsDone = true;
			IsSuccess = false;
			Error = error;
			var action = m_OnComplete;
			m_OnComplete = null;
			action?.Invoke(this);
			m_Future.TrySetException(error);
		}

		public Task<T> ToTask()
		{
			return m_Future.Task;
		}

	}


}