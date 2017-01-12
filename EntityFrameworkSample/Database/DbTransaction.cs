// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbTransaction.cs" company="Grass Valley K.K.">
//   Copyright (C) 2017 Grass Valley K.K. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkSample.Database
{
	public class DbTransaction : IDisposable
	{
		private readonly Task _worker;
		private readonly Queue<Action> _queue;
		private readonly ManualResetEventSlim _queuedEvent;
		private readonly ManualResetEventSlim _idleEvent;
		private readonly CancellationTokenSource _cancellationTokenSource;

		public DbTransaction()
		{
			this._queue = new Queue<Action>();
			this._queuedEvent = new ManualResetEventSlim();
			this._idleEvent = new ManualResetEventSlim(true);
			this._cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = this._cancellationTokenSource.Token;

			this._worker = Task.Factory.StartNew(() =>
			{
				Trace.TraceInformation("Start worker.");
				try
				{
					do
					{
						try
						{
							cancellationToken.ThrowIfCancellationRequested();
							this._queuedEvent.Wait(cancellationToken);
						}
						catch (OperationCanceledException)
						{
							Trace.TraceInformation("Cancel worker.");
							return;
						}

						lock (this._queue)
						{
							if (this._queue.Any())
							{
								var action = this._queue.Dequeue();
								action();
							}

							if (!this._queue.Any())
							{
								this._queuedEvent.Reset();
								this._idleEvent.Set();
							}
						}

					} while (true);
				}
				finally
				{
					Trace.TraceInformation("End worker.");
				}
			}, TaskCreationOptions.LongRunning);
		}

		public void Enqueue(Action action)
		{
			lock (this._queue)
			{
				this._queue.Enqueue(action);
				this._queuedEvent.Set();
				this._idleEvent.Reset();
			}
		}

		public void Shutdown()
		{
			this._cancellationTokenSource.Cancel();
		}

		public void Join()
		{
			var cancellationToken = this._cancellationTokenSource.Token;
			this._idleEvent.Wait(cancellationToken);
		}

		#region IDisposable Support

		private bool _disposedValue = false; // 重複する呼び出しを検出するには

		protected virtual void Dispose(bool disposing)
		{
			Trace.TraceInformation("DbTransaction dispose {0}", disposing);
			if (!this._disposedValue)
			{
				if (disposing)
				{
					// TODO: マネージ状態を破棄します (マネージ オブジェクト)。
				}

				// TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
				// TODO: 大きなフィールドを null に設定します。
				this.Shutdown();
				this._worker.Wait();
				this._cancellationTokenSource.Dispose();
				lock (this._queue)
				{
					this._queuedEvent.Dispose();
				}
				this._worker.Dispose();

				this._disposedValue = true;
			}
		}

		// TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
		~DbTransaction()
		{
			// このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
			this.Dispose(false);
		}

		// このコードは、破棄可能なパターンを正しく実装できるように追加されました。
		public void Dispose()
		{
			// このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
			this.Dispose(true);
			// TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
