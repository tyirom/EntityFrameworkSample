// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaSample.cs" company="Grass Valley K.K.">
//   Copyright (C) 2017 Grass Valley K.K. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace EntityFrameworkSample
{
	using System.IO;

	class Program
	{
		static void Main(string[] args)
		{
			var sample = new MediaSample();
			var di = new DirectoryInfo(@"E:\data\SAIPAN");
			foreach (var fileInfo in di.GetFiles())
			{
				if (sample.HasMedia(fileInfo.FullName))
				{
					continue;
				}

				sample.Add(fileInfo);
//				sample.Add(fileInfo.FullName);
			}

			sample.Dump();

			Console.ReadKey();
		}
	}
}
