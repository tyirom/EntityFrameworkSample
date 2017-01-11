// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaSample.cs" company="Grass Valley K.K.">
//   Copyright (C) 2017 Grass Valley K.K. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace EntityFrameworkSample
{
	using System;
	using System.IO;
	using System.Linq;
	using EntityFrameworkSample.Model;

	public class MediaSample
	{
//		public void Add(string filePath)
//		{
//			using (var db = new Repository())
//			{
//				var media = new Media();
//				media.FilePath = filePath;
//				db.Medias.Add(media);
//				var task = db.SaveChangesAsync();
//				task.Wait();
//			}
//		}

		public void Add(FileInfo fi)
		{
			using (var db = new Repository())
			{
				var media = new Media();
				media.FilePath = fi.FullName;
				media.CreationTime = fi.CreationTime;
				media.LastWriteTime = fi.LastWriteTime;
				db.Medias.Add(media);
				db.SaveChanges();
			}
		}

		public void Dump()
		{
			using (var db = new Repository())
			{
				foreach (var media in db.Medias.Select(i => i))
				{
					Console.WriteLine("media path:{0} create:{1} update:{2}", media.FilePath, media.CreationTime, media.LastWriteTime);
				}
			}
		}

		public bool HasMedia(string filePath)
		{
			using (var db = new Repository())
			{
				return db.Medias.Any(m => m.FilePath == filePath);
			}
		}
	}
}