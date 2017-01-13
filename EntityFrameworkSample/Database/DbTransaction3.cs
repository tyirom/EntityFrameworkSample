// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbTransaction3.cs" company="Grass Valley K.K.">
//   Copyright (C) 2017 Grass Valley K.K. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkSample.Model;

namespace EntityFrameworkSample.Database
{
	public class DbTransaction3
	{
		private readonly Task _worker;
		private readonly Queue<Action<Repository>> _queue;
		private readonly ManualResetEventSlim _queuedEvent;
		private readonly ManualResetEventSlim _idleEvent;
		private readonly CancellationTokenSource _cancellationTokenSource;

		public DbTransaction3()
		{
			this._queue = new Queue<Action<Repository>>();
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
//								action(this.Context);
							}

							if (!this._queue.Any())
							{
								this._queuedEvent.Reset();
								this._idleEvent.Set();
							}
						}

					} while (!cancellationToken.IsCancellationRequested);
				}
				finally
				{
					Trace.TraceInformation("End worker.");
				}
			}, TaskCreationOptions.LongRunning);
		}
	}
}