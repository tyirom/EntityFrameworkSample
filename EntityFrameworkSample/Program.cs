// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaSample.cs" company="Grass Valley K.K.">
//   Copyright (C) 2017 Grass Valley K.K. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading.Tasks;

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
			List<long> results = new List<long>();

			// single thread
			timer.Start();
			var sample2 = new MediaSample2();
			foreach (var fileInfo in di.GetFiles())
			{
				if (!sample2.HasMedia(fileInfo.FullName))
				{
					sample2.Add(fileInfo);
//				sample.Add(fileInfo.FullName);
				}

				sample2.Dump();
			}
			timer.Stop();
			results.Add(timer.ElapsedMilliseconds);
			timer.Reset();

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
			results.Add(timer.ElapsedMilliseconds);
			timer.Reset();

			// read/write
			// single thread
			timer.Start();
			sample2 = new MediaSample2();
			foreach (var fileInfo in di.GetFiles())
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
			results.Add(timer.ElapsedMilliseconds);
			timer.Reset();

			// multi threads
			timer.Start();
			sample = new MediaSample();
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
			results.Add(timer.ElapsedMilliseconds);
			timer.Reset();

			// read/write
			// multi threads
			timer.Start();
			sample = new MediaSample();
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
			results.Add(timer.ElapsedMilliseconds);
			timer.Reset();

			// single thread
			timer.Start();
			sample2 = new MediaSample2();
			foreach (var fileInfo in di.GetFiles())
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
			results.Add(timer.ElapsedMilliseconds);
			timer.Reset();

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
			results.Add(timer.ElapsedMilliseconds);
			timer.Reset();
			
			// single thread
			timer.Start();
			sample2 = new MediaSample2();
			foreach (var fileInfo in di.GetFiles())
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
			results.Add(timer.ElapsedMilliseconds);
			timer.Reset();

			Console.WriteLine("==================================");
			sample.Dump();

			foreach (var result in results)
			{
				Console.WriteLine("result : {0}msec", result);
			}

			Console.ReadKey();
		}
	}
}
