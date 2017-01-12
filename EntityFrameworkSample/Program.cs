// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaSample.cs" company="Grass Valley K.K.">
//   Copyright (C) 2017 Grass Valley K.K. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkSample.Database;
using EntityFrameworkSample.Model;

namespace EntityFrameworkSample
{
	using System.IO;
	using System.Collections.Generic;

	class Program
	{
		static void Main(string[] args)
		{
			var di = new DirectoryInfo(@"E:\data\SAIPAN");
			var timer = new Stopwatch();
			var results = new List<Tuple<string, long>>();

//			ReadOnSingleThread(timer, di, results);
//			ReadOnMultiThread(timer, di, results);

			ReadWriteOnSingleThread(timer, di, results);
			ReadWriteOnMultiThread(timer, di, results);
			ReadOnMultThreadAndWriteOnSingleThread(timer, di, results);
			ReadWriteOnWorkerThread(timer, di, results);

#if false
			using (var transaction = new DbTransaction())
			{
				transaction.Enqueue(() => Console.WriteLine("Enqueue1"));
				transaction.Enqueue(() => Console.WriteLine("Enqueue2"));
				transaction.Enqueue(() => Console.WriteLine("Enqueue3"));
				transaction.Enqueue(() => Console.WriteLine("Enqueue4"));
				transaction.Enqueue(() => Console.WriteLine("Enqueue5"));
				transaction.Join();
			}
#endif

			Console.WriteLine("==================================");
			foreach (var result in results)
			{
				Console.WriteLine("result[{0}] {1}msec", result.Item1, result.Item2);
			}

			Console.ReadKey();
		}

		private static void ReadOnMultThreadAndWriteOnSingleThread(Stopwatch timer, DirectoryInfo di, List<Tuple<string, long>> results)
		{
// read(multi threads)/write(single thread)
			timer.Start();
			var sample3 = new MediaSample3();
			Parallel.ForEach(di.GetFiles("*.*", SearchOption.AllDirectories), fileInfo =>
			{
				if (!sample3.HasMedia(fileInfo.FullName))
				{
					sample3.Add(fileInfo);
					//				sample.Add(fileInfo.FullName);
				}
				else
				{
					sample3.Add(fileInfo);
				}

				sample3.Dump();
			});
			timer.Stop();
			results.Add(Tuple.Create("multi thread (read)/single thread (write)", timer.ElapsedMilliseconds));
			timer.Reset();
		}

		private static MediaSample ReadWriteOnMultiThread(Stopwatch timer, DirectoryInfo di, List<Tuple<string, long>> results)
		{
// multi threads
			timer.Start();
			var sample = new MediaSample();
			Parallel.ForEach(di.GetFiles("*.*", SearchOption.AllDirectories), fileInfo =>
			{
				if (!sample.HasMedia(fileInfo.FullName))
				{
					sample.Add(fileInfo);
					//				sample.Add(fileInfo.FullName);
				}
				else
				{
					sample.Add(fileInfo);
				}

				sample.Dump();
			});
			timer.Stop();
			results.Add(Tuple.Create("multi thread (read/write)", timer.ElapsedMilliseconds));
			timer.Reset();
			return sample;
		}

		private static void ReadWriteOnSingleThread(Stopwatch timer, DirectoryInfo di, List<Tuple<string, long>> results)
		{
			MediaSample2 sample2;
// read/write
			// single thread
			timer.Start();
			sample2 = new MediaSample2();
			foreach (var fileInfo in di.GetFiles("*.*", SearchOption.AllDirectories))
			{
				if (!sample2.HasMedia(fileInfo.FullName))
				{
					sample2.Add(fileInfo);
					//				sample.Add(fileInfo.FullName);
				}
				else
				{
					sample2.Add(fileInfo);
				}

				sample2.Dump();
			}
			timer.Stop();
			results.Add(Tuple.Create("single thread (read/write)", timer.ElapsedMilliseconds));
			timer.Reset();
		}

		private static MediaSample ReadOnMultiThread(Stopwatch timer, DirectoryInfo di, List<Tuple<string, long>> results)
		{
// multi threads
			timer.Start();
			var sample = new MediaSample();
			Parallel.ForEach(di.GetFiles("*.*", SearchOption.AllDirectories), fileInfo =>
			{
				if (!sample.HasMedia(fileInfo.FullName))
				{
					sample.Add(fileInfo);
					//				sample.Add(fileInfo.FullName);
				}

				sample.Dump();
			});
			timer.Stop();
			results.Add(Tuple.Create("multi thread", timer.ElapsedMilliseconds));
			timer.Reset();
			return sample;
		}

		private static void ReadOnSingleThread(Stopwatch timer, DirectoryInfo di, List<Tuple<string, long>> results)
		{
// single thread
			timer.Start();
			var sample2 = new MediaSample2();
			foreach (var fileInfo in di.GetFiles("*.*", SearchOption.AllDirectories))
			{
				if (!sample2.HasMedia(fileInfo.FullName))
				{
					sample2.Add(fileInfo);
//				sample.Add(fileInfo.FullName);
				}

				sample2.Dump();
			}
			timer.Stop();
			results.Add(Tuple.Create("single thread", timer.ElapsedMilliseconds));
			timer.Reset();
		}

		private static void ReadWriteOnWorkerThread(Stopwatch timer, DirectoryInfo di, List<Tuple<string, long>> results)
		{
			timer.Start();
			using (var transaction = new DbTransaction2())
			{
//				var di = new DirectoryInfo(@"E:\data\SAIPAN");
				var rows = new List<Task<Media>>();
//				foreach (var fi in di.GetFiles("*.*", SearchOption.AllDirectories))
//					rows.Add(transaction.Enqueue(context =>
//					{
//						var media = new Media {FilePath = fi.FullName, CreationTime = fi.CreationTime, LastWriteTime = fi.LastWriteTime};
//						return context.Medias.Add(media);
//					}));
				Parallel.ForEach(di.GetFiles("*.*", SearchOption.AllDirectories), fi =>
				{
					rows.Add(transaction.Enqueue(context =>
					{
						var media = new Media {FilePath = fi.FullName, CreationTime = fi.CreationTime, LastWriteTime = fi.LastWriteTime};
						return context.Medias.Add(media);
					}));
				});
				transaction.Commit();
				foreach (var item in rows)
				{
//					Console.WriteLine("Added media path:{0} create:{1} update:{2}", item.Result.FilePath, item.Result.CreationTime, item.Result.LastWriteTime);
				}
//				transaction.Enqueue(context =>
//				{
//					foreach (var media in context.Medias.Select(i => i))
//					{
//						Console.WriteLine("media path:{0} create:{1} update:{2}", media.FilePath, media.CreationTime, media.LastWriteTime);
//					}
//				});
				var task = transaction.Enqueue(context => context.Medias.Select(i => i).ToList());
				foreach (var media in task.Result)
				{
//					Console.WriteLine("media path:{0} create:{1} update:{2}", media.FilePath, media.CreationTime, media.LastWriteTime);
				}

				transaction.Join();
			}
			timer.Stop();
			results.Add(Tuple.Create("worker thread (read/write)", timer.ElapsedMilliseconds));
			timer.Reset();
		}
	}
}
